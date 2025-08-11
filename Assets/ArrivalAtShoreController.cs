using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ArrivalAtShoreController : MonoBehaviour
{
    public GameObject player;
    public GameObject raft;
    private PlayerInformation playerInformation;

    public GameObject minimapCanvas;
    public GameObject teleportManager;
    public GameObject bow;
    public GameObject canvasesParent;
    public Canvas questCanvas;
    public PlayerMovement playerMovement;
    public Animator playerAnimator;
    public AnimationClip wakeUpAnimationClip; // Assign in inspector
    public GameObject blackOutImage;
    public CinemachineVirtualCamera virtualCamera;

    private List<Canvas> canvasesToReEnable = new List<Canvas>();
    private List<MonoBehaviour> scriptsToReEnable = new List<MonoBehaviour>();

    public Vector2 shoreTargetPosition; // Set this to where you want the raft to stop.
    public float stopThreshold = 0.1f;   // How close is "close enough" to stop the raft.

    private bool hasArrivedAtShore = false;
    public CinemachineImpulseSource impulseSource;
    public CinemachineImpulseDefinition def;

    public AudioSource sailingAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip sailingClip;
    public AudioClip impactClip;

    public CanvasGroup timeSkipCanvasGroup;
    public TMP_Text timeSkipText;

    private void Awake()
    {
        playerInformation = player.GetComponent<PlayerInformation>();
        def = impulseSource.m_ImpulseDefinition;
        if (PlayerPrefs.GetInt("IntroCompleted", 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }
        InitializeStates();
    }

    public void InitializeStates()
    {
        virtualCamera.m_Lens.OrthographicSize = 2f;
        minimapCanvas.SetActive(false);
        teleportManager.SetActive(false);
        bow.GetComponent<SpriteRenderer>().enabled = false;
        playerMovement.enabled = false;

        // Start the player in the Unconscious state
        playerAnimator.SetBool("isUnconscious", true);

        Canvas[] allCanvases = canvasesParent.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.enabled && canvas.name != "BlackOutCanvas")
            {
                canvasesToReEnable.Add(canvas);
                canvas.enabled = false;
            }

            MonoBehaviour[] allScripts = canvas.GetComponents<MonoBehaviour>();
            foreach (var script in allScripts)
            {
                if (script is CanvasScaler || script is GraphicRaycaster)
                    continue;

                if (script.enabled)
                {
                    scriptsToReEnable.Add(script);
                    script.enabled = false;
                }
            }
        }
    }
    private void Start()
    {
        questCanvas = QuestUIManager.Instance.questCanvas;
        questCanvas.enabled = false;

        SetupPlayerAndRaft();
    }

    public void SetupPlayerAndRaft()
    {
        player.transform.SetParent(raft.transform);
        player.transform.localPosition = new Vector3(0, 0.25f, 0);
        raft.GetComponent<RaftMovementController>().moveSpeed = 0.5f;

        sailingAudioSource.gameObject.SetActive(true);
        sailingAudioSource.clip = sailingClip;
        sailingAudioSource.loop = true;
        sailingAudioSource.Play();
    }

    public void EnableComponents()
    {
        StartCoroutine(EnableComponentsAfterDelay());
    }
    private void Update()
    {
        if (!hasArrivedAtShore)
        {
            float distance = Vector2.Distance(raft.transform.position, shoreTargetPosition);
            if (distance <= stopThreshold)
            {
                ArriveAtShore();
            }
        }
    }
    private void ArriveAtShore()
    {
        hasArrivedAtShore = true;
        raft.GetComponent<RaftMovementController>().moveSpeed = 0;

        sailingAudioSource.Stop();
        sailingAudioSource.gameObject.SetActive(false);
        sfxAudioSource.PlayOneShot(impactClip);

        DefineAndGenerateImpulse();
        StartCoroutine(LingerBeforeUnparent());
        raft.GetComponent<Animator>().SetBool("isMoving", false);
    }

    private IEnumerator LingerBeforeUnparent()
    {
        yield return new WaitForSeconds(4f); // Linger a bit on shore before fade

        blackOutImage.GetComponent<SceneFader>().StartFadeOut();
        // Unparent player
        yield return new WaitForSeconds(3f);
        player.transform.SetParent(null);
        // Force player to new shore position
        player.transform.position = new Vector3(2, -48.6f, 0); // Replace with your intended position
        StartTimeSkipText();
        yield return new WaitForSeconds(5f);
        blackOutImage.GetComponent<SceneFader>().StartFadeIn();
        yield return new WaitForSeconds(1f);
        StartCoroutine(ZoomOutCamera(3.5f, 2f)); // Adjust duration as needed
    }

    private IEnumerator EnableComponentsAfterDelay()
    {
        // Trigger wake-up animation
        playerAnimator.SetBool("isUnconscious", false);

        // Wait for wake-up animation length
        yield return new WaitForSeconds(wakeUpAnimationClip.length);

        minimapCanvas.SetActive(true);
        teleportManager.SetActive(true);
        bow.GetComponent<SpriteRenderer>().enabled = true;
        playerMovement.enabled = true;
        questCanvas.enabled = true;
        foreach (Canvas canvas in canvasesToReEnable)
        {
            if (canvas != null)
                canvas.enabled = true;
        }

        foreach (var script in scriptsToReEnable)
        {
            if (script != null)
                script.enabled = true;
        }

        FinishIntro();
    }

    private IEnumerator ZoomOutCamera(float targetSize, float duration)
    {
        float startSize = virtualCamera.m_Lens.OrthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize;
        EnableComponents();
    }

    public void StartTimeSkipText()
    {
        timeSkipText.enabled = true;
        timeSkipText.text = "";
        StartCoroutine(TypewriterEffect("Sometime later..."));
    }

    private IEnumerator TypewriterEffect(string text)
    {
        foreach (char c in text)
        {
            timeSkipText.text += c;

            if (c == '.')
                yield return new WaitForSeconds(0.5f); // Slower for dots
            else
                yield return new WaitForSeconds(0.05f); // Normal speed
        }
        yield return new WaitForSeconds(1f); // Small pause before fade out
        StartFadeOutTimeSkipText();
    }
    public void StartFadeOutTimeSkipText()
    {
        StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText()
    {
        float duration = 1f;
        float elapsed = 0f;
        float startAlpha = timeSkipCanvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            timeSkipCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        timeSkipCanvasGroup.alpha = 0f;
        timeSkipText.text = "";
        timeSkipText.enabled = false;
    }
    private void FinishIntro()
    {
        playerInformation.finishedIntro = true;
        PlayerPrefs.SetInt("IntroCompleted", 1);
        PlayerPrefs.Save();
    }
    public void DefineAndGenerateImpulse()
    {
        def.m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Dissipating; // Other options: Uniform, Dissipating, Propagating
        def.m_DissipationDistance = 100f;
        def.m_DissipationRate = 0f;
        def.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble; // Other options: Rumble, Bump, Recoil, Custom
        def.m_ImpulseDuration = 0.5f;
        impulseSource.m_DefaultVelocity = new Vector3(-0.3f, -0.3f, 0f);

        impulseSource.GenerateImpulse();
    }
}
