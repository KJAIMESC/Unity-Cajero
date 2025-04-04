using UnityEngine;
using UnityEngine.UI;

public class ForcePlayButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError($"No Button component found on {gameObject.name}!");
            return;
        }
        button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (ButtonSounds.instance == null)
        {
            Debug.LogError("ButtonSounds instance is missing!");
            return;
        }
        ButtonSounds.instance.PlayButtonSound();
    }

    public void CompletePurchase()
    {
       SoundFXManager.instance.PlaySound(SoundFXManager.instance.Purchase, transform.position, 1f);
    }

   
}

