using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private AudioListener audioListener;

    [SerializeField] private bool startActive = true;

    void Awake()
    {
        cam = GetComponent<Camera>();
        audioListener = GetComponent<AudioListener>();

        if (cam == null)
        {
            Debug.LogError("CameraController is not attached to a Camera. Disabling script.");
            enabled = false;
            return;
        }

        cam.gameObject.SetActive(startActive);
        HandleAudioListener();
    }

    public void Start()
    {
        HandleAudioListener();
    }

    public void EnableCamera()
    {
        if (cam != null && !cam.gameObject.activeSelf)
        {
            cam.gameObject.SetActive(true);
            cam.depth = -1;
            cam.clearFlags = CameraClearFlags.Depth;
            cam.targetDisplay = 1; 

            HandleAudioListener();
        }
    }

    public void DisableCamera()
    {
        if (cam != null && cam.gameObject.activeSelf)
        {
            cam.gameObject.SetActive(false);
            HandleAudioListener();
        }
    }

    public void SwitchToCamera()
    {
        if (cam == null)
        {
            Debug.LogError("SwitchToCamera failed: Camera component not found.");
            return;
        }

        Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        bool cameraActivated = false;

        foreach (Camera c in allCameras)
        {
            if (c == cam)
            {
                c.gameObject.SetActive(true);
                c.depth = 1;
                c.clearFlags = CameraClearFlags.Skybox;
                c.targetDisplay = 0;
                cameraActivated = true;
            }
            else
            {
                c.gameObject.SetActive(false);
            }
        }

        if (!cameraActivated)
        {
            Debug.LogError("No cameras were activated. Check camera settings.");
        }

        HandleAudioListener();
    }

    
    private void HandleAudioListener()
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        foreach (AudioListener listener in listeners)
        {
            listener.enabled = false;
        }

        if (audioListener == null)
        {
            Debug.LogWarning($"CameraController: No AudioListener found on {cam.name}. Adding one.");
            audioListener = cam.gameObject.GetComponent<AudioListener>();

            if (audioListener == null)
            {
                audioListener = cam.gameObject.AddComponent<AudioListener>();
            }
        }

        if (cam.gameObject.activeSelf)
        {
            audioListener.enabled = true;
        }
    }

}