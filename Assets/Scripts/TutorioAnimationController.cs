using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class TutorioAnimationController : MonoBehaviour
{
    // A list of lessons in this learning unity
    public Lesson[] lessons;

    // AudioSource of Tutorio
    public AudioSource audioSource;

    // GameObject used for playing description video
    public GameObject video;

    // The animator that controls Tutorio
    private Animator animator;

    // Index of the current lesson in lessons
    private int currentLesson;

    // Right Controller
    private List<InputDevice> rightHandedControllers;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentLesson = -1;

        // Get right hand controller
        rightHandedControllers = new List<InputDevice>();
        var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);
    }

    public void StartHoverAnim()
    {
        currentLesson++;

        // Starting Hover Animation
        animator.SetTrigger("StartHover");

        if (currentLesson < lessons.Length)
        {
            PlayAudioClip(lessons[currentLesson].getIntro());
            StartCoroutine(WaitForAudio(lessons[currentLesson].getIntro()));
        }
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private IEnumerator WaitForAudio(AudioClip Sound)
    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);

        PlayVideoClip();
    }

    private void PlayVideoClip()
    {
        if (video == null)
            return;

        Transform videoPlayer = video.gameObject.transform.GetChild(0);

        videoPlayer.GetComponent<VideoPlayer>().clip = lessons[currentLesson].getInstructionVideo();
        videoPlayer.GetComponent<VideoPlayer>().targetTexture = lessons[currentLesson].getRenderTexure();

        Transform rawImage = video.gameObject.transform.GetChild(1).GetChild(0);

        rawImage.GetComponent<RawImage>().texture = lessons[0].getRenderTexure();

        video.SetActive(true);

        StartCoroutine(WaitForCompletion());

        GetComponent<LookAtTarget>().target = video.transform;
        PlayAudioClip(lessons[currentLesson].getInstructionAudio());
        StartCoroutine(LookAtInstruction(3));
    }

    private IEnumerator LookAtInstruction(int sec)
    {
        yield return new WaitForSeconds(sec);

        GetComponent<LookAtTarget>().target = Camera.main.transform;
    }

    private IEnumerator WaitForCompletion()
    {
        yield return new WaitUntil(() =>
        {
            bool returnVal = false;

            foreach (var device in rightHandedControllers)
            {
                bool primaryButtonState = false;
                
                returnVal = device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonState) && primaryButtonState;
            }

            return returnVal;
        });
    }
}
