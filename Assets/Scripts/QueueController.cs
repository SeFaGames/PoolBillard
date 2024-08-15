using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to handle the queue controls.
/// It provides public methods to suppress and update the queue.
/// Its associated with the GameController, MenuManager.
/// </summary>
public class QueueController : MonoBehaviour
{
    public BallManager ballManager;     //To retrieve the queue ball
    public GameObject rotationAnchor;   //To rotate the queue
    public GameObject queue;            //To move the queue forwards or backwards
    public MenuManager menuManager;     //To Check if the menu is open if asked to unsuppress

    public float tensionPower = 2.0f;   //Determines the amount of tension while pulling the queue backwards

    private Vector2 startMousePos;          //To calculate distance between mouse down and up
    private bool mouseDown;                 //whether or not the left mouse button is down
    private bool suppressQueue = false;     //whether or not the queue is suppressed
    private float impulsePower = 1.0f;      //To store the latest impulse power
    private float suppressionDelay = 0.0f;  //For timed suppression (seconds)

    /// <summary>
    /// Updates the queue position on start
    /// </summary>
    void Start()
    {
        UpdatePosition();
    }

    /// <summary>
    /// Retrieves the queue ball and positions the rotation anchor on its position and orientation
    /// </summary>
    public void UpdatePosition()
    {
        Transform queueBall = GetPrimaryQueueBall().transform;
        rotationAnchor.transform.position = queueBall.position;
        rotationAnchor.transform.rotation = queueBall.rotation;
    }

    /// <summary>
    /// Sets the queue to be suppressed (true) or unsuppressed (false).
    /// A suppressed queue is not visible and will not interact with the balls.
    /// Used while the turn is active (balls moving) or the menu is open (allow left click on menu buttons)
    /// </summary>
    /// <param name="suppressQueue">true = suppressed, false = unsuppressed</param>
    public void SetSuppressQueue(bool suppressQueue)
    {
        if(menuManager.menuOpen && !suppressQueue)
        {
            return;
        }
        this.suppressQueue = suppressQueue;
    }

    /// <summary>
    /// Retrieves the primary (latest) queue ball.
    /// </summary>
    /// <returns> newest queue ball (if multiple are present) </returns>
    /// <exception cref="System.Exception"></exception>
    private BallMarker GetPrimaryQueueBall()
    {
        BallMarker[] queueMarkers = ballManager.GetAllQueueBalls();
        if (queueMarkers.Length == 0)
        {
            throw new System.Exception("Spawn Queue Ball first");
        }
        return queueMarkers[queueMarkers.Length -1];
    }

    /// <summary>
    /// Handles Control of the queue.
    /// </summary>
    void Update()
    {
        //If Queue is Suppressed, skip logic.
        if (suppressQueue || suppressionDelay > 0)
        {
            queue.SetActive(false);
            if(suppressionDelay > 0)
                suppressionDelay -= Time.deltaTime;
            return;
        }
        queue.SetActive(true);

        //Calculate rotation of rotation anchor
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(rotationAnchor.transform.position).z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 direction = mouseWorldPos - rotationAnchor.transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        rotationAnchor.transform.rotation = Quaternion.Euler(2.75f, angle, 0);

        //For the frame the mouse button is pressed down, store position
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            startMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        //If mouse is down, calculate distance of mouse to start position and related impulse strength
        if (mouseDown)
        {
            Vector2 currentMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            float distance = Mathf.Abs(Vector2.Distance(startMousePos, currentMousePos));
            float displacement = Mathf.Pow(distance, 1.0f/tensionPower) / 100.0f;
            displacement = Mathf.Max(0.05f, displacement);
            Vector3 displacedQueuePos = new Vector3(0, 0, -displacement);
            queue.transform.SetLocalPositionAndRotation(displacedQueuePos, Quaternion.identity);

            impulsePower = displacement * 5;
        }

        //For the frame the mouse button goes back up, release queue (reset position) and apply force to ball
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Releasing Queue with Impulse Power of " + impulsePower);
            mouseDown = false;
            queue.transform.SetLocalPositionAndRotation(new Vector3(0, 0, -0.05f), Quaternion.identity);

            GameObject queueBall = GetPrimaryQueueBall().gameObject;
            queueBall.GetComponent<Rigidbody>().AddForce(direction * impulsePower, ForceMode.Impulse);
        }

    }

    /// <summary>
    /// Sets the queue as suppressed for the specified amount of seconds
    /// </summary>
    /// <param name="seconds">duration to suppress</param>
    public void SuppressFor(float seconds)
    {
        suppressionDelay = seconds;
    }
}
