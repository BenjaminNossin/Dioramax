using UnityEngine;

public class SwitchSoundOnOff : MonoBehaviour
{
    [SerializeField] private TweenTouch[] tweens = new TweenTouch[2]; // ON -> 0, OFF -> 1
    [SerializeField] private TMPro.TMP_Text soundTmp;

    [Space, SerializeField] private GameObject soundOFF;
    private bool showSoundOffIcon; 

    public void SetSound()
    {
        soundOFF.SetActive(showSoundOffIcon);
        tweens[BoolToInt(showSoundOffIcon)].Tween();
        soundTmp.text = showSoundOffIcon ? "Mute" : "Unmute"; 

        showSoundOffIcon = !showSoundOffIcon;
    }

    private int BoolToInt(bool value) => value == true ? 1 : 0; 
}
