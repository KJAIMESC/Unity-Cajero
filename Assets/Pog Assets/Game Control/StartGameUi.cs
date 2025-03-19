using UnityEngine;

public class StartGameUI : MonoBehaviour
{
    public GameObject startMenuCanvas;
    public CustomerMovement customer;
    public GameTimer gameTimer;

    public void StartGame()
    {
        Time.timeScale = 1;

        if (gameTimer != null)
        {
            gameTimer.StartTimer();
        }
        else
        {
            Debug.LogError("GameTimer reference is missing in StartGameUI!");
        }

        if (customer != null)
        {
            customer.moveToRegister();
        }
        else
        {
            Debug.LogError("Customer reference is missing in StartGameUI!");
        }
    }
}
