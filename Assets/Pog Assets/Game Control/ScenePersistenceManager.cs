using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ScenePersistenceManager : MonoBehaviour
{
    public static ScenePersistenceManager instance;

    private string previousScene;
    private GameObject[] previousRootObjects;

    private Dictionary<GameObject, bool> originalActiveStates = new();
    private Dictionary<GameObject, Vector3> originalPositions = new();
    private Dictionary<Animator, Dictionary<string, bool>> savedAnimatorTriggers = new();

    private Vector3 hiddenPosition = new Vector3(10000, 10000, 10000);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveAndSwitchScene(string newSceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        previousScene = currentScene.name;
        previousRootObjects = currentScene.GetRootGameObjects();

        originalActiveStates.Clear();
        originalPositions.Clear();
        savedAnimatorTriggers.Clear();

        foreach (GameObject rootObj in previousRootObjects)
        {
            if (rootObj == null) continue;

            originalActiveStates[rootObj] = rootObj.activeSelf;
            originalPositions[rootObj] = rootObj.transform.position;

            Animator animator = rootObj.GetComponentInChildren<Animator>();
            if (animator)
            {
                SaveAnimatorTriggers(animator);
            }
            else
            {
                rootObj.SetActive(false); // Disable only non-animated objects
            }

            rootObj.transform.position = hiddenPosition;
        }

        Time.timeScale = 0;

        SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive).completed += (op) =>
        {
            Scene newScene = SceneManager.GetSceneByName(newSceneName);
            if (newScene.IsValid())
            {
                SceneManager.SetActiveScene(newScene);
                EnsureOnlyOneAudioListener();
            }
        };
    }

    public void ReloadPreviousScene()
    {
        if (string.IsNullOrEmpty(previousScene)) return;

        string currentScene = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(currentScene).completed += (op) =>
        {
            foreach (GameObject obj in previousRootObjects)
            {
                if (obj == null) continue;

                if (originalPositions.TryGetValue(obj, out Vector3 originalPos))
                {
                    obj.transform.position = originalPos;
                }

                if (originalActiveStates.TryGetValue(obj, out bool wasActive))
                {
                    obj.SetActive(wasActive);
                }

                Animator animator = obj.GetComponentInChildren<Animator>();
                if (animator && savedAnimatorTriggers.TryGetValue(animator, out var triggerDict))
                {
                    RestoreAnimatorTriggers(animator, triggerDict);
                }
            }

            Time.timeScale = 1;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(previousScene));
            EnsureOnlyOneAudioListener();
        };
    }

    private void SaveAnimatorTriggers(Animator animator)
    {
        var triggers = new Dictionary<string, bool>();
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                // Can't query trigger state directly, so assume fresh state
                triggers[param.name] = false;
            }
        }
        savedAnimatorTriggers[animator] = triggers;
    }

    private void RestoreAnimatorTriggers(Animator animator, Dictionary<string, bool> triggers)
    {
        foreach (var trigger in triggers)
        {
            if (trigger.Value)
            {
                animator.SetTrigger(trigger.Key);
            }
        }
    }

    private void EnsureOnlyOneAudioListener()
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        bool enabledOne = false;

        foreach (var listener in listeners)
        {
            if (!enabledOne)
            {
                listener.enabled = true;
                enabledOne = true;
            }
            else
            {
                listener.enabled = false;
            }
        }
    }
}
