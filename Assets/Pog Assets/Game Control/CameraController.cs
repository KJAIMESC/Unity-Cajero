using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public Camera transactionCamera;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (transactionCamera == null)
        {
            transactionCamera = GameObject.FindWithTag("2nd Camera")?.GetComponent<Camera>();
        }

        if (mainCamera != null && transactionCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            transactionCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("One or both cameras are not assigned in CameraController!");
        }
    }

    public void SwitchCameras()
    {
        if (mainCamera != null && transactionCamera != null)
        {
            bool isTransactionActive = transactionCamera.gameObject.activeSelf;
            mainCamera.gameObject.SetActive(isTransactionActive);
            transactionCamera.gameObject.SetActive(!isTransactionActive);
        }
        else
        {
            Debug.LogWarning("Cameras are not assigned in CameraController!");
        }
    }
}
