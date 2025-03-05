using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ScreenController : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI textBox;
    private static int paymentAmount;
    private CameraController cameraController;
    private static int ChangeAmount;

    public bool isActive = false;

    void Start()
    {
        if (textBox == null)
        {
            Debug.LogError("❌ TextMeshProUGUI component not assigned in ScreenController!");
            return;
        }

        cameraController = FindFirstObjectByType<CameraController>();

        if (cameraController == null)
        {
            Debug.LogError("❌ CameraController not found in the scene!");
        }

    
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("❌ No Collider found on Screen! Add a BoxCollider or MeshCollider.");
        }
        else
        {
            Debug.Log("✅ Collider found on Screen: " + col.GetType().Name);
        }
    }

    public void ActivateScreen()
    {
        isActive = true;
        Debug.Log("✅ Screen is now clickable!");
    }

    public void UpdateRandomNumber()
    {
        int randomValue = GetRandomValue();
        textBox.text = "Total por pagar: " + randomValue.ToString("N0"); 
        paymentAmount = randomValue;
    }

    public void UpdateChangeAmount()
    {
        ChangeAmount = CustomerMovement.GetPaymentAmount() - paymentAmount;
        textBox.text = "Cambio: " + (ChangeAmount).ToString("N0");
    } 

    private int GetRandomValue()
    {
        int min = 1000;
        int max = 1000000;
        int step = 100;

        int randomValue = Random.Range(min / step, max / step) * step;
        return randomValue;
    }

    public static int GetPaymentAmount()
    {
        return paymentAmount;
    }

    public static int GetChangeAmount()
    {
        return ChangeAmount;
    }

    public void TriggerScreenSwitch()
    {
        if (cameraController != null)
        {
            cameraController.SwitchCameras();
            Debug.Log("✅ Camera switched!");
        }
        else
        {
            Debug.LogWarning("⚠ CameraController is not assigned!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive)
        {
            Debug.Log("⚠ Screen is not active yet! Click ignored.");
            return;
        }

        Debug.Log("✅ Screen clicked! Switching cameras...");
        TriggerScreenSwitch();
    }
}
