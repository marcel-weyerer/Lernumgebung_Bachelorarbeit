using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ToggleHandMenu : MonoBehaviour
{
    // Level Changer
    [SerializeField]
    private GameObject levelChanger;

    private InputDevice leftController;

    // Start is called before the first frame update
    void Start()
    {
        // Get left hand controller
        var leftHandedControllers = new List<InputDevice>();
        var desiredCharacteristicsLeft = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristicsLeft, leftHandedControllers);

        if (leftHandedControllers.Count > 0)
            leftController = leftHandedControllers[0];
    }

    // Update is called once per frame
    void Update()
    {
        // Check if Menu-Button is being pressed
        if(leftController.TryGetFeatureValue(CommonUsages.menuButton, out bool inputDetected) && inputDetected)
        {
            // Return to main menu
            levelChanger.GetComponent<LevelChanger>().FadeToLevel("MainMenu");
        }
    }
}
