using System.Collections;
using UnityEngine;

public class MoveToView : MonoBehaviour
{
    // Rotation speed when turning towards target
    [SerializeField]
    [Range(0f, 1f)]
    private float moveSpeed;

    public void MoveToNextPosition(Vector3 position)
    {
        if (position == null)
            throw new System.Exception("Position must not be null!");

        StartCoroutine(TranslateSmoothly(position));
    }

    private IEnumerator TranslateSmoothly(Vector3 nextPosition)
    {
        yield return new WaitUntil(() =>
        {
            // Move our position a step closer to the target.
            var step = moveSpeed * Time.deltaTime; // calculate distance to move

            // Smoothly move to target position
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, step);

            if (Vector3.Distance(transform.position, nextPosition) < 0.001)
            {
                transform.position = nextPosition;
                return true;
            }

            return false;
        });
    }
}
