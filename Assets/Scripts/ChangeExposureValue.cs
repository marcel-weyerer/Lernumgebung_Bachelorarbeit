using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeExposureValue : MonoBehaviour
{
    // The predefined values of the exposure setting
    [SerializeField]
    private string[] exposureValues;

    // Index of the starting value in exposureValues
    [SerializeField]
    private int initValueIndex;

    // TextMeshPro Component
    private TMP_Text value;

    private InputDevice leftController;

    // Flag to check if thumb stick has been released once before switching to the next value
    private bool stickReleased;

    // Start is called before the first frame update
    void Start()
    {
        // Get right hand controller
        var leftHandedControllers = new List<InputDevice>();
        var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandedControllers);

        if (leftHandedControllers.Count > 0)
            leftController = leftHandedControllers[0];

        value = GetComponent<TMP_Text>();

        value.text = exposureValues[initValueIndex];

        stickReleased = true;
    }

    // Update is called once per frame
    void Update()
    {
        leftController.TryReadAxis2DValue(InputHelpers.Axis2D.PrimaryAxis2D, out Vector2 thumbStickValue);

        // display next value when stick is moved to the right and previous value if it is moved to the left
        if (stickReleased && (thumbStickValue.x <= -0.5))
        {
            // stick Released to false to only change a value once
            stickReleased = false;

            value.text = exposureValues[--initValueIndex % exposureValues.Length];
        }
        else if (stickReleased && (thumbStickValue.x >= 0.5))
        {
            // stick Released to false to only change a value once
            stickReleased = false;

            value.text = exposureValues[++initValueIndex % exposureValues.Length];
        }

        // Only when the stick is released the flag is set to true to allow another change of the value
        // This is supposed to prevent multiple value changes without releasing the stick inbetween
        if (!stickReleased && (thumbStickValue.x < 0.5) && (thumbStickValue.x > -0.5))
            stickReleased = true;
    }
}
