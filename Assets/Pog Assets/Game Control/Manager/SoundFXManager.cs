using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{   
    public static SoundFXManager instance;
    public AudioClip buttonClick;
    public AudioClip toggleSwitch;

    [SerializeField] private AudioSource soundFXPrefab;
    [SerializeField] private AudioMixerGroup sfxMixerGroup; // 🎚 Assign this in Inspector

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep manager persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return; // Prevent duplicate instance issues
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume)
    {
        if (clip == null) return; // Prevent errors if clip is missing

        AudioSource audioSource = Instantiate(soundFXPrefab, position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.outputAudioMixerGroup = sfxMixerGroup; 
        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length);
    }
    
    public AudioSource PlayPersistentSound(AudioClip clip, Vector3 position, float volume)
    {
        if (clip == null) return null; // Prevent errors if clip is missing

        GameObject soundObject = new GameObject("PersistentSoundFX");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.outputAudioMixerGroup = sfxMixerGroup; 
        audioSource.Play();

        DontDestroyOnLoad(soundObject);

        Destroy(soundObject, clip.length);

        return audioSource;
    }
}
