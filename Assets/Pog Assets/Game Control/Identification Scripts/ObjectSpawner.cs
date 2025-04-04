using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn;
    public GameObject[] dropZones;
    public Transform[] dropZoneSpawnPoints;

    public Transform spawnPoint;
    public TMPro.TextMeshProUGUI counterText;
    public TMPro.TextMeshProUGUI scoreText;
    private int correctlyPlacedObjects = 0;
    private int totalSpawnedObjects = 0;
    private bool looped = false;

    private List<(int index, bool isBill)> availableIndexes = new List<(int, bool)>();

    [System.Serializable]
    public class ObjectStats
    {
        public int errors = 0;
        public float totalTimeSpent = 0f;

        [System.NonSerialized]
        public int internalAttempts = 0;

        public float AverageTime => internalAttempts > 0 ? totalTimeSpent / internalAttempts : 0f;
    }

    private Dictionary<string, ObjectStats> objectStatsDict = new Dictionary<string, ObjectStats>();
    private float objectStartTime = 0f;
    private string currentObjectName = "";

    [Header("Events")]
    public UnityEvent onFinished;

    private Dictionary<string, string> nombreSimplificado = new Dictionary<string, string>()
    {
        { "2k Bill Blurred", "2 mil" },
        { "5k Bill Blurred", "5 mil" },
        { "10k Bill Blurred", "10 mil" },
        { "20k Bill Blurred", "20 mil" },
        { "50k Bill Blurred", "50 mil" },
        { "100k Bill Blurred", "100 mil" },
        { "100 Coin Blurred", "100" },
        { "200 Coin Blurred", "200" },
        { "500 Coin Blurred", "500" },
        { "1000 Coin Blurred", "1000" }
    };

    private void Start(){}

    public void Restart()
    {
        looped = false;
        totalSpawnedObjects = 0;
        correctlyPlacedObjects = 0;
        objectStatsDict.Clear();
        UpdateCounter();
        Enable();
    }

    public void Enable()
    {
        ResetAvailableIndexes();
        SpawnRandomObject();
    }

    private void ResetAvailableIndexes()
    {
        availableIndexes.Clear();
        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            bool isBill = (i < 6);
            availableIndexes.Add((i, isBill));
        }
    }

    public void SpawnRandomObject()
    {
        if (availableIndexes.Count == 0 && looped)
        {
            PrintSummary();
            return;
        }
        else if (availableIndexes.Count == 0)
        {
            looped = true;
            ResetAvailableIndexes();
        }

        int randomIndex = Random.Range(0, availableIndexes.Count);
        var (objectIndex, isBill) = availableIndexes[randomIndex];
        availableIndexes.RemoveAt(randomIndex);

        GameObject newObject = Instantiate(objectsToSpawn[objectIndex], spawnPoint.position, Quaternion.identity);
        newObject.tag = "Draggable";

        Collider col = newObject.GetComponent<Collider>() ?? newObject.AddComponent<BoxCollider>();

        if (isBill)
        {
            newObject.transform.localScale *= 6.5f;
            newObject.transform.rotation = Quaternion.Euler(-90, 180, 90);
        }
        else
        {
            newObject.transform.localScale *= 8f;
            newObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        Rigidbody rb = newObject.GetComponent<Rigidbody>();
        if (rb == null) rb = newObject.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (newObject.GetComponent<DraggableObject>() == null)
        {
            newObject.AddComponent<DraggableObject>();
        }

        if (SoundFXManager.instance != null)
        {
            if (isBill)
            {
                SoundFXManager.instance.PlaySound(SoundFXManager.instance.Cash, spawnPoint.position, 1f);
            }
            else
            {
                SoundFXManager.instance.PlaySound(SoundFXManager.instance.Coin, spawnPoint.position, 1f);
            }
        }

        totalSpawnedObjects++;

        // Start tracking
        string rawName = newObject.name.Replace("(Clone)", "").Trim();
        currentObjectName = nombreSimplificado.ContainsKey(rawName) ? nombreSimplificado[rawName] : rawName;
        objectStartTime = Time.time;

        if (!objectStatsDict.ContainsKey(currentObjectName))
        {
            objectStatsDict[currentObjectName] = new ObjectStats();
        }

        SpawnDropZones(objectIndex);
    }

    public void RecordAttempt(bool wasCorrect)
    {
        if (!objectStatsDict.ContainsKey(currentObjectName)) return;

        float timeTaken = Time.time - objectStartTime;
        ObjectStats stats = objectStatsDict[currentObjectName];

        stats.internalAttempts++;
        if (!wasCorrect) stats.errors++;
        stats.totalTimeSpent += timeTaken;

        objectStatsDict[currentObjectName] = stats;
    }

    public void UpdateCounter()
    {
        if (counterText != null)
        {
            counterText.text = $"{20 - totalSpawnedObjects}";
        }
    }

    public void IncrementCorrectCount()
    {
        correctlyPlacedObjects++;
        UpdateCounter();
    }

    void SpawnDropZones(int matchingIndex)
    {
        if (dropZones.Length == 0 || dropZoneSpawnPoints.Length < 4) return;

        List<Transform> availableDropPoints = new List<Transform>(dropZoneSpawnPoints);

        Transform firstDropPoint = availableDropPoints[Random.Range(0, availableDropPoints.Count)];
        InstantiateDropZone(matchingIndex, firstDropPoint, true);
        availableDropPoints.Remove(firstDropPoint);

        List<int> remainingIndexes = new List<int>();
        for (int i = 0; i < dropZones.Length; i++)
        {
            if (i != matchingIndex) remainingIndexes.Add(i);
        }

        ShuffleList(remainingIndexes);

        for (int i = 0; i < 3; i++)
        {
            Transform randomDropPoint = availableDropPoints[Random.Range(0, availableDropPoints.Count)];
            int randomDropIndex = remainingIndexes[i];

            InstantiateDropZone(randomDropIndex, randomDropPoint);
            availableDropPoints.Remove(randomDropPoint);
        }
    }

    void InstantiateDropZone(int index, Transform spawnPoint, bool isCorrect = false)
    {
        GameObject dropZone = Instantiate(dropZones[index], spawnPoint.position, spawnPoint.rotation);
        dropZone.transform.localScale *= 1.05f;

        Rigidbody rb = dropZone.GetComponent<Rigidbody>() ?? dropZone.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        dropZone.tag = isCorrect ? "Correct" : "Incorrect";

        Renderer renderer = dropZone.GetComponent<Renderer>();
        if (renderer != null && isCorrect)
        {
            renderer.material.color = Color.green;
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    public void ResetScene()
    {
        foreach (GameObject dropZone in GameObject.FindGameObjectsWithTag("Correct"))
        {
            Destroy(dropZone);
        }
        foreach (GameObject dropZone in GameObject.FindGameObjectsWithTag("Incorrect"))
        {
            Destroy(dropZone);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Draggable"))
        {
            Destroy(obj);
        }

        SpawnRandomObject();
    }

    void PrintSummary()
    {
        

        string[] orderedNames = new string[]
        {
            "100", "200", "500", "1000",
            "2 mil", "5 mil", "10 mil", "20 mil", "50 mil", "100 mil"
        };

        float totalSessionTime = 0f;
        int totalErrors = 0;

        Dictionary<string, object> finalStats = new();

        foreach (string name in orderedNames)
        {
            if (objectStatsDict.ContainsKey(name))
            {
                var stats = objectStatsDict[name];
                totalSessionTime += stats.totalTimeSpent;
                totalErrors += stats.errors;

                finalStats[name] = new
                {
                    errors = stats.errors,
                    averageTime = stats.AverageTime
                };
            }
        }

        int correct = 20 - totalErrors;

        string feedback = correct switch
        {
            20 => "¡Increible! Identificaste correctamente los 20 billetes y monedas.",
            >= 18 => $"¡Excelente! Identificaste {correct} de 20 billetes y monedas.",
            >= 14 => $"¡Muy bien! Identificaste {correct} de 20 billetes y monedas.",
            >= 10 => $"Identificaste {correct} de 20 billetes y monedas. Vas por buen camino.",
            _ => $"Identificaste {correct} de 20 billetes y monedas. ¡Sigue practicando, lo estas haciendo bien!"
        };

        if (scoreText != null)
        {
            scoreText.text = feedback;
        }

        var jsonOutput = new
        {
            totalSessionTime = totalSessionTime,
            totalErrors = totalErrors,
            performance = finalStats
        };

        string json = JsonConvert.SerializeObject(jsonOutput, Formatting.Indented);
        Debug.Log("Activity Summary:\n" + json);
        onFinished?.Invoke();
    }
}