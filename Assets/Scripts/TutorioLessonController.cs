using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorioLessonController : MonoBehaviour
{
    // A list of lessons in this learning unity
    [SerializeField]
    private Lesson[] lessons;

    // Index of the current lesson in lessons
    private int currentLesson;

    // AudioSource of Tutorio
    [SerializeField]
    private AudioSource audioSource;

    // GameObject used for playing description video
    [SerializeField]
    private GameObject video;

    // Render Texture necessary for playing the instruction video
    [SerializeField]
    private RenderTexture renderTexture;

    // The animator that controls Tutorio
    private Animator animator;

    // Event that is triggered when all conditions of the current lesson have been met
    private UnityEvent LessonCompleted;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        currentLesson = -1;

        LessonCompleted = new UnityEvent();

        // Listen for Event on Completion
        LessonCompleted.AddListener(StartCongrats);
    }

    public void StartHoverAnim()
    {
        // Starting Hover Animation
        animator.SetTrigger("StartHover");

        currentLesson++;

        // While there are still lessons to do, continue
        // Else, return to Level Menu
        if (currentLesson < lessons.Length)
        {
            if (lessons[currentLesson].getIntro() != null)
                PlayAudioClip(lessons[currentLesson].getIntro());   // Play introduction audio
            StartCoroutine(WaitForAudio());     // Wait for audio to finish playing
        } else
        {

        }
    }

    private IEnumerator WaitForAudio()
    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);

        // When introduction has finished playing, start playing the description video
        PlayVideoClip();
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip == null)
            throw new System.Exception("Missing Audio Clip!");

        audioSource.clip = clip;
        audioSource.Play();
    }

    private void PlayVideoClip()
    {
        if (video == null)
            throw new System.Exception("Missing Description Video!");

        // Add video clip and render texture to the video player
        var videoPlayer = video.transform.GetChild(0).GetComponent<VideoPlayer>();
        videoPlayer.clip = lessons[currentLesson].getInstructionVideo();
        videoPlayer.targetTexture = renderTexture;

        // Add render texture to rawImage
        var rawImage = video.gameObject.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
        rawImage.texture = renderTexture;

        // Activate description video
        video.SetActive(true);

        // Wait until all conditions of current lesson have been met
        StartCoroutine(WaitForCompletion());

        // Make Tutorio look at the video for a number of seconds to redirect attention of the Player
        transform.GetChild(0).GetComponent<LookAtTarget>().target = video.transform;
        StartCoroutine(LookAtInstruction(3));

        // Play instruction audio
        PlayAudioClip(lessons[currentLesson].getInstructionAudio());
    }

    private IEnumerator LookAtInstruction(int sec)
    {
        yield return new WaitForSeconds(sec);

        // Make Tutorio look at Player again
        transform.GetChild(0).GetComponent<LookAtTarget>().target = Camera.main.transform;
    }

    private IEnumerator WaitForCompletion()
    {
        var conditionComponent = GetComponent<DetectButtonPress>();

        // Wait for each condition of current lesson to be completed
        foreach (var condition in lessons[currentLesson].getConditions())
        {
            conditionComponent.SetInputDetected();
            yield return new WaitUntil(() => conditionComponent.ActivateSelectedFunction(condition));
        }

        // When all conditions have been met trigger LessonCompleted
        LessonCompleted.Invoke();
    }

    // StartCongrats is being called, when a lesson has been completed successfully
    private void StartCongrats()
    {
        // Trigger for Celebration Animation
        animator.SetTrigger("StartCelebration");

        // Hide Video Player
        video.SetActive(false);

        // Play Audio Congratulation
        PlayAudioClip(lessons[currentLesson].getCongratulation());
    }
}
