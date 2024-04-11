using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject rightTeleportationRay;

    private InputDevice rightController;

    // Start is called before the first frame update
    void Start()
    {
        // Get right hand controller
        var rightHandedControllers = new List<InputDevice>();
        var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);

        if (rightHandedControllers.Count > 0 )
            rightController = rightHandedControllers[0];
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 thumbStickValue;

        // Enable Teleportation Ray when thumb Stick is moved foreward enough
        if (rightController.TryReadAxis2DValue(InputHelpers.Axis2D.PrimaryAxis2D, out thumbStickValue) && (thumbStickValue.y >= 0.5f))
            rightTeleportationRay.SetActive(true);
        else
            rightTeleportationRay.SetActive(false);
    }
}
