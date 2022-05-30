using UnityEngine;

public class UIFadeIn : MonoBehaviour
{
    public static UIFadeIn Instance;

    [SerializeField] private Animator fadeInAnimator;
    [SerializeField] private AnimationClip fadeInClip;

    public static System.Action OnFadeInComplete { get; set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    public void DoFadeIn()
    {
        fadeInAnimator.Play(fadeInClip.name);
    }
}
