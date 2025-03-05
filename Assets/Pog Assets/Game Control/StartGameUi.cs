using UnityEngine;

public class StartGameUI : MonoBehaviour
{
    public GameObject startMenuCanvas;
    public CustomerMovement customer;
    public GameTimer gameTimer;

    public void StartGame()
    {
        Debug.Log("StartGame() called!");

        startMenuCanvas.SetActive(false);
        Time.timeScale = 1;

        if (gameTimer != null)
        {
            Debug.Log("Starting the game timer...");
            gameTimer.StartTimer();
        }
        else
        {
            Debug.LogError("GameTimer reference is missing in StartGameUI!");
        }

        if (customer != null)
        {
            Debug.Log("Customer found, calling moveToRegister()...");
            customer.moveToRegister();
        }
        else
        {
            Debug.LogError("Customer reference is missing in StartGameUI!");
        }
    }
}
