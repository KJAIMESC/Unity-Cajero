using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private string playerPrefKey;

    void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(playerPrefKey, 0.75f);
        slider.value = savedValue;

        UpdateText(savedValue); 
        slider.onValueChanged.AddListener(UpdateText);
        slider.onValueChanged.AddListener(SaveSliderValue); 
    }

    private void UpdateText(float value)
    {
        sliderText.text = Mathf.RoundToInt(value * 100).ToString();
    }

    private void SaveSliderValue(float value)
    {
        PlayerPrefs.SetFloat(playerPrefKey, value);
        PlayerPrefs.Save();
    }
}
