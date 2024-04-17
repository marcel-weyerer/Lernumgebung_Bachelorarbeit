using System.Collections;
using UnityEngine;

public class MoveToView : MonoBehaviour
{
    public Transform Player;

    // Rotation speed when turning towards target
    [Range(0f, 1f)]
    public float moveSpeed;

    // List of positions that Tutorio will move to on specific events
    [SerializeField]
    private Vector3[] positions;
    private int currentposition;

    void Start()
    {
        GetComponentInParent<DetectButtonPress>().OnMovePosition += MoveToNextPosition;

        currentposition = -1;
    }

    private void MoveToNextPosition()
    {
        currentposition++;

        if (currentposition < positions.Length)
        {
            StartCoroutine(TranslateSmoothly(positions[currentposition]));
        }
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
