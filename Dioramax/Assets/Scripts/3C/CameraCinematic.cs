using UnityEngine;
using UnityEngine.UI; 

public class CameraCinematic : MonoBehaviour
{
    public static CameraCinematic Instance { get; private set; }

    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip openingCinematic; // TODO : REFACTORING

    [SerializeField] private AnimationClip phase2Cinematic; // TODO : REFACTORING

    [Space, SerializeField] private Image cameraFadePanelImage;
    [SerializeField, Range(1f, 5f)] private float fadeOutSpeedMultiplier = 1f;
    [SerializeField, Range(1f, 5f)] private float fadeInSpeedMultiplier = 2f;
    [SerializeField, Range(0.1f, 5f)] private float delayFromFadeOutToFadeIn = 0.5f;

    public static System.Action OnFadeOutComplete;
    public static System.Action OnFadeInComplete;

    private void Awake()
    {
        animator.applyRootMotion = false; 
        if (Instance)
        {
            Destroy(Instance); 
        }
        Instance = this; 
    }

    private void Start()
    {
        animator.Play(openingCinematic.name); 
    }

    public void FadeCameraPanel()
    {
        InvokeRepeating(nameof(DoFadeOut), 0f, Time.deltaTime);
        LevelManager.Instance.SetGameState(GameState.Cinematic); 
    }

    private void DoFadeOut()
    {
        cameraFadePanelImage.color += new Color(0f, 0f, 0f, Time.deltaTime * fadeOutSpeedMultiplier);
        if (cameraFadePanelImage.color.a >= 1f)
        {
            OnFadeOutComplete();
            CancelInvoke(nameof(DoFadeOut));
            InvokeRepeating(nameof(DoFadeIn), delayFromFadeOutToFadeIn, Time.deltaTime);
        }
    }

    private void DoFadeIn()
    {
        cameraFadePanelImage.color -= new Color(0f, 0f, 0f, Time.deltaTime * fadeOutSpeedMultiplier);
        if (cameraFadePanelImage.color.a <= 0f)
        {
            OnFadeInComplete(); 
            CancelInvoke(nameof(DoFadeIn)); 
        }
    }

    // done at the end of every cinematic
    public void SetAnimatorState(int enabled)  
    {
        animator.enabled = enabled == 1; // PLACEHOLDER until I find how to get control of movement when animation is done; 
    }

    // abstract this out
    public void PlayPhase2Cinematic()
    {
        SetAnimatorState(1);
        animator.Play(phase2Cinematic.name); 
    }
}
