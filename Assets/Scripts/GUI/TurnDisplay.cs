using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is used to display the current turn player
/// </summary>
public class TurnDisplay : MonoBehaviour
{
    public TMP_Text textComponent;          //Text Component to display the current turn
    public GameController gameController;   //To get current turn  

    //Labels
    private const string playerOneLabel = "Player 1's Turn";
    private const string playerTwoLabel = "Player 2's Turn";

    /// <summary>
    /// Initialize Label to playerOneLabel
    /// </summary>
    void Start()
    {
        textComponent.text = playerOneLabel;
    }

    /// <summary>
    /// Retrieves current turn from gameController and displays label based on current turn
    /// </summary>
    void Update()
    {
        if (gameController.playerOneTurn)
        {
            textComponent.text = playerOneLabel;
        }
        else {
            textComponent.text = playerTwoLabel;
        }
    }
}
