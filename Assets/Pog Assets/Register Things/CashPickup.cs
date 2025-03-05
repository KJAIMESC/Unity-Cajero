using UnityEngine;
using UnityEngine.EventSystems;

public class CashPickup : MonoBehaviour, IPointerClickHandler
{
    private int cashValue;

    public void SetValue(int value)
    {
        cashValue = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CashInteraction.reduceTotalCash(cashValue);
        Debug.Log("Cash Removed! Total: " + CashInteraction.GetTotalCash());
        Destroy(gameObject);
    }
}
