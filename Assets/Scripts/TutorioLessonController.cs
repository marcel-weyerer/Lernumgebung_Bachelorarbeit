using System;
using System.Collections;
using UnityEngine;
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
    private AudioSource tutorioAudioSource;

    // AudioSource of Video
    [SerializeField]
    private AudioSource videoAudioSource;

    // AudioSource of Player
    [SerializeField]
    private AudioSource playerAudioSource;

    // GameObject used for playing description video
    [SerializeField]
    private GameObject video;

    // Render Texture necessary for playing the instruction video
    [SerializeField]
    private RenderTexture renderTexture;

    // Audio Files
    // Sound effect when instruction video appears or vanishes
    [SerializeField]
    private AudioClip hideShowVideoSound;

    // Sound effect when completing a lesson
    [SerializeField]
    private AudioClip lessonCompleteAudio;

    // Sound effect when completing a chapter
    [SerializeField]
    private AudioClip chapterCompleteAudio;

    // GameObject to transition to next scene
    [SerializeField]
    private GameObject levelChanger;

    // The animator that controls Tutorio
    private Animator animator;

    // Variable to check if audio has finished playing
    private bool audioFinished;

    // Event that is triggered when all conditions of the current lesson have been met
    public event Action OnLessonCompleted;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        currentLesson = -1;
        audioFinished = true;

        OnLessonCompleted += StartCongrats;
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
                PlayAudioClip(tutorioAudioSource, lessons[currentLesson].getIntro());   // Play introduction audio
            StartCoroutine(GiveInstructions());
        }
        else
        {
            PlayAudioClip(playerAudioSource, chapterCompleteAudio);
            levelChanger.GetComponent<LevelChanger>().FadeToLevel("TutorialInteraction");
        }
    }

    private void PlayAudioClip(AudioSource source, AudioClip clip)
    {
        if (clip == null)
            throw new Exception("Missing Audio Clip!");
        if (source == null)
            throw new Exception("Missing Audio Source!");

        source.clip = clip;
        source.Play();
    }

    private IEnumerator WaitForAudio()
    {
        audioFinished = false;

        yield return new WaitUntil(() => tutorioAudioSource.isPlaying == false);

        // Set audioFinished true
        audioFinished = true;
    }

    private IEnumerator GiveInstructions()
    {
        // Wait until all conditions of current lesson have been met
        StartCoroutine(WaitForCompletion());

        // For all instruction audio clips make Tutorio look at the associated GameObject
        foreach (var instruction in lessons[currentLesson].getInstructions())
        {
            StartCoroutine(WaitForAudio());     // Wait for audio to finish playing

            yield return new WaitUntil(() => audioFinished);

            if (instruction.instructionType == Lesson.InstructionType.Video)
                PlayVideoClip();
            else if (instruction.instructionType == Lesson.InstructionType.Spawn)
                instruction.lookAtTarget.SetActive(true);

            PlayAudioClip(tutorioAudioSource, instruction.audioClip);
            StartCoroutine(WaitForAudio());

            transform.GetChild(0).GetComponent<LookAtTarget>().target = instruction.lookAtTarget.transform;
            StartCoroutine(LookAtInstruction(2));
        }
    }

    private void PlayVideoClip()
    {
        if (video == null)
            throw new Exception("Missing Description Video!");

        // Add video clip and render texture to the video player
        var videoPlayer = video.transform.GetChild(0).GetComponent<VideoPlayer>();
        videoPlayer.clip = lessons[currentLesson].getInstructionVideo();
        videoPlayer.targetTexture = renderTexture;

        // Add render texture to rawImage
        var rawImage = video.gameObject.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
        rawImage.texture = renderTexture;

        // Activate description video
        video.SetActive(true);
        PlayAudioClip(videoAudioSource, hideShowVideoSound);
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
            conditionComponent.ResetInputDetected();
            yield return new WaitUntil(() => conditionComponent.ActivateSelectedFunction(condition));
        }
        // When all conditions have been met trigger LessonCompleted
        OnLessonCompleted?.Invoke();

        PlayAudioClip(playerAudioSource, lessonCompleteAudio);
        PlayAudioClip(videoAudioSource, hideShowVideoSound);
    }

    // StartCongrats is being called, when a lesson has been completed successfully
    private void StartCongrats()
    {
        // Trigger for Celebration Animation
        animator.SetTrigger("StartCelebration");

        // Hide Video Player
        video.SetActive(false);

        // Play Audio Congratulation
        PlayAudioClip(tutorioAudioSource, lessons[currentLesson].getCongratulation());
    }
}
