using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ScreenController : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI textBox;
    private static int paymentAmount;
    private static int ChangeAmount;

    public bool isActive = false;
    [SerializeField] private UnityEvent onClick;

    void Start()
    {
        if (textBox == null)
        {
            return;
        }
    
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("No Collider found on Screen! Add a BoxCollider or MeshCollider.");
        }
    }

    public void ActivateScreen()
    {
        isActive = true;
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

    public void UpdateText(string text)
    {
        textBox.text = text;
    }

    private int GetRandomValue()
    {
        int min = 1000;
        int max = 500000;
        int step = 100;
        float t = Mathf.Pow(Random.value, 2.2f);
        int rawValue = Mathf.RoundToInt(Mathf.Lerp(min, max, t));
        int snappedValue = (rawValue / step) * step;

        return snappedValue;
    }


    public static int GetPaymentAmount()
    {
        return paymentAmount;
    }

    public static int GetChangeAmount()
    {
        return ChangeAmount;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive)
        {
            Debug.Log("Screen is not active yet! Click ignored.");
            return;
        }
        onClick?.Invoke();
    }
}
