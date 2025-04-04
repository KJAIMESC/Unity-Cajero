using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CustomerMovement : MonoBehaviour
{
    [Header("Locations")]
    public Transform registerTarget;
    public Transform exitTarget;
    public Transform spawnPoint;

    [Header("Connections")]
    public ScreenController screenController;
    [SerializeField] GameTimer timer;
    [SerializeField] private CustomerCashSpawner cashSpawner;

    [Header("Events")]
    [SerializeField] private UnityEvent onEnd;
    private int exitCount = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private bool atRegister = false;
    private bool hasUpdatedNumber = false;
    private bool isWalking = false;

    private int lastAvatarIndex = -1;

    private AudioSource footstepSource;

    public static int customerPaymentAmount;

    void Awake()
    {
        RandomizeAvatar();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Create an AudioSource attached to the customer for footsteps
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.clip = SoundFXManager.instance?.customerWalking;
        footstepSource.loop = true;
        footstepSource.volume = 1f;
        footstepSource.spatialBlend = 1f; // 3D sound
        footstepSource.playOnAwake = false;
    }

    public void moveToRegister()
    {
        if (!atRegister)
        {
            MoveTo(registerTarget);
            atRegister = true;
        }
    }

    void Update()
    {
        float targetSpeed = agent.velocity.magnitude;
        float currentSpeed = animator.GetFloat("Speed");
        float smoothedSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);
        animator.SetFloat("Speed", smoothedSpeed);

        HandleWalkingSound(targetSpeed);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            animator.SetFloat("Speed", 0);

            StopWalkingSound();

            if (atRegister && !hasUpdatedNumber)
            {
                if (screenController != null)
                {
                    screenController.UpdateRandomNumber();
                    screenController.ActivateScreen();
                    hasUpdatedNumber = true;
                    setPaymentAmount();
                    if (cashSpawner != null)
                    {
                        cashSpawner.SpawnCash(customerPaymentAmount);
                    }
                    else
                    {
                        Debug.LogWarning("cashSpawner not assigned on CustomerMovement.");
                    }
                }
                else
                {
                    Debug.LogWarning("ScreenController is not assigned!");
                }
            }

            if (Vector3.Distance(transform.position, exitTarget.position) < 2f)
            {
                exitCount++;
                if (exitCount > 4)
                {
                    if (onEnd != null)
                    {
                        onEnd.Invoke();
                    }

                    // Optional: Disable movement or animation after end
                    agent.isStopped = true;
                    animator.SetFloat("Speed", 0);
                    this.enabled = false; // stops Update() from running
                }
                else
                {
                    ResetCustomer();
                }
            }
        }
    }

    private void ResetCustomer()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned!");
            return;
        }
        agent.isStopped = true;
        agent.ResetPath();
        agent.Warp(spawnPoint.position);
        agent.isStopped = false;
        RandomizeAvatar();
        atRegister = false;
        hasUpdatedNumber = false;
        isWalking = false;
        animator.SetFloat("Speed", 0);
        moveToRegister();
        screenController.UpdateText("");
        timer.StartTimer();
    }


    public void MoveToExit()
    {
        if (exitTarget != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
            MoveTo(exitTarget);
        }
        else
        {
            Debug.LogWarning("Exit target is not set!");
        }
    }

    private void MoveTo(Transform target)
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            agent.isStopped = false;
        }
    }

    public void setPaymentAmount()
    {
        int step = 100;
        int baseAmount = ScreenController.GetPaymentAmount();
        int min = Mathf.Max(10000, baseAmount - Mathf.RoundToInt(baseAmount * 0.3f));

        int max;
        if (baseAmount <= 50000)
            max = baseAmount + 20000;
        else if (baseAmount <= 150000)
            max = baseAmount + 50000;
        else
            max = baseAmount + 200000;

        min = (min / step) * step;
        max = (max / step) * step;
        int rawAmount = Random.Range(min / step, max / step) * step;
        float roundChance = Random.value;
        if (roundChance < 0.25f)
        {
            rawAmount = Mathf.CeilToInt(rawAmount / 50000f) * 50000;
        }
        else if (roundChance < 0.35f)
        {
            rawAmount = Mathf.CeilToInt(rawAmount / 100000f) * 100000;
        }
        customerPaymentAmount = rawAmount;
    }

    public static int GetPaymentAmount()
    {
        return customerPaymentAmount;
    }

    private void HandleWalkingSound(float speed)
    {
        if (speed > 0.1f && !isWalking)
        {
            StartWalkingSound();
        }
        else if (speed <= 0.1f && isWalking)
        {
            StopWalkingSound();
        }
    }

    private void StartWalkingSound()
    {
        if (footstepSource != null && !footstepSource.isPlaying)
        {
            footstepSource.Play();
            isWalking = true;
        }
    }

    private void StopWalkingSound()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
        isWalking = false;
    }

 private void RandomizeAvatar()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, childCount-1);
        } while (randomIndex == lastAvatarIndex && childCount > 1);

        transform.GetChild(randomIndex).gameObject.SetActive(true);
        lastAvatarIndex = randomIndex;
    }
}
