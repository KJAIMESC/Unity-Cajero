using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic; 
using System.Collections;

public class CashInteraction : MonoBehaviour, IPointerClickHandler
{
    public GameObject cashPrefab;
    public Transform spawnPoint;

    public Transform clickPoint;
    public int cashValue;
    public TMP_Text cashCounterText;
    public float spawnRadius = 0.05f;
    public float maxRotationOffset = 5f;
    public bool isBill;

    private static int totalCash = 0;
    private static List<GameObject> spawnedCashObjects = new List<GameObject>();

    private void Start()
    {
        GameObject registerObject = GameObject.FindWithTag("Register");
        if (registerObject == null)
            Debug.LogError("No GameObject with tag 'Register' found in the scene!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (RegisterController.IsOpen())
        {
            SpawnCash();
            PlayCashSound();
            totalCash += cashValue;

            if (cashCounterText != null)
            {
                cashCounterText.text = totalCash.ToString();
            }
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

        spawnedCashObjects.Add(spawnedCash);

        StartCoroutine(MoveInArc(spawnedCash, clickPoint, spawnPoint));

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
        pickupScript.SetValue(cashValue, isBill);
    }

    private IEnumerator MoveInArc(GameObject cashObject, Transform startPoint, Transform endPoint)
    {
        if (cashObject == null)
        {
            Debug.LogError("MoveInArc: Cash object is null!");
            yield break;
        }

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("MoveInArc: Start or End Point is not assigned!");
            yield break;
        }

        float duration = 0.5f; 
        float elapsedTime = 0f;

        Vector3 startPosition = startPoint.position;
        Vector3 endPosition = endPoint.position;
        Vector3 midPoint = (startPosition + endPosition) / 2 + Vector3.up * 1.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;


            Vector3 currentPos = Vector3.Lerp(Vector3.Lerp(startPosition, midPoint, t), Vector3.Lerp(midPoint, endPosition, t), t);

            if (cashObject != null)
            {
                cashObject.transform.position = currentPos;
            }
            else
            {
                Debug.LogError("MoveInArc: Cash object was destroyed during animation!");
                yield break;
            }

            yield return null;
        }
    }



    public static int GetTotalCash()
    {
        return totalCash;
    }

    public static void reduceTotalCash(int value)
    {
        totalCash -= value;
    }

    public void completeTransaction()
    {
        if (totalCash - ScreenController.GetChangeAmount() == 0)
        {
            Debug.Log("Successful Transaction Activity (REPORT IN DATABASE)");
        }
        else
        {
            Debug.Log("Failed Transaction Activity (USER ERROR TO REPORT IN DATABASE)");
        }
        DeleteAllSpawnedCash();
        totalCash = 0;
    }

    void DeleteAllSpawnedCash()
    {
        foreach (GameObject cash in spawnedCashObjects)
        {
            if (cash != null)
            {
                CashPickup cashPickup = cash.GetComponent<CashPickup>();
                if (cashPickup != null)
                {
                    cashPickup.OnPointerClick(null, true);
                }
                else
                {
                    Debug.LogWarning("Cash object has no CashPickup script: " + cash.name);
                    Destroy(cash);
                }
            }
        }

        spawnedCashObjects.Clear();
    }

    public void PlayCashSound()
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
            Debug.LogWarning("SoundFXManager instance is missing!");
        }
    }
}
