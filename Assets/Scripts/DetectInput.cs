using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Variable to check if couroutine is currently running
    private bool coroutineStarted;

    // Event that is triggered when all conditions of the current lesson have been met
    public event Action OnMovePosition;

    // Check if player has entered a waypoint
    private bool waypointEntered;

    // Check if Object has been selected
    private bool objectSelected;

    // Check if Object has been activated
    private bool objectActivated;

    private IEnumerator measure;

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
            { FunctionOption.SelectObject, DetectObjectSelection }
        };

        // Find XR Origin GameObject
        player = GameObject.FindGameObjectWithTag("Player");

        coroutineStarted = false;
        waypointEntered = false;
        objectSelected = false;
        objectActivated = false;
    }

    public void ResetInputDetected()
    {
        inputDetected = false;
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

        if (inputDetected)
            rightController.SendHapticImpulse(2, 0.3f);

        return inputDetected;
    }

    private bool DetectRotationLeft(GameObject[] param)
    {
        Vector2 thumbStickValue;

        // Activate rotation functionality
        player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;

        // Start Measuring Time until we have waited long enough or until Player has stopped rotating
        if (rightController.TryReadAxis2DValue(InputHelpers.Axis2D.PrimaryAxis2D, out thumbStickValue) && (thumbStickValue.x <= -0.5f)) {
            if (!coroutineStarted)
            {
                measure = MeasureTime(2f);
                StartCoroutine(measure);
            }
        } else {
            if (measure != null)
                StopCoroutine(measure);
            coroutineStarted= false;
        }

        // Invoke event to move Tutorio
        if (inputDetected)
            OnMovePosition?.Invoke();

        // Will only be true when the Player has rotated for long enough
        return inputDetected;
    }

    private bool DetectRotationRight(GameObject[] param)
    {
        Vector2 thumbStickValue;

        // Activate rotation functionality
        player.GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;

        // Start Measuring Time until we have waited long enough or until Player has stopped rotating
        if (rightController.TryReadAxis2DValue(InputHelpers.Axis2D.PrimaryAxis2D, out thumbStickValue) && (thumbStickValue.x >= 0.5f))
        {
            if (!coroutineStarted)
            {
                measure = MeasureTime(2f);
                StartCoroutine(measure);
            }
        }
        else
        {
            if (measure != null)
                StopCoroutine(measure);
            coroutineStarted = false;
        }

        // Invoke event to move Tutorio
        if (inputDetected)
            OnMovePosition?.Invoke();

        // Will only be true when the Player has rotated for long enough
        return inputDetected;
    }

    private bool DetectTeleport(GameObject[] param)
    {
        // Activate teleportation functionality
        player.GetComponent<ActivateTeleportationRay>().enabled = true;

        // Subscribe method to player enter event
        param.First().GetComponent<Trigger>().OnPlayerEnter += () => waypointEntered = true;

        // Reset waypointEnter
        bool returnValue = waypointEntered;
        waypointEntered = false;

        // Invoke event to move Tutorio
        if (returnValue)
            OnMovePosition?.Invoke();

        return returnValue;
    }

    private bool DetectContinuousMove(GameObject[] param)
    {
        // Deactivate Teleportation
        player.GetComponent<ActivateTeleportationRay>().enabled = false;
        // Activate Continuous Move
        player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;

        // Subscribe method to player enter event
        param.First().GetComponent<Trigger>().OnPlayerEnter += () => waypointEntered = true;

        // Reset waypointEnter
        bool returnValue = waypointEntered;
        waypointEntered = false;

        // Invoke event to move Tutorio
        if (returnValue)
            OnMovePosition?.Invoke();

        return returnValue;
    }

    private bool DetectObjectSelection(GameObject[] param)
    {
        if (param == null || !param.Any())
            throw new Exception("param must contain one element!");

        param.First().GetComponent<XRGrabInteractable>().selectEntered.AddListener((SelectEnterEventArgs) => { objectSelected = true; });

        // Reset objectSelected
        bool returnValue = objectSelected;
        objectSelected = false;

        return returnValue;
    }

    private IEnumerator MeasureTime(float time)
    {
        coroutineStarted = true;
        var startTime = Time.time;

        yield return new WaitUntil(() => (Time.time - startTime) > time);

        rightController.SendHapticImpulse(2, 0.3f);

        // Set inputDetected to true when we have waited long enough
        inputDetected = true;
        coroutineStarted = false;
    }
}
