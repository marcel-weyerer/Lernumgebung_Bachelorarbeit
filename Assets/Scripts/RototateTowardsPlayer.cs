using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    // Update is called once per frame
    void Update()
    {
        // Make object turn towards target
        transform.LookAt(target.transform);

        // The object should only turn on y-Axis
        var targetRotation = transform.rotation.eulerAngles;
        targetRotation.x = 0;
        targetRotation.z = 0;

        // Apply rotation
        transform.rotation = Quaternion.Euler(targetRotation);
    }
}
