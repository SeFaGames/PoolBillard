using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to mark game objects as balls and allow other scripts to determine the type of ball.
/// </summary>
public class BallMarker : MonoBehaviour
{
    public bool isFull = false;     //true = full ball, false = half ball
    public bool isQueue = false;    //ball is queue ball or not
    public bool isEight = false;    //ball is eight ball or not
}
