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
        GameLogger.Log("creating fade out instance");
    }


    public void DoFadeOut()
    {
        GameLogger.Log("doing fade out"); 
        fadeOutAnimator.Play(fadeOutClip.name);
    }
}
