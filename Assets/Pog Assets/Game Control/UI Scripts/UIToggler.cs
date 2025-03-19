using UnityEngine;

public class UIToggler : MonoBehaviour
{
    [SerializeField] private bool startOn = false;

    void Awake()
    {
        gameObject.SetActive(startOn);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
