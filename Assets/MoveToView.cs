using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoveToView : MonoBehaviour
{
    public Transform Player;

    // Rotation speed when turning towards target
    [Range(0f, 0.5f)]
    public float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        if (Player.position.y - transform.position.y > 0.2f || Player.position.y - transform.position.y < -0.2f)
        {
            // Move our position a step closer to the target.
            var step = moveSpeed * Time.deltaTime; // calculate distance to move

            // Only move in y-coordinate
            var targetPosition = new Vector3(transform.position.x, Player.position.y, transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
    }
}
