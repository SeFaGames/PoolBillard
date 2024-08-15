using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is used to display messages for a short amount of time in the middle of the screen.
/// Associated with GameController.
/// </summary>
public class OverlayMessageManager : MonoBehaviour
{
    public TMP_Text text;           //to update the text

    private float timer = 0;        
    private string message = null;  

    /// <summary>
    /// Activates the text component if the timer greater than 0
    /// </summary>
    void Update()
    {
        if (message == null)
            return;

        //If time is over, hide message
        if (timer <= 0)
        {
            this.message = null;
            timer = 0;
            text.gameObject.SetActive(false);
            return;
        }

        //If time is not over, show message
        text.gameObject.SetActive(true);
        text.text = message;
        timer -= Time.deltaTime;    //And reduce timer
    }

    /// <summary>
    /// Displays a Text Message in the center of the users screen for a specified duration
    /// </summary>
    /// <param name="message">message content</param>
    /// <param name="seconds">duration in seconds</param>
    public void DisplayMessage(string message, float seconds)
    {
        Debug.Log("Displaying " + message + " for " + seconds + " s");
        this.timer = seconds;
        this.message = message;
    }
}
