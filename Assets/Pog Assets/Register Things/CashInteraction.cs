using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CashInteraction : MonoBehaviour, IPointerClickHandler
{
    public GameObject cashPrefab;
    public Transform spawnPoint;
    public int cashValue;
    public TMP_Text cashCounterText;
    public float spawnRadius = 0.05f;
    public float maxRotationOffset = 5f;
    public bool isBill;

    private static int totalCash = 0;
    private ObjectInteraction registerScript;

    private void Start()
    {
        GameObject register = GameObject.FindWithTag("Register");
        if (register != null)
        {
            registerScript = register.GetComponent<ObjectInteraction>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (registerScript != null && registerScript.IsRegisterOpen()) 
        {
            SpawnCash();
            totalCash += cashValue;
            Debug.Log("Cash Collected! Total: " + totalCash);

            if (cashCounterText != null)
            {
                cashCounterText.text = totalCash.ToString();
            }
        }
        else
        {
            Debug.Log("Register is closed! Can't collect cash.");
        }
    }

    void SpawnCash()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );

        Vector3 spawnPosition = spawnPoint.position + randomOffset;

        Quaternion spawnRotation;
        if (isBill)
        {
            spawnRotation = Quaternion.Euler(-90, Random.Range(-10f, 10f), 0);
        }
        else
        {
            float randomZRotation = Random.Range(-maxRotationOffset, maxRotationOffset);
            spawnRotation = Quaternion.Euler(0, 0, randomZRotation);
        }

        GameObject spawnedCash = Instantiate(cashPrefab, spawnPosition, spawnRotation);

        Rigidbody rb = spawnedCash.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedCash.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        Collider collider = spawnedCash.GetComponent<Collider>();
        if (collider == null)
        {
            collider = spawnedCash.AddComponent<BoxCollider>();
        }

        CashPickup pickupScript = spawnedCash.GetComponent<CashPickup>();
        if (pickupScript == null)
        {
            pickupScript = spawnedCash.AddComponent<CashPickup>();
        }
        pickupScript.SetValue(cashValue);
    }

    public static int GetTotalCash()
    {
        return totalCash;
    }

    public static void reduceTotalCash(int value)
    {
        totalCash -= value;
    }
}
