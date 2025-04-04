using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerCashSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CashPrefab
    {
        public GameObject prefab;
        public int value;
        public bool isBill;
        public Transform spawnPoint;
    }

    [Header("Cash Settings")]
    public List<CashPrefab> cashPrefabs;
    public Transform commonSpawnPoint;
    public float maxRotationOffset = 25f;
    public float delayBetweenSpawns = 0.15f;
    public float arcHeight = 1.5f;
    [Range(0f, 1f)]
    public float randomness = 0.3f;
    public bool isGreedy = true;
    public bool isSloppy = false;

    private List<GameObject> spawnedCashObjects = new();

    public void SpawnCash(int totalAmount)
    {
        ClearSpawnedCash();
        StartCoroutine(SpawnCashWithArc(totalAmount));
    }

    private IEnumerator SpawnCashWithArc(int amount)
    {
        List<CashPrefab> breakdown = GetRandomGreedyBreakdown(amount);
        if (breakdown == null)
        {
            Debug.LogWarning("Could not break down the amount: " + amount);
            yield break;
        }

        foreach (CashPrefab cash in breakdown)
        {
            if (commonSpawnPoint == null)
            {
                Debug.LogWarning("Common spawn point is not set!");
                continue;
            }

            Transform targetSpawn = cash.spawnPoint != null ? cash.spawnPoint : commonSpawnPoint;

            Vector3 spawnOffset = Vector3.zero;

            if (!isGreedy)
            {
                float offsetRange = cash.isBill ? 0.3f : 0.5f;
                if (isSloppy && !cash.isBill)
                {
                    offsetRange *= 2f;
                }

                spawnOffset = new Vector3(
                    Random.Range(-offsetRange, offsetRange),
                    0f,
                    Random.Range(-offsetRange, offsetRange)
                );
            }

            Vector3 spawnPos = commonSpawnPoint.position;
            Vector3 targetPos = targetSpawn.position + spawnOffset;

            GameObject obj = Instantiate(cash.prefab, spawnPos, Quaternion.identity);
            spawnedCashObjects.Add(obj);

            // Initial Rotation
            Quaternion rotation = cash.isBill
                ? Quaternion.Euler(-90f, Random.Range(-10f, 10f), 0f)
                : Quaternion.Euler(0f, 0f, Random.Range(-maxRotationOffset, maxRotationOffset));
            obj.transform.rotation = rotation;

            // Setup Rigidbody and Collider
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = obj.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            rb.isKinematic = true;

            if (obj.GetComponent<Collider>() == null)
            {
                obj.AddComponent<BoxCollider>();
            }

            StartCoroutine(MoveInArc(obj, spawnPos, targetPos, rb));

            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        Debug.Log($"Spawned {spawnedCashObjects.Count} cash objects totaling {amount}");
    }

    private IEnumerator MoveInArc(GameObject obj, Vector3 start, Vector3 end, Rigidbody rb)
    {
        float elapsedTime = 0f;
        float arcDuration = 0.7f;
        Vector3 peak = (start + end) / 2 + Vector3.up * arcHeight;

        while (elapsedTime < arcDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / arcDuration;

            Vector3 pos = Vector3.Lerp(
                Vector3.Lerp(start, peak, t),
                Vector3.Lerp(peak, end, t),
                t
            );

            if (obj != null)
                obj.transform.position = pos;
            else
                yield break;

            yield return null;
        }

        if (obj != null)
        {
            obj.transform.position = end;
            if (rb != null)
                rb.isKinematic = false;
        }
    }

    private List<CashPrefab> GetRandomGreedyBreakdown(int amount)
    {
        List<CashPrefab> result = new();
        List<CashPrefab> sorted = new(cashPrefabs);
        sorted.Sort((a, b) => b.value.CompareTo(a.value));

        int remaining = amount;

        while (remaining > 0)
        {
            List<CashPrefab> options = sorted.FindAll(c => c.value <= remaining);
            if (options.Count == 0)
                break;

            CashPrefab selected = Random.value < randomness
                ? options[Random.Range(0, options.Count)]
                : options[0];

            result.Add(selected);
            remaining -= selected.value;
        }

        return remaining == 0 ? result : null;
    }

    public void ClearSpawnedCash()
    {
        foreach (GameObject obj in spawnedCashObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedCashObjects.Clear();
    }
}
