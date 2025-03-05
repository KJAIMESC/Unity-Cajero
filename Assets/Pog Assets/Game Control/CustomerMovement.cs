using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class CustomerMovement : MonoBehaviour
{
    public Transform registerTarget;
    public Transform exitTarget;
    public ScreenController screenController;

    private NavMeshAgent agent;
    private Animator animator;
    private bool atRegister = false;
    private bool hasUpdatedNumber = false;

    public static int customerPaymentAmount;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            animator.SetFloat("Speed", 0);

            if (atRegister && !hasUpdatedNumber)
            {
                if (screenController != null)
                {
                    screenController.UpdateRandomNumber();
                    screenController.ActivateScreen();
                    hasUpdatedNumber = true;
                    setPaymentAmount();
                }
                else
                {
                    Debug.LogWarning("⚠ ScreenController is not assigned!");
                }
            }
        }
    }

    public void MoveToExit()
    {
        Debug.Log("Confirm button clicked! Attempting to move customer.");

        if (exitTarget != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
            agent.SetDestination(exitTarget.position);
            Debug.Log("Moving to exit at: " + exitTarget.position);
        }
        else
        {
            Debug.LogWarning("⚠ Exit target is not set!");
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
        int min = ScreenController.GetPaymentAmount();
        int max = 1200000;
        int step = 100;
        customerPaymentAmount = Random.Range(min / step, max / step) * step;
    }

    public static int GetPaymentAmount()
    {
        return customerPaymentAmount;
    }
}
