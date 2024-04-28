using System.Collections;
using UnityEngine;

public class PlaySoundOnMovement : MonoBehaviour
{
    // Audio Source for moving sound
    [SerializeField]
    private AudioSource audioSource;

    // Moving audio clip
    [SerializeField]
    private AudioClip clip;

    // Position in the last frame
    private Vector3 lastPosition;

    // Flag if coroutine is already running
    private bool coroutineRunning;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = clip;

        lastPosition = transform.position;

        coroutineRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Vector between lastPosition and current position
        var vectorToLastPosition = lastPosition - transform.position;

        // Y-coordinate should be ignored
        vectorToLastPosition.y = 0;

        // Play moving sound if Tutorio has moved since last frame
        if (vectorToLastPosition.magnitude > 0.005 && !coroutineRunning)
        {
            coroutineRunning = true;
            StartCoroutine(PlayMovingSound());
        }
    }

    private IEnumerator PlayMovingSound()
    {
        // Start playing moving sound
        audioSource.Play();

        yield return new WaitUntil(() =>
        {
            // Vector between lastPosition and current position
            var vectorToLastPosition = lastPosition - transform.position;

            // Y-coordinate should be ignored
            vectorToLastPosition.y = 0;

            // Save current position for the next frame
            lastPosition = transform.position;

            return vectorToLastPosition.magnitude < 0.005;
        });

        // Stop playing moving sound when Tutorio stops moving
        audioSource.Stop();

        coroutineRunning = false;
    }
}
