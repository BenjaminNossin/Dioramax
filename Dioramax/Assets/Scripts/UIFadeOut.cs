using UnityEngine;

public class UIFadeOut : MonoBehaviour
{
    public static UIFadeOut Instance;

    [SerializeField] private Animator fadeOutAnimator;
    [SerializeField] private AnimationClip fadeOutClip;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    public void DoFadeOut()
    {
        fadeOutAnimator.Play(fadeOutClip.name);
    }
}
