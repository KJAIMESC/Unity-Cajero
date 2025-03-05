using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float elapsedTime = 0f;
    private bool isRunning = false;

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
        timerText.gameObject.SetActive(true);
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Timer stopped. Final time: {FormatTime(elapsedTime)}");
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        timerText.text = FormatTime(elapsedTime);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}
