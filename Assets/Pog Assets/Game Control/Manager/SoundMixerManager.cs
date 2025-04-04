using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Button muteButton;
    [SerializeField] private Image muteButtonImage;
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Sprite unmutedSprite;

    private bool isMuted = false;

    private void Start()
    {
        if (muteButton == null || muteButtonImage == null)
        {
            Debug.LogError("Mute Button or Image component is not assigned in the Inspector!");
            return;
        }

        // Load saved settings
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        // Always set stored values in PlayerPrefs (even if muted)
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();

        ApplyMuteState(); // Apply mute or restore volumes

        muteButton.onClick.AddListener(ToggleMute);
    }

    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();

        if (!isMuted) // Only apply to the mixer if not muted
        {
            audioMixer.SetFloat("MasterVolume", ConvertToDecibels(volume));
        }
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();

        if (!isMuted)
        {
            audioMixer.SetFloat("MusicVolume", ConvertToDecibels(volume));
        }
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();

        if (!isMuted)
        {
            audioMixer.SetFloat("SFXVolume", ConvertToDecibels(volume));
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        ButtonSounds.instance.PlayToggleSound();
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMuteState();
    }

    private void ApplyMuteState()
    {
        if (muteButtonImage == null) return;

        if (isMuted)
        {
            audioMixer.SetFloat("MasterVolume", -80f);
            muteButtonImage.sprite = mutedSprite;
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", ConvertToDecibels(PlayerPrefs.GetFloat("MasterVolume", 0.75f)));
            audioMixer.SetFloat("MusicVolume", ConvertToDecibels(PlayerPrefs.GetFloat("MusicVolume", 0.75f)));
            audioMixer.SetFloat("SFXVolume", ConvertToDecibels(PlayerPrefs.GetFloat("SFXVolume", 0.75f)));

            muteButtonImage.sprite = unmutedSprite;
        }
    }

    public void RefreshAllVolumes()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        if (!isMuted)
        {
            audioMixer.SetFloat("MasterVolume", ConvertToDecibels(masterVolume));
            audioMixer.SetFloat("MusicVolume", ConvertToDecibels(musicVolume));
            audioMixer.SetFloat("SFXVolume", ConvertToDecibels(sfxVolume));
        }
    }

    private float ConvertToDecibels(float value)
    {
        return value > 0 ? Mathf.Log10(value) * 20f : -80f;
    }
}
