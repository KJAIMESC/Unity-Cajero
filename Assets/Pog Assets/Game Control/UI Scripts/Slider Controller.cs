using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private string playerPrefKey;
    private SoundMixerManager mixerManager;

    void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(playerPrefKey, 0.75f);
        slider.value = savedValue;

        UpdateText(savedValue); 
        slider.onValueChanged.AddListener(UpdateText);
        slider.onValueChanged.AddListener(SaveSliderValue); 
        slider.onValueChanged.AddListener(UpdateMixer);
    }

    private void UpdateText(float value)
    {
        sliderText.text = Mathf.RoundToInt(value * 100).ToString();
    }

    private void SaveSliderValue(float value)
    {
        PlayerPrefs.SetFloat(playerPrefKey, value);
        PlayerPrefs.Save();
        SoundMixerManager soundMixer = FindFirstObjectByType<SoundMixerManager>();
        if (soundMixer != null)
        {
            soundMixer.RefreshAllVolumes();
        }
    }

    private void UpdateMixer(float value)
    {
        if (mixerManager == null) return;

        switch (playerPrefKey)
        {
            case "MasterVolume":
                mixerManager.SetMasterVolume(value);
                break;
            case "MusicVolume":
                mixerManager.SetMusicVolume(value);
                break;
            case "SFXVolume":
                mixerManager.SetSFXVolume(value);
                break;
        }
    }
}
