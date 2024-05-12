using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static Types;

public class DetectButtonPress : MonoBehaviour
{
    // XR Origin GameObject
    private GameObject player;

    // Dictionary that connects a FunctionOption to a specific method
    private Dictionary<ConditionType, Func<Condition, bool>> functionLookup;

    private InputDevice rightController;
    private InputDevice leftController;

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
        var desiredCharacteristicsRight = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristicsRight, rightHandedControllers);

        if (rightHandedControllers.Count > 0)
            rightController = rightHandedControllers[0];

        // Get left hand controller
        var leftHandedControllers = new List<InputDevice>();
        var desiredCharacteristicsLeft = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristicsLeft, leftHandedControllers);

        if (leftHandedControllers.Count > 0)
            leftController = leftHandedControllers[0];

        // Initialize Dictionary
        functionLookup = new Dictionary<ConditionType, Func<Condition, bool>>()
        {
            { ConditionType.A_ButtonPress, DetectA_Button },
            { ConditionType.X_ButtonPress, DetectX_Button },
            { ConditionType.RotateLeft, DetectRotationLeft },
            { ConditionType.RotateRight, DetectRotationRight },
            { ConditionType.Teleport, DetectTeleport },
            { ConditionType.ContinuousMove, DetectContinuousMove },
            { ConditionType.SelectObject, DetectObjectSelection },
            { ConditionType.RaySelect, DetectRayObjectSelection },
            { ConditionType.TakePhoto, DetectPhotoTaken },
            { ConditionType.CompareValue, DetectValueComparison }
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
        return functionLookup[condition.selectedFunction].Invoke(condition);
    }

    // A_ButtonPress detects wether the A-Button has been pressed
    private bool DetectA_Button(Condition condition)
    {
        // Check if A-Button is being pressed
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out inputDetected);

        // Haptic Impule on completion
        if (inputDetected)
            rightController.SendHapticImpulse(2, 0.3f);

        return inputDetected;
    }

    private bool DetectX_Button(Condition condition)
    {
        // Check if X-Button is being pressed
        leftController.TryGetFeatureValue(CommonUsages.primaryButton, out inputDetected);

        // Haptic Impule on completion
        if (inputDetected)
            leftController.SendHapticImpulse(2, 0.3f);

        return inputDetected;
    }

    private bool DetectRotationLeft(Condition condition)
    {
        return DetectRotation(-0.5f);
    }

    private bool DetectRotationRight(Condition condition)
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

    private bool DetectTeleport(Condition condition)
    {
        if (condition.objects == null || !condition.objects.Any())
            throw new Exception("condition.objects must contain one element!");

        // TODO: Check if component exists in object (same for following methods)

        // Activate teleportation functionality
        player.GetComponent<ActivateTeleportationRay>().enabled = true;

        return DetectWaypointEnter(condition.objects[0]);
    }

    private bool DetectContinuousMove(Condition condition)
    {
        if (condition.objects == null || !condition.objects.Any())
            throw new Exception("condition.objects must contain one element!");

        // Deactivate Teleportation
        player.GetComponent<ActivateTeleportationRay>().enabled = false;
        // Activate Continuous Move
        player.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;

        return DetectWaypointEnter(condition.objects[0]);
    }

    private bool DetectWaypointEnter(GameObject waypoint)
    {
        // Subscribe method to player enter event
        waypoint.GetComponent<Trigger>().OnPlayerEnter += () => waypointEntered = true;

        // Invoke event to move Tutorio
        if (waypointEntered)
        {
            OnMovePosition?.Invoke();
            rightController.SendHapticImpulse(2, 0.3f);
        }

        return waypointEntered;
    }

    private bool DetectObjectSelection(Condition condition)
    {
        if (condition.objects == null || !condition.objects.Any())
            throw new Exception("condition.objects must contain one element!");

        // Activate select input of controller
        condition.objects[1].GetComponent<XRController>().enableInputActions = true;

        // Wait for interactable to be selected
        condition.objects[0].GetComponent<XRGrabInteractable>().selectEntered.AddListener((SelectEnterEventArgs) => { objectSelected = true; });

        // Haptic Impule on completion
        if (objectSelected)
            rightController.SendHapticImpulse(2, 0.3f);

        return objectSelected;
    }

    private bool DetectRayObjectSelection(Condition condition)
    {
        if (condition.objects == null || condition.objects.Length < 2)
            throw new Exception("condition.objects must contain one element!");

        condition.objects[1].SetActive(true);

        return DetectObjectSelection(condition);
    }

    private bool DetectPhotoTaken(Condition condition)
    {
        if (condition.objects == null || !condition.objects.Any())
            throw new Exception("condition.objects must contain one element!");

        var captureComponent = condition.objects[0].GetComponent<PhotoCapture>();

        // Activate Photo Capture component of camera
        captureComponent.enabled = true;

        condition.objects[0].GetComponent<XRGrabInteractable>().activated.AddListener((ActivateEnterEventArgs) => { 
            objectActivated = true;
            captureComponent.picture = condition.objects[1];
        });

        // Haptic Impule on completion and reset
        if (objectActivated)
        {
            captureComponent.enabled = false;
            rightController.SendHapticImpulse(2, 0.3f);
        }

        return objectActivated;
    }

    private bool DetectValueComparison(Condition condition)
    {
        if (condition.objects == null || !condition.objects.Any())
            throw new Exception("condition.objects must contain one element!");
        if (condition.comparison == null || condition.comparison.Length < 1)
            throw new Exception("condition.comparisons must not be empty or null!");

        var valueComponent = condition.objects[0].GetComponent<ChangeExposureValue>();

        condition.objects[1].GetComponent<Button>().onClick.AddListener(() =>
        {
            // Compare current exposure value to expected value
            if (valueComponent.GetCurrentUnsavedValue().Equals(condition.comparison))
            {
                valueComponent.PlayCorrectSound();
                valueComponent.SetCurrentValue();

                condition.objects[2].GetComponent<Button>().interactable = false;

                inputDetected = true;
            } else
            {
                valueComponent.PlayErrorSound();
            }
        });
        
        return inputDetected;
    }
}
