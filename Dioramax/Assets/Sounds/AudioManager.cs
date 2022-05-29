using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource ostAudioSource;

    [Space, SerializeField] private AudioClip[] OST = new AudioClip[4];
    [SerializeField] private AudioMixerGroup[] OSTGroups = new AudioMixerGroup[4];

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return; 
        }

        Instance = this;
    }

    public void PlaySound(AudioSource source, AudioClip clip, bool waitForEnd = false)
    {
        if (waitForEnd && source.isPlaying) return; 

        source.PlayOneShot(clip); 
    }

    private AudioClip currentOST;
    public bool PlayingSharedOST { get; private set; }
    public void PlayOST(int index)
    {
        currentOST = OST[index];
        PlayingSharedOST = currentOST == OST[0];

        ostAudioSource.clip = currentOST;
        ostAudioSource.outputAudioMixerGroup = OSTGroups[index]; 

        GameLogger.Log($"current ost {ostAudioSource.clip}"); 
        ostAudioSource.Stop();
        ostAudioSource.Play(); 
    }

    public void PlayRandomSound(AudioSource source, AudioClip[] clips, bool waitForEnd = false)
    {
        if (waitForEnd && source.isPlaying) return;

        source.PlayOneShot(clips[Random.Range(0, clips.Length - 1)]); 
    }
}
