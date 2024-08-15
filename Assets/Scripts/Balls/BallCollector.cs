using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to detect balls that collide with the game objects collider and call the OnBallCollected Method of the gameController
/// </summary>
public class BallCollector : MonoBehaviour
{
    public GameController gameController;   //To call OnBallCollected()

    /// <summary>
    /// If a collider entered this collider, check if it is a ball (has BallMarker), destroy it and call gameController
    /// </summary>
    /// <param name="other"> entering collider</param>
    private void OnTriggerEnter(Collider other)
    {
        BallMarker marker = GetMarker(other);
        if (marker == null)
            return;

        Destroy(other.transform.gameObject);
        gameController.OnBallCollected(marker);
    }

    /// <summary>
    /// Returns the BallMarker of the colliders gameobject
    /// </summary>
    /// <param name="collider">other collider</param>
    /// <returns> BallMarker or null if collider is not a ball</returns>
    private BallMarker GetMarker(Collider collider)
    {
       return collider.transform.GetComponent<BallMarker>();
    }
}
