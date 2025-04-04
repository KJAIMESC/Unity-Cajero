using UnityEngine;
using UnityEngine.EventSystems;

public class CashPickup : MonoBehaviour, IPointerClickHandler
{
    private int cashValue;
    private bool isBill;

    public void SetValue(int value, bool isBillType)
    {
        cashValue = value;
        isBill = isBillType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound();
        CashInteraction.reduceTotalCash(cashValue);
        Destroy(gameObject);
    }
    
    public void OnPointerClick(PointerEventData eventData, bool silentDelete)
    {
        CashInteraction.reduceTotalCash(cashValue);
        Destroy(gameObject);
    }

    private void PlaySound()
    {
        if (SoundFXManager.instance != null)
        {
            if (isBill)
            {
                SoundFXManager.instance.PlaySound(SoundFXManager.instance.Cash, transform.position, 1f);
            }
            else
            {
                SoundFXManager.instance.PlaySound(SoundFXManager.instance.Coin, transform.position, 1f);
            }
        }
        else
        {
            Debug.LogWarning("CashPickup: SoundFXManager instance is missing!");
        }
    }
}
