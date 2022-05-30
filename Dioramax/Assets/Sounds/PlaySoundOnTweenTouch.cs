using UnityEngine;

public class PlaySoundOnTweenTouch : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clipOnTouchTween;
    [SerializeField] private AudioClip[] randArrayClipsOnTouchTween = new AudioClip[3];
    [SerializeField] private AudioClip[] pingPongOnTouchTween = new AudioClip[2];
    [SerializeField] private bool isTutoCatButton;
    [SerializeField] private bool isMill;
    [SerializeField] private bool isHydrant;


    private bool isOn; 
    public void Play(SoundPlayType playType = SoundPlayType.NONE)
    {
        try
        {
            if (playType == SoundPlayType.Rand)
            {
                AudioManager.Instance.PlayRandomSound(audioSource, randArrayClipsOnTouchTween);
            }
            else if (playType == SoundPlayType.PingPong)
            {
                isOn = !isOn;
                AudioManager.Instance.PlaySoundPingPong(audioSource, pingPongOnTouchTween, BoolToInt(isOn));
            }
            else if (playType == SoundPlayType.NONE)
            {
                if (isTutoCatButton && TutorialWinConditionController.stateIsPuzzle1
                    || LevelManager.Instance.ValidatedPuzzleAmount == 1 && isHydrant
                    || LevelManager.Instance.ValidatedPuzzleAmount == 2 && isMill) return;

                AudioManager.Instance.PlaySound(audioSource, clipOnTouchTween);
            }
        }
        catch (System.Exception)
        {
            GameLogger.Log($"no sound to play was found on {gameObject.name}");
        }
    }

    private int BoolToInt(bool value) => value == true ? 1 : 0; 
}
