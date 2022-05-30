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
        GameLogger.Log("creating fade in instance");
    }

    public void DoFadeIn()
    {
        GameLogger.Log("doing fade in");
        fadeInAnimator.Play(fadeInClip.name);
    }

    public void CallOnFadeInComplete()
    {
        GameLogger.Log("on fade in complete"); 
        OnFadeInComplete();
    }
}
