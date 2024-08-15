using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to manage the menu.
/// It provides public methods to open and close the menu
/// Associated with GameController, QueueController, ScoreBoardManager.
/// </summary>
public class MenuManager : MonoBehaviour
{
    public QueueController queueController;
    public Canvas menu;
    public bool menuOpen = false;

    /// <summary>
    /// Deactivate Menu by default
    /// </summary>
    void Start()
    {
        menu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Toggles menu visibilty each time esc is pressed.
    /// Also suppresses queue if menu is open
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !Input.GetKey(KeyCode.Tab))
        {
            if (!menuOpen)
                menuOpen = true;
            else
                menuOpen = false;
        }

        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (menuOpen)
            {
                queueController.SetSuppressQueue(true);
                menu.gameObject.SetActive(true);
            }
            else
            {
                queueController.SetSuppressQueue(false);
                menu.gameObject.SetActive(false);
            }   
        }
    }

    /// <summary>
    /// Opens the menu
    /// </summary>
    public void OpenMenu()
    {
        menu.gameObject.SetActive(true);
        menuOpen = true;
    }

    /// <summary>
    /// Closes the menu
    /// </summary>
    public void CloseMenu()
    {
        menu.gameObject.SetActive(false);
        menuOpen = false;
    }
}
