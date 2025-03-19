using UnityEngine;

public class RegisterController : MonoBehaviour
{
    private static bool isOpen = false;
    private Animator animator;

    void Start()
    {
        GameObject registerObject = GameObject.FindWithTag("Register");

        if (registerObject != null)
        {
            animator = registerObject.GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("Animator component is missing on the Register object!");
            }
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Register' found in the scene!");
        }
    }

    public void ToggleRegister()
    {
        if (animator == null)
        {
            Debug.LogError("Cannot toggle Register! Animator is missing.");
            return;
        }

        isOpen = !isOpen;
        string triggerName = isOpen ? "TrOpen" : "TrClose";
        animator.SetTrigger(triggerName);

        PlayRegisterSound();
    }

    private void PlayRegisterSound()
    {
        if (SoundFXManager.instance == null)
        {
            Debug.LogError("SoundFXManager instance is missing! Cannot play register sound.");
            return;
        }

        AudioClip soundToPlay = isOpen ? SoundFXManager.instance.registerOpen : SoundFXManager.instance.registerClose;
        SoundFXManager.instance.PlaySound(soundToPlay, transform.position, 2f);
    }

    public static bool IsOpen()
    {
        return isOpen;
    }
}
