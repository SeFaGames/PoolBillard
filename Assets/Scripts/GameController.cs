using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class is used to handle the main game logic, like turns, gameover and scores.
/// Its associated with the QueueController, BallCollector, TurnDisplay, ScoreBoardManager.
/// </summary>
public class GameController : MonoBehaviour
{
    //referenced objects / scripts
    public MenuManager menuManager;                 //To open / close the menu
    public BallManager ballManager;                 //To respawn the balls
    public QueueController queueController;         //To suppress the queue
    public OverlayMessageManager onScreenMessage;   //To display a message on screen

    //Player scores
    public int scorePlayerOne { get; set; }
    public int scorePlayerTwo { get; set; }

    //public flags
    public bool playerOneTurn;      //true = Player 1; false = Player 2;
    public bool playerOneHasHalf;   //true = player 1 has half & player 2 full; false = player 1 has full & player 2 half
    public bool firstBallInGame;

    //private flags
    private bool turnover;  //whether or not a turn is active (balls rolling)
    private bool gameOver;  //whether or not the game is over
    private bool foul;      //whether or not a foul has been committed this turn
    private bool scored;    //whether or not a ball has been scored this turn

    //timers
    private float preGameTimer;     //Amount of time to wait before the game logic starts
    private float afterGameTimer;   //Amount of time to waut after the game is over and before the menu is opened

    void Start()
    {
        InitializeGlobals();
    }

    /// <summary>
    /// Handles Turns, fouls and gameover
    /// </summary>
    void Update()
    {
        //Wait a little while, cause balls are spawned slightly levitated. Wait until they have fallen down and are static
        if (preGameTimer > 0)
        {
            preGameTimer -= Time.deltaTime;
            return;
        }

        //No need to calculate logic if the game is over
        if (gameOver)
        {
            queueController.SetSuppressQueue(true);
            //display the menu after the timer is over
            if(afterGameTimer > 0)
            {
                afterGameTimer -= Time.deltaTime;
                return;
            }
            menuManager.OpenMenu();
        }

        //if all balls aren't moving, end turn
        if(IsStatic() && !turnover)
        {
            //Respawn queue ball if a foul was committed
            if (foul)
            {
                ballManager.RespawnQueue();
                ResetPreGameTimer();
            }
                

            turnover = true;
            queueController.UpdatePosition();
            queueController.SetSuppressQueue(false);

            //Allow second try if no foul was committed and a ball was scored
            if (scored && !foul)
            {
                scored = false;
                return;
            }

            foul = false;
            scored = false;

            //switches turns
            if (playerOneTurn)
            {
                Debug.Log("Turn over. Player Two is next!");
                onScreenMessage.DisplayMessage("Player Two's Turn", 2);
                playerOneTurn = false;
                return;
            }
            else
            {
                Debug.Log("Turn over. Player One is next!");
                onScreenMessage.DisplayMessage("Player One's Turn", 2);
                playerOneTurn = true;
                return;
            }
                
        }

        //If balls are moving while the turn is marked as over, it will be marked as active and the queue will be suppressed
        if (!IsStatic() && turnover)
        {
            turnover = false;
            queueController.SetSuppressQueue(true);
        }
    }

    /// <summary>
    /// Resets the game to the original state
    /// </summary>
    public void ResetGame()
    {
        ballManager.RespawnObjectives();
        ballManager.RespawnQueue();

        InitializeGlobals(); 

        menuManager.CloseMenu();
        queueController.UpdatePosition();
        queueController.SetSuppressQueue(false);
        queueController.SuppressFor(0.2f);  //Suppress the Queue to avoid instant queue bumping after new game button has been clicked
    }

    /// <summary>
    /// Sets globals to base value
    /// </summary>
    private void InitializeGlobals()
    {
        playerOneTurn = true; 
        playerOneHasHalf = true;
        ResetPreGameTimer();
        afterGameTimer = 4.0f;
        gameOver = false;
        playerOneTurn = true;
        scorePlayerOne = 0;
        scorePlayerTwo = 0;
        turnover = true;
        firstBallInGame = true;
        foul = false;
        scored = false;
    }

    /// <summary>
    /// Resets the pre game timer
    /// </summary>
    private void ResetPreGameTimer()
    {
        preGameTimer = 0.5f;
    }

    /// <summary>
    /// Closes the application
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    /// <summary>
    /// Is executed once a ball has been collected by the BallCollector.
    /// Determines the type of the balls and issues a gameover, foul or score
    /// </summary>
    /// <param name="marker">BallMarker of the collected ball</param>
    public void OnBallCollected(BallMarker marker)
    {
        //If the ball was the eight, check whether or not all previous balls where collected and determine a winner
        if (marker.isEight)
        {
            if((playerOneTurn && scorePlayerOne >= 7) || (!playerOneTurn && scorePlayerTwo < 7))
            {
                onScreenMessage.DisplayMessage("Player One wins.", 4);
                queueController.SetSuppressQueue(true);
                Debug.Log("Game Over. Player One wins");
            }
            else
            {
                onScreenMessage.DisplayMessage("Player Two wins.", 4);
                queueController.SetSuppressQueue(true);
                Debug.Log("Game Over. Player Two wins");
            }
            gameOver = true;
            return;
        }

        //if scored ball was the queue ball, its a foul.
        if (marker.isQueue)
        {
            Debug.Log("Foul");
            foul = true;
            return;
        }

        //First player to score a valid ball gets its color as objective
        if (firstBallInGame)
        {
            firstBallInGame = false;
            if((playerOneTurn && !marker.isFull) || (!playerOneTurn && marker.isFull))
            {
                playerOneHasHalf = true;
                onScreenMessage.DisplayMessage("Player One gets Half. \nPlayer Two gets Full.", 2);
                Debug.Log("Player One gets Half. Player Two gets Full.");
            }
            else
            {
                playerOneHasHalf = false;
                onScreenMessage.DisplayMessage("Player One gets Full. \nPlayer Two gets Half.", 2);
                Debug.Log("Player One gets Full. Player Two gets Half.");
            }
        }

        //Increment the score of the right player (dependent on ball color)
        if ((marker.isFull && playerOneHasHalf) || (!playerOneHasHalf && !marker.isFull))
        {
            scorePlayerTwo++;
            Debug.Log("Player Two scored.");

            if (!playerOneTurn)
            {
                scored = true;
            }
        }
        else
        {
            scorePlayerOne++;
            Debug.Log("Player One scored.");

            if (playerOneTurn)
            {
                scored = true;
            }
        }
    }

    /// <summary>
    /// Determines whether or not the balls are static.
    /// Calculates the total velocity of all balls.
    /// Velocity has to be zero to be static.
    /// </summary>
    /// <returns>whether or not the balls are static</returns>
    private bool IsStatic()
    {
        float totalVelo = 0.0f;
        foreach(BallMarker marker in ballManager.GetAllBalls())
        {
            Rigidbody rigid = marker.gameObject.GetComponent<Rigidbody>();
            if (rigid == null)
                continue;

            totalVelo += rigid.velocity.magnitude;
        }

        return totalVelo == 0;
    }
}
