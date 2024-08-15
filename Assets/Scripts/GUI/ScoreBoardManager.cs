using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is used to handle the scoreboard.
/// Opens Scoreboard if TAB is pressed
/// </summary>
public class ScoreBoardManager : MonoBehaviour
{
    public MenuManager menuManager; //To suppress scoreboard opening while menu is open
    public Canvas canvas;           //Canvas Component which displays the scoreboard
    public GameController gameController;
    public TMP_Text scoreDisplayPlayerOne;
    public TMP_Text scoreDisplayPlayerTwo;

    /// <summary>
    /// Initialize the scoreboard as inactive
    /// </summary>
    private void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show Scoreboard if Tab is held down and menu is not open
    /// </summary>
    void Update()
    {
        if(Input.GetKey(KeyCode.Tab) && !menuManager.menuOpen)
        {
            string p1BallColor = "Half";
            string p2BallColor = "Full";
            if (!gameController.playerOneHasHalf)
            {
                p1BallColor = "Full";
                p2BallColor = "Half";
            }  
            if (gameController.firstBallInGame)
                p1BallColor = p2BallColor = "Undecided";
                
            scoreDisplayPlayerOne.text = gameController.scorePlayerOne + " [" + p1BallColor + "]";
            scoreDisplayPlayerTwo.text = gameController.scorePlayerTwo + " [" + p2BallColor + "]";
            canvas.gameObject.SetActive(true);
        }
        else
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
