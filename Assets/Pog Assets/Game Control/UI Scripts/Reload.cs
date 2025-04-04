using UnityEngine;

public class SwitchAndReturnOnClick : MonoBehaviour
{
    [SerializeField] private string sceneToGoTo;

    public void SaveCurrentAndSwitch()
    {
        if (ScenePersistenceManager.instance != null)
        {
            ScenePersistenceManager.instance.SaveAndSwitchScene(sceneToGoTo);
        }
    }

    public void ReloadPrevious()
    {
        if (ScenePersistenceManager.instance != null)
        {
            ScenePersistenceManager.instance.ReloadPreviousScene();
        }
    }
}
