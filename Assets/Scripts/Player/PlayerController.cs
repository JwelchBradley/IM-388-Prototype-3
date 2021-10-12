/*****************************************************************************
// File Name :         PlayerController.cs
// Author :            Jacob Welch
// Creation Date :     15 September 2021
//
// Brief Description : Handles the inputs of the player and controls them.
*****************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The pausemenu in the scene.
    /// </summary>
    private PauseMenuBehavior pauseMenu;

    /// <summary>
    /// The player movement script on this player.
    /// </summary>
    private PlayerMovement pm;
    #endregion

    #region Funcitons
    #region Initialize
    /// <summary>
    /// Initiliazes the player.
    /// </summary>
    private void Awake()
    {
        pauseMenu = GameObject.Find("Pause Menu Templates Canvas").GetComponent<PauseMenuBehavior>();
        pm = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion

    #region Inputs
    /// <summary>
    /// Pauses the game if the escape key is pressed.
    /// </summary>
    private void OnPauseGame()
    {
        pauseMenu.PauseGame();
    }

    /// <summary>
    /// Makes the player jump.
    /// </summary>
    public void OnJump()
    {
        pm.Jump();
    }

    /// <summary>
    /// Calls for the player to be moved.
    /// </summary>
    /// <param name="input">A vector 2 input direction.</param>
    public void OnMovement(InputValue input)
    {
        pm.MovePlayer(input.Get<Vector2>());

    }

    /// <summary>
    /// Calls for the player to crouch or uncrouch.
    /// </summary>
    public void OnCrouch()
    {
        //pm.Crouch();
    }
    #endregion
    #endregion
}
