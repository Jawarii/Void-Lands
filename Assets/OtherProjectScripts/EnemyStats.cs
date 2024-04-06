using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Unity.VisualScripting.Member;
using TMPro;

public class EnemyStats : MonoBehaviour
{
    public float maxHp = 100;
    public float hp = 100;
    public float attack = 10;
    public float defense = 10;
    public float timeSince = 0;
    public GameObject player;
    public Animator animator_;
    // public GameObject hitAudio;
    private PlayerStats stats;
    private float prevHp;
    public int enemyLvl = 1;

    private float deathDuration = 0;
    private bool isDead = false;

    public float hitIndicator = 0f;

    private Color baseColor;

    public AudioSource source;
    public AudioClip clip;
    public List<GameObject> goldItems = new List<GameObject>();
    [SerializeField] private DamagePopup damagePopup;
    private System.Random random = new System.Random();

    public bool isBoss = false;
    private float goldAmount = 0;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
       //hitAudio = GameObject.FindGameObjectWithTag("HitAudio");
        stats = player.GetComponentInChildren<PlayerStats>();


        maxHp = 25f + (enemyLvl - 1) * 125f;
        hp = maxHp;
        prevHp = hp;

        attack = 10f + (enemyLvl - 1) * 100f;
        defense = (enemyLvl - 1) * 50f;

        baseColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        if (hitIndicator > 0)
        {
            hitIndicator -= Time.deltaTime;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = baseColor;
        }
        timeSince += Time.deltaTime;
        if (prevHp != hp)
        {
            timeSince = 0;
            prevHp = hp;
        }
        if (hp <= 0)
        {
            if (deathDuration > 0.5f)
            {   if (goldAmount > 0 && goldAmount < 10)
                {
                    GameObject goldDrop = Instantiate(goldItems[0], transform.position, transform.rotation);
                    goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
                    goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
                }
                else if(goldAmount >= 10 && goldAmount < 50)
                {
                    GameObject goldDrop = Instantiate(goldItems[1], transform.position, transform.rotation);
                    goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
                    goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
                }
                else
                {
                    GameObject goldDrop = Instantiate(goldItems[2], transform.position, transform.rotation);
                    goldDrop.GetComponentInChildren<TMP_Text>().text = goldAmount.ToString() + " Gold";
                    goldDrop.GetComponent<GoldDropBehaviour>().goldAmount = goldAmount;
                }

                Destroy(gameObject);
            }
            else
            {
                if (!isDead)
                {
                    goldAmount = Random.Range(2, 6);
                    animator_.SetBool("isDead", true);
                    isDead= true;
                }
                deathDuration += Time.deltaTime;
                gameObject.GetComponent<SlimeMovement>().enabled = false;
            }
        }
    }

      public void TakeDamage(int damage, bool isCrit)
    {
        source.Stop();
        hp -= damage;
        damagePopup.isPlayer = false;
        damagePopup.Create(transform.position, damage, isCrit);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        hitIndicator = 0.12f;
        source.PlayOneShot(clip);
        SlimeMovement enemyMovement = transform.GetComponent<SlimeMovement>();
        enemyMovement.inPursuit = true;
    }
    public void ApplyKnockback(Vector2 direction, float force)
    {
        // Move the enemy in the specified direction with the specified force
        transform.position += (Vector3)direction * force * Time.deltaTime;
    }

}
