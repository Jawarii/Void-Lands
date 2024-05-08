using UnityEngine;
using UnityEditor.Animations;
using System.Collections;

public class BossPatternsAnimationController : MonoBehaviour
{
    public AnimatorController animatorController;
    public float animationDuration;

    void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animatorController = animator.runtimeAnimatorController as AnimatorController;

            if (animatorController != null && animatorController.animationClips.Length > 0)
            {
                animationDuration = animatorController.animationClips[0].length;
                StartCoroutine(DestroyAnimation());
            }
            else
            {
                Debug.LogWarning("AnimatorController or animation clips are missing.");
            }
        }
        else
        {
            Debug.LogWarning("Animator component not found.");
        }
    }
    IEnumerator DestroyAnimation()
    {
        yield return new WaitForSeconds(animationDuration);
        Destroy(gameObject);
    }
}
