using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static Lesson;

public class DetectButtonPress : MonoBehaviour
{
    // Teleport Waypoint
    public GameObject teleportWaypoint;

    // Dictionary that connects a FunctionOption to a specific method
    private Dictionary<FunctionOption, Func<bool>> functionLookup;

    private InputDevice rightController;

    // Variable to check if a specific Input has been made
    private bool inputDetected;

    // Variable to check if couroutine is currently running
    private bool coroutineStarted;

    // Event that is triggered when all conditions of the current lesson have been met
    public event Action OnMovePosition;

    private bool waypointEnter;

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
        functionLookup = new Dictionary<FunctionOption, System.Func<bool>>()
        {
            { FunctionOption.A_ButtonPress, DetectA_Button },
            { FunctionOption.RotateLeft, DetectRotationLeft },
            { FunctionOption.RotateRight, DetectRotationRight },
            { FunctionOption.Teleport, DetectTeleport }
        };

        coroutineStarted = false;
        waypointEnter = false;
    }

    public void SetInputDetected()
    {
        inputDetected = false;
    }

    // ActivateSelectedFunction is called to call a selected function
    public bool ActivateSelectedFunction(FunctionOption selectedFunction)
    {
        return functionLookup[selectedFunction].Invoke();
    }

    // A_ButtonPress detects wether the A-Button has been pressed
    private bool DetectA_Button()
    {
        // Check if A-Button is being pressed
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out inputDetected);

        if (inputDetected)
            rightController.SendHapticImpulse(2, 0.3f);

        return inputDetected;
    }

    private bool DetectRotationLeft()
    {
        Vector2 thumbStickValue;
        Coroutine coroutine = null;

        // Start Measuring Time until we have waited long enough or until Player has stopped rotating
        if (rightController.TryReadAxis2DValue(InputHelpers.Axis2D.PrimaryAxis2D, out thumbStickValue) && (thumbStickValue.x <= -0.5f)) {
            if (!coroutineStarted)
                coroutine = StartCoroutine(MeasureTime(2f));
        } else {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutineStarted= false;
        }

        // Invoke event to move Tutorio
        if (inputDetected)
            OnMovePosition?.Invoke();

        // Will only be true when the Player has rotated for long enough
        return inputDetected;
    }

    private bool DetectRotationRight()
    {
        Vector2 thumbStickValue;
        Coroutine coroutine = null;

        // Start Measuring Time until we have waited long enough or until Player has stopped rotating
        if (rightController.TryReadAxis2DValue(InputHelpers.Axis2D.PrimaryAxis2D, out thumbStickValue) && (thumbStickValue.x >= 0.5f))
        {
            if (!coroutineStarted)
                coroutine = StartCoroutine(MeasureTime(2f));
        }
        else
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutineStarted = false;
        }

        // Invoke event to move Tutorio
        if (inputDetected)
            OnMovePosition?.Invoke();

        // Will only be true when the Player has rotated for long enough
        return inputDetected;
    }

    private bool DetectTeleport()
    {
        // Activate Waypoint and add Listener to Trigger Event
        if (!teleportWaypoint.activeInHierarchy)
            teleportWaypoint.SetActive(true);
        teleportWaypoint.GetComponent<Trigger>().OnPlayerEnter += () => waypointEnter = true;

        bool entered = waypointEnter;
        waypointEnter = false;

        // Invoke event to move Tutorio
        if (entered)
            OnMovePosition?.Invoke();

        return entered;
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
