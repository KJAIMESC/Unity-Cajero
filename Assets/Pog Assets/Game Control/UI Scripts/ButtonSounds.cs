using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public static ButtonSounds instance;

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
        }
    }

    public void PlayButtonSound()
    {
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlayPersistentSound(SoundFXManager.instance.buttonClick, Vector3.zero, 1f);
        }
    }

    public void PlayToggleSound()
    {
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlaySound(SoundFXManager.instance.toggleSwitch, Vector3.zero, 1f);
        }
    }
}
