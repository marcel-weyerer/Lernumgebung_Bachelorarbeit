using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using static Types;

public class TutorioLessonController : MonoBehaviour
{
    [Header("Lessons")]
    // A list of lessons in this learning unity
    [SerializeField]
    private Lesson[] lessons;

    [Header("Audio Sources")]
    // AudioSource of Tutorio
    [SerializeField]
    private AudioSource tutorioAudioSource;

    // AudioSource of Video
    [SerializeField]
    private AudioSource videoAudioSource;

    // AudioSource of Player
    [SerializeField]
    private AudioSource playerAudioSource;

    [Header("Audio Clips")]
    // Sound effect when instruction video appears or vanishes
    [SerializeField]
    private AudioClip hideShowVideoSound;

    // Sound effect when completing a lesson
    [SerializeField]
    private AudioClip lessonCompleteAudio;

    // Sound effect when completing a chapter
    [SerializeField]
    private AudioClip chapterCompleteAudio;

    [Header("Video Player")]
    // GameObject used for playing description video
    [SerializeField]
    private GameObject video;

    // Render Texture necessary for playing the instruction video
    [SerializeField]
    private RenderTexture renderTexture;

    [Header("Level Changer")]
    // GameObject to transition to next scene
    [SerializeField]
    private GameObject levelChanger;

    // Scene to load after this one
    [SerializeField]
    private string nextScene;

    // Index of the current lesson in lessons
    private int currentLesson;

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
        // Reset all coroutines
        StopAllCoroutines();

        // Starting Hover Animation
        animator.SetTrigger("StartHover");

        currentLesson++;

        // While there are still lessons to do, continue
        // Else load next scene
        if (currentLesson < lessons.Length)
        {
            if (lessons[currentLesson].GetIntro() != null)
                PlayAudioClip(tutorioAudioSource, lessons[currentLesson].GetIntro());   // Play introduction audio
            StartCoroutine(GiveInstructions());
        }
        else
        {
            PlayAudioClip(playerAudioSource, chapterCompleteAudio);
            levelChanger.GetComponent<LevelChanger>().FadeToLevel(nextScene);
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
        // For all instruction audio clips make Tutorio look at the associated GameObject
        foreach (var instruction in lessons[currentLesson].GetInstructions())
        {
            StartCoroutine(WaitForAudio());     // Wait for audio to finish playing

            yield return new WaitUntil(() => audioFinished);

            switch (instruction.instructionType)
            {
                case InstructionType.Video:
                    PlayVideoClip();
                    break;
                case InstructionType.ShowObject:
                    instruction.target.SetActive(true);
                    break;
                case InstructionType.HideObject:
                    instruction.target.SetActive(false);
                    break;
                case InstructionType.TeleportObject:
                    TeleportObject(instruction.target, instruction.optPosition);
                    break;
                case InstructionType.MoveObject:
                    MoveObject(instruction.target, instruction.optPosition);
                    break;
                case InstructionType.RotateObject:
                    RotateObject(instruction.target, instruction.optPosition);
                    break;
                default:
                    break;
            }

            if (instruction.audioClip != null)
            {
                PlayAudioClip(tutorioAudioSource, instruction.audioClip);
                StartCoroutine(WaitForAudio());
            }

            transform.GetChild(0).GetComponent<LookAtTarget>().target = instruction.target.transform;
            StartCoroutine(LookBackAtPlayer(2));
        }

        // Wait until all conditions of current lesson have been met
        StartCoroutine(WaitForCompletion());
    }

    private void PlayVideoClip()
    {
        if (video == null)
            throw new Exception("Missing Description Video!");

        // Add video clip and render texture to the video player
        var videoPlayer = video.transform.GetChild(0).GetComponent<VideoPlayer>();
        videoPlayer.clip = lessons[currentLesson].GetInstructionVideo();
        videoPlayer.targetTexture = renderTexture;

        // Add render texture to rawImage
        var rawImage = video.transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
        rawImage.texture = renderTexture;

        video.SetActive(true);
        PlayAudioClip(videoAudioSource, hideShowVideoSound);
    }

    private IEnumerator LookBackAtPlayer(int sec)
    {
        yield return new WaitForSeconds(sec);

        // Make Tutorio look at Player again
        transform.GetChild(0).GetComponent<LookAtTarget>().target = Camera.main.transform;
    }

    private IEnumerator WaitForCompletion()
    {
        var conditionComponent = GetComponent<DetectButtonPress>();

        // Wait for each condition of current lesson to be completed
        foreach (var condition in lessons[currentLesson].GetConditions())
        {
            conditionComponent.ResetInput();
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
        PlayAudioClip(tutorioAudioSource, lessons[currentLesson].GetCongratulation());
    }

    private void TeleportObject(GameObject obj, Vector3 position)
    {
        var interactable = obj.GetComponent<XRGrabInteractable>();

        if (interactable == null)
            return;

        // Deactivate interactable component while moving the object
        interactable.enabled = false;

        obj.transform.position = position;

        // Activate interactable component
        interactable.enabled = true;
    }

    private void MoveObject(GameObject obj, Vector3 position)
    {
        obj.GetComponent<MoveToView>().MoveToNextPosition(position);
    }

    private void RotateObject(GameObject obj, Vector3 rotation)
    {
        obj.transform.rotation = Quaternion.Euler(rotation);
    }
}
