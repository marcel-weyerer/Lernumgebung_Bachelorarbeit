using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TurnOffLeftHandConrol : MonoBehaviour
{
    [SerializeField]
    private GameObject origin;

    // Exposure Menu
    [SerializeField]
    private GameObject menu;

    private InputDevice leftController;

    private bool coroutineStarted;

    private void Start()
    {
        // Get left hand controller
        var leftHandedControllers = new List<InputDevice>();
        var desiredCharacteristicsLeft = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristicsLeft, leftHandedControllers);

        if (leftHandedControllers.Count > 0)
            leftController = leftHandedControllers[0];

        coroutineStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle menu when X-Button is pressed
        if (!coroutineStarted && leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool inputDetected) && inputDetected)
            StartCoroutine(ToggleMenu());

        if (menu.activeSelf)
            origin.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
        else
            origin.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
    }

    private IEnumerator ToggleMenu()
    {
        coroutineStarted = true;
        menu.SetActive(!menu.activeSelf);

        // Only allow a new toggle after some time
        yield return new WaitForSeconds(0.5f);

        coroutineStarted = false;
    }
}
