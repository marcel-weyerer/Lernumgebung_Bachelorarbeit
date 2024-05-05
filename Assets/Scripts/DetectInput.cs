using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static Types;

public class DetectButtonPress : MonoBehaviour
{
    // XR Origin GameObject
    private GameObject player;

    // Dictionary that connects a FunctionOption to a specific method
    private Dictionary<FunctionOption, Func<GameObject[], bool>> functionLookup;

    private InputDevice rightController;

    // Variable to check if a specific Input has been made
    private bool inputDetected;

    // Event that is triggered when all conditions of the current lesson have been met
    public event Action OnMovePosition;

    // Check if player has entered a waypoint
    private bool waypointEntered;

    // Check if Object has been selected
    private bool objectSelected;

    // Check if Object has been activated
    private bool objectActivated;

    // Start is called before the first frame update
    void Start()
    {
        // Get right hand controller
        var rightHandedControllers = new List<InputDevice>();
        var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);

        if (rightHandedControllers.Count > 0)
            rightController = rightHandedControllers[0];

        // Initialize Dictionary
        functionLookup = new Dictionary<FunctionOption, Func<GameObject[], bool>>()
        {
            { FunctionOption.A_ButtonPress, DetectA_Button },
            { FunctionOption.RotateLeft, DetectRotationLeft },
            { FunctionOption.RotateRight, DetectRotationRight },
            { FunctionOption.Teleport, DetectTeleport },
            { FunctionOption.ContinuousMove, DetectContinuousMove },
            { FunctionOption.SelectObject, DetectObjectSelection },
            { FunctionOption.RaySelect, DetectRayObjectSelection },
            { FunctionOption.Activate, DetectObjectActivate }
        };

        // Find XR Origin GameObject
        player = GameObject.FindGameObjectWithTag("Player");

        waypointEntered = false;
        objectSelected = false;
        objectActivated = false;
    }

    public void ResetInput()
    {
        inputDetected = false;
        waypointEntered = false;
        objectSelected = false;
        objectActivated = false;
    }

    // ActivateSelectedFunction is called to call a selected function
    public bool ActivateSelectedFunction(Condition condition)
    {
        return functionLookup[condition.selectedFunction].Invoke(condition.parameters);
    }

    // A_ButtonPress detects wether the A-Button has been pressed
    private bool DetectA_Button(GameObject[] param)
    {
        // Check if A-Button is being pressed
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out inputDetected);

        // Haptic Impule on completion
        if (inputDetected)
            rightController.SendHapticImpulse(2, 0.3f);

        return inputDetected;
    }

    private bool DetectRotationLeft(GameObject[] param)
    {
        return DetectRotation(-0.5f);
    }

    private bool DetectRotationRight(GameObject[] param)
    {
        return DetectRotation(0.5f);
    }

    private bool DetectRotation(float stickValue)
    {
        // Activate rotation functionality
        player.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;

        // Start Measuring Time until we have waited long enough or until Player has stopped rotating
        if (stickValue < 0)
        {
            if (rightController.TryReadAxis2DValue(InputHelpers.Axis2D.PrimaryAxis2D, out Vector2 thumbStickValue) && (thumbStickValue.x <= stickValue))
                inputDetected = true;
        } else
        {
            if (rightController.TryReadAxis2DValue(InputHelpers.Axis2D.PrimaryAxis2D, out Vector2 thumbStickValue) && (thumbStickValue.x >= stickValue))
                inputDetected = true;
        }

        // Invoke event to move Tutorio
        if (inputDetected)
        {
            OnMovePosition?.Invoke();
            rightController.SendHapticImpulse(2, 0.3f);
        }

        // Will only be true when the Player has rotated for long enough
        return inputDetected;
    }

    private bool DetectTeleport(GameObject[] param)
    {
        if (param == null || !param.Any())
            throw new Exception("param must contain one element!");

        // Activate teleportation functionality
        player.GetComponent<ActivateTeleportationRay>().enabled = true;

        return DetectWaypointEnter(param.First());
    }

    private bool DetectContinuousMove(GameObject[] param)
    {
        if (param == null || !param.Any())
            throw new Exception("param must contain one element!");

        // Deactivate Teleportation
        player.GetComponent<ActivateTeleportationRay>().enabled = false;
        // Activate Continuous Move
        player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;

        return DetectWaypointEnter(param.First());
    }

    private bool DetectWaypointEnter(GameObject param)
    {
        // Subscribe method to player enter event
        param.GetComponent<Trigger>().OnPlayerEnter += () => waypointEntered = true;

        // Invoke event to move Tutorio
        if (waypointEntered)
        {
            OnMovePosition?.Invoke();
            rightController.SendHapticImpulse(2, 0.3f);
        }

        return waypointEntered;
    }

    private bool DetectObjectSelection(GameObject[] param)
    {
        if (param == null || !param.Any())
            throw new Exception("param must contain one element!");

        param.First().GetComponent<XRGrabInteractable>().selectEntered.AddListener((SelectEnterEventArgs) => { objectSelected = true; });

        // Haptic Impule on completion
        if (objectSelected)
            rightController.SendHapticImpulse(2, 0.3f);

        return objectSelected;
    }

    private bool DetectRayObjectSelection(GameObject[] param)
    {
        if (param == null || param.Length < 2)
            throw new Exception("param must contain one element!");

        param[1].SetActive(true);

        return DetectObjectSelection(param);
    }

    private bool DetectObjectActivate(GameObject[] param)
    {
        if (param == null || !param.Any())
            throw new Exception("param must contain one element!");

        // ToDo: Activate Activation

        param.First().GetComponent<XRGrabInteractable>().activated.AddListener((ActivateEnterEventArgs) => { objectActivated = true; });

        // Haptic Impule on completion
        if (objectActivated)
            rightController.SendHapticImpulse(2, 0.3f);

        return objectActivated;
    }
}
