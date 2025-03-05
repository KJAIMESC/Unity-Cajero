using UnityEngine;
using UnityEngine.UI;

public class TransactionCanvasController : MonoBehaviour
{
    public GameObject enoughQuestionsText; // Assign the text that should disappear
    public GameObject yesButton; // Assign the Yes button
    public GameObject confirmButton; // Assign the Confirm button (should start hidden)

    void Start()
    {
        if (confirmButton != null)
        {
            confirmButton.SetActive(false); // Ensure Confirm button is hidden at start
        }
    }

    public void OnYesButtonClicked()
    {
        Debug.Log("✅ Yes button clicked. Hiding UI elements and showing Confirm button.");

        if (enoughQuestionsText != null)
        {
            enoughQuestionsText.SetActive(false); // Hide text
        }

        if (yesButton != null)
        {
            yesButton.SetActive(false); // Hide Yes button
        }

        if (confirmButton != null)
        {
            confirmButton.SetActive(true); // Show Confirm button
        }
    }
}
