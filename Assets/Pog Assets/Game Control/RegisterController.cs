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
                Debug.LogError("❌ Animator component is missing on the Register object!");
            }
            else
            {
                Debug.Log("✅ Animator successfully found on Register.");
            }
        }
        else
        {
            Debug.LogError("❌ No GameObject with tag 'Register' found in the scene!");
        }
    }

    public void ToggleRegister()
    {
        if (animator == null)
        {
            Debug.LogError("❌ Cannot toggle Register! Animator is missing.");
            return;
        }

        isOpen = !isOpen; // ✅ Update isOpen when toggling
        string triggerName = isOpen ? "TrOpen" : "TrClose";
        animator.SetTrigger(triggerName);

        Debug.Log("✅ Register toggled: " + triggerName + ", isOpen = " + isOpen); // 🔹 Debugging log
    }

    public static bool IsOpen()
    {
        Debug.Log("🔍 Checking register state: " + isOpen); // 🔹 Debugging log
        return isOpen;
    }
}
