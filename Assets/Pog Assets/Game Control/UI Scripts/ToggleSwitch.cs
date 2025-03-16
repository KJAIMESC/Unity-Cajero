using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Toggle Slider Settings")]
    [SerializeField, Range(0, 1f)] private float sliderValue;
    [SerializeField] private string playerPrefsKey = "DefaultToggle";
    
    public bool CurrentValue { get; private set; }

    private Slider slider;

    [Header("Animation")]
    [SerializeField, Range(0, 1f)] private float animationSpeed = 0.5f;
    [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine animateToggleCoroutine;

    [Header("Events")]
    [SerializeField] private UnityEvent onToggleOn;
    [SerializeField] private UnityEvent onToggleOff;

    private ToggleSwitchGroupManager toggleSwitchGroupManager;
    protected Action transitionEffect;


    protected void OnValidate()
    {
        SetupToggleComponents();
        slider.value = sliderValue;
    }

    private void SetupToggleComponents()
    {
        if(slider != null) return;
        SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
           Debug.LogError("Slider component not found on GameObject!");
           return;
        }
        
        slider.interactable = false;
        var sliderColors = slider.colors;
        sliderColors.disabledColor = sliderColors.normalColor;
        slider.colors = sliderColors;
        slider.transition = Selectable.Transition.None;
    }

    public void SetupForManager(ToggleSwitchGroupManager manager)
    {
        toggleSwitchGroupManager = manager;
    }

    private void Awake()
    {
        SetupToggleComponents();
        LoadToggleState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();

        if (ButtonSounds.instance != null)
        {
            ButtonSounds.instance.PlayToggleSound();
        }
        else
        {
            Debug.LogError("ButtonSounds instance is null!");
        }
    }

    private void Toggle()
    {
        if (toggleSwitchGroupManager != null)
        {
            toggleSwitchGroupManager.ToggleGroup(this);
        }
        else
        {
            SetStateAndStartAnimation(!CurrentValue);
        }
    }

    public void ToggleByGroupManager(bool valueToSetTo)
    {
        SetStateAndStartAnimation(valueToSetTo);
    }

    public void SetStateAndStartAnimation(bool state){
        CurrentValue = state;
        SaveToggleState();
        if (CurrentValue)
        {
            onToggleOn.Invoke();
        }
        else
        {
            onToggleOff.Invoke();
        }
        if(animateToggleCoroutine != null) StopCoroutine(animateToggleCoroutine);
        animateToggleCoroutine = StartCoroutine(AnimateSlider());
    }

    private IEnumerator AnimateSlider()
    {
        float startValue = slider.value;
        float endValue = CurrentValue ? 1 : 0;

        float time = 0;
        if (animationSpeed > 0)
        {
            while (time < animationSpeed)
            {
                time += Time.deltaTime;

                float lerpFactor = slideEase.Evaluate(time / animationSpeed);
                slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                transitionEffect?.Invoke();
                        
                yield return null;
            }
        }

        slider.value = endValue;
    }

    private void SaveToggleState()
    {
        PlayerPrefs.SetInt(playerPrefsKey, CurrentValue ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadToggleState()
    {
        CurrentValue = PlayerPrefs.GetInt(playerPrefsKey, 0) == 1;
        sliderValue = CurrentValue ? 1f : 0f;
        slider.value = sliderValue;
    }
}
