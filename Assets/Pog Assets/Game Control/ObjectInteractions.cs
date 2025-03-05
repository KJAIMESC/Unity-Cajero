using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectInteraction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Register;
    public Camera MainCamera;
    public Camera TransactionCamera;
    private bool isOpen = false;

    private void Start()
    {
        if (Register == null)
        {
            Register = GameObject.FindWithTag("Register");
        }

        if (MainCamera == null)
        {
            MainCamera = Camera.main;
        }

        if (TransactionCamera == null)
        {
            TransactionCamera = GameObject.FindWithTag("2nd Camera").GetComponent<Camera>();
        }

        if (MainCamera != null && TransactionCamera != null)
        {
            MainCamera.gameObject.SetActive(true);
            TransactionCamera.gameObject.SetActive(false);
        }
    }

    public void ExecuteTrigger(string trigger)
    {
        Debug.Log("Trigger: " + trigger);
        if (Register != null)
        {
            var animator = Register.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(trigger);
            }
        }
        else
        {
            Debug.LogError("Register GameObject not found!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + gameObject.name);

        if (isOpen)
        {
            ExecuteTrigger("TrClose");
            SwitchCamera(MainCamera, TransactionCamera);
        }
        else
        {
            ExecuteTrigger("TrOpen");
            SwitchCamera(TransactionCamera, MainCamera);
        }

        isOpen = !isOpen;
    }

    private void SwitchCamera(Camera activeCam, Camera inactiveCam)
    {
        if (activeCam != null && inactiveCam != null)
        {
            activeCam.gameObject.SetActive(true);
            inactiveCam.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Enter: " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exit: " + gameObject.name);
    }

    public bool IsRegisterOpen()
    {
        return isOpen;
    }

}
