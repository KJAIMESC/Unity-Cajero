using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 offset;
    private float fixedY;
    private ObjectSpawner spawner;
    private bool isPlaced = false;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        spawner = FindFirstObjectByType<ObjectSpawner>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void OnMouseDown()
    {
        if (isPlaced) return;

        isDragging = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        Vector3 newPos = transform.position + Vector3.up * 0.3f;
        rb.MovePosition(newPos);

        offset = newPos - GetMouseWorldPosition();
        fixedY = newPos.y;
    }

    void OnMouseUp()
    {
        StopDragging();
    }

    void Update()
    {
        if (isDragging && !IsMouseInsideScreen())
        {
            StopDragging();
        }
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector3 targetPos = GetMouseWorldPosition() + offset;
            targetPos.y = fixedY;
            rb.MovePosition(Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * 10));
        }
    }

    void StopDragging()
    {
        isDragging = false;
        rb.useGravity = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPlaced) return;
        isPlaced = true;

        if (other.CompareTag("Correct"))
        {
            rb.isKinematic = true;
            spawner.RecordAttempt(true);
            spawner.IncrementCorrectCount();

            if (SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySound(SoundFXManager.instance.correctPlacement, transform.position, 1f);
            }

            Invoke(nameof(SpawnNewObject), 0.5f);
        }
        else if (other.CompareTag("Incorrect"))
        {
            rb.isKinematic = true;
            spawner.RecordAttempt(false);
            spawner.UpdateCounter();

            if (SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySound(SoundFXManager.instance.incorrectPlacement, transform.position, 1f);
            }

            Invoke(nameof(SpawnNewObject), 0.5f);
        }
    }


    void SpawnNewObject()
    {
        spawner.ResetScene();
        Destroy(gameObject);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;

        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    bool IsMouseInsideScreen()
    {
        return Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width &&
               Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height;
    }
}
