using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{   
    public static SoundFXManager instance;

    [Header("Button Sounds")]
    public AudioClip buttonClick;
    public AudioClip toggleSwitch;

    [Header("Register Sounds")]
    public AudioClip registerOpen;
    public AudioClip registerClose;
    public AudioClip Purchase;

    [Header("Money Sounds")]
    public AudioClip Cash;
    public AudioClip Coin;

    [Header("Customer Sounds")]
    public AudioClip customerWalking;

    [Header("Feedback Sounds")]
    public AudioClip correctPlacement;
    public AudioClip incorrectPlacement;

    [Header("Audio Sources")]

    [SerializeField] private AudioSource soundFXPrefab;
    [SerializeField] public AudioMixerGroup sfxMixerGroup;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
            return;
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume)
    {
        if (clip == null) return;

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
        if (clip == null) return null;

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
