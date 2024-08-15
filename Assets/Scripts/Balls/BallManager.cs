using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Used to (re)spawn and retrieve balls.
/// Associated with GameController, QueueController.
/// </summary>
public class BallManager : MonoBehaviour
{
    public GameObject[] ballPrefabs;        // Array of the 15 Ball-Prefabs for instatitation
    public GameObject queueBallPrefab;      // Prefab of the queue ball for instantiation
    public GameObject queueBallsParent;     // GameObject storing the queue ball to allow for easy removal
    public GameObject objectiveBallParent;  // GameObject storing the objective balls to allow for easy removal
    public float ballDiameter = 0.0572f;    // diameter of the balls, to calculate placement 
    public float queueXPos = -0.7f;         // X Axis offset of the queue ball
    public float xObjOffset = 0.78f;        // X Axis offset of the objective valls
    public bool respawnOnStart = false;     // force respawn of balls on game start

    //Button to allow respawn of objective balls in editor
    [InspectorButton("RespawnObjectives")]
    public bool respawnObjectives;

    //Button to allow respawn of queue ball in editor
    [InspectorButton("RespawnQueue")]
    public bool respawnQueue;

    /// <summary>
    /// Respawn balls if respawn on start is enabled
    /// </summary>
    void Start()
    {
        if (respawnOnStart)
        {
            ballDiameter = queueBallPrefab.transform.localScale.x;
            RespawnQueue();
            RespawnObjectives();
        }
    }

    /// <summary>
    /// Destroys all existing queue balls and respawns a new one
    /// </summary>
    public void RespawnQueue()
    {
        foreach(BallMarker cue in GetAllQueueBalls())
        {
            Destroy(cue.transform.gameObject);
        }

        GameObject newCue = Instantiate(queueBallPrefab, new Vector3(queueXPos, 0.7925f, 0), new Quaternion(0, 0, 0, 0), queueBallsParent.transform);
        newCue.name = "CueBall";
    }

    /// <summary>
    /// Destroys all existing objective balls and respawns new ones
    /// </summary>
    public void RespawnObjectives()
    {
        Vector3 startPosition = new Vector3(0, 0, 0);

        foreach (BallMarker obj in GetAllObjectiveBalls())
        {
            DestroyImmediate(obj.transform.gameObject);
        }

        int ballIndex = 0;
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col <= row; col++)
            {
                if (ballIndex >= ballPrefabs.Length)
                    return;

                // Berechne die Position des Balls
                Vector3 position = startPosition +
                    new Vector3(xObjOffset + row * ballDiameter * Mathf.Sqrt(3) / 2, 0.7925f, col * ballDiameter - row * ballDiameter / 2);

                // Erzeuge den Ball
                Instantiate(ballPrefabs[ballIndex], position, new Quaternion(180, 180, 0, 0), objectiveBallParent.transform);

                ballIndex++;
            }
        }
    }

    /// <summary>
    /// Returns a Array containing all balls (objective and queue)
    /// </summary>
    /// <returns>BallMarker Array of all Balls</returns>
    public BallMarker[] GetAllBalls()
    {
        List<BallMarker> list = new List<BallMarker>(GetAllObjectiveBalls());
        foreach(BallMarker marker in GetAllQueueBalls())
        {
            list.Add(marker);
        }
        return list.ToArray();
    }

    /// <summary>
    /// Returns a Array containing all objective balls
    /// </summary>
    /// <returns>BallMarker Array of all objective balls</returns>
    public BallMarker[] GetAllObjectiveBalls()
    {
        return objectiveBallParent.GetComponentsInChildren<BallMarker>();
    }

    /// <summary>
    /// Returns a Array containing all queue balls
    /// </summary>
    /// <returns>BallMarker Array of all queue balls</returns>
    public BallMarker[] GetAllQueueBalls()
    {
        return queueBallsParent.GetComponentsInChildren<BallMarker>();
    }
}
