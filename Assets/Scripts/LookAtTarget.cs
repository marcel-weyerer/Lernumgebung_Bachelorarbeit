using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    // The target that this GameObject should look at
    public Transform target;

    // Rotation speed when turning towards target
    [Range(0f, 10f)]
    public float rotSpeed;

    // Update is called once per frame
    void Update()
    {
        // Get desired direction
        Vector3 dir = target.position - transform.position;

        // Get desired rotation
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

        // Applay rotation
        transform.rotation = rot;
    }
}
