using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(PlaySoundThenLoad(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void GoToScene(string sceneName)
    {
        StartCoroutine(PlaySoundThenLoad(sceneName));
    }

    private IEnumerator PlaySoundThenLoad(string sceneName)
    {
        if (SoundFXManager.instance != null && SoundFXManager.instance.buttonClick != null)
        {
            AudioSource audioSource = SoundFXManager.instance.PlayPersistentSound(SoundFXManager.instance.buttonClick, Vector3.zero, 1f);
            yield return new WaitForSeconds(SoundFXManager.instance.buttonClick.length * 0.02f);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator PlaySoundThenLoad(int sceneIndex)
    {
        if (SoundFXManager.instance != null && SoundFXManager.instance.buttonClick != null)
        {
            AudioSource audioSource = SoundFXManager.instance.PlayPersistentSound(SoundFXManager.instance.buttonClick, Vector3.zero, 1f);
            yield return new WaitForSeconds(SoundFXManager.instance.buttonClick.length * 0.02f);
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
