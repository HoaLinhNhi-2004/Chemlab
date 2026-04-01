using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class NPCController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clapClip;
    [SerializeField] private int clapLoopCount = 2;

    private Coroutine animationCoroutine;
    private bool isClapping;

    private static readonly int ClappingState = Animator.StringToHash("Clapping");
    private static readonly int IdleState = Animator.StringToHash("Idle");

    private void Reset()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isClapping) return;

            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            animationCoroutine = StartCoroutine(PlayClappingLoopsThenIdle());
        }
    }

    private IEnumerator PlayClappingLoopsThenIdle()
    {
        isClapping = true;

        animator.Play(ClappingState, 0, 0f);

        yield return null;

        while (animator.IsInTransition(0))
            yield return null;

        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.shortNameHash == ClappingState && stateInfo.normalizedTime >= clapLoopCount)
                break;

            yield return null;
        }

        animator.Play(IdleState, 0, 0f);
        StopClapSound();

        animationCoroutine = null;
        isClapping = false;
    }

    public void PlayClapSound()
    {
        if (audioSource == null || clapClip == null) return;

        audioSource.Stop();
        audioSource.clip = clapClip;
        audioSource.Play();
    }

    public void StopClapSound()
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}