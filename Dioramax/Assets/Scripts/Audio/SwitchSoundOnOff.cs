using UnityEngine;
using UnityEngine.Audio;

public class SwitchSoundOnOff : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; 
    [SerializeField] private TweenTouch[] tweens = new TweenTouch[2]; // ON -> 0, OFF -> 1
    [SerializeField] private TMPro.TMP_Text soundTmp;

    [Space, SerializeField] private GameObject soundOFF;
    private bool showSoundOffIcon;
    private float decibels; 
    public void SetSound()
    {
        showSoundOffIcon = !showSoundOffIcon;

        soundOFF.SetActive(showSoundOffIcon);
        tweens[BoolToInt(showSoundOffIcon)].Tween();
        soundTmp.text = showSoundOffIcon ? "Unmute" : "Mute";

        decibels = 0f;
        CancelInvoke(nameof(DecreaseSound));

        if (audioMixer)
        {
            audioMixer.SetFloat("masterVolume", decibels);
        }

        if (showSoundOffIcon)
        {
            InvokeRepeating(nameof(DecreaseSound), 0f, Time.deltaTime); 
        }
    }

    private void DecreaseSound()
    {
        if (audioMixer)
        {
            decibels -= 80f * Time.deltaTime;
            audioMixer.SetFloat("masterVolume", decibels);

            if (decibels <= -80f)
            {
                CancelInvoke(nameof(DecreaseSound));
            }
        }
    }

    private int BoolToInt(bool value) => value == true ? 1 : 0; 
}
