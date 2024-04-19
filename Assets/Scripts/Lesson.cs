using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class Lesson
{
    [System.Serializable]
    public struct Instruction
    {
        public InstructionType instructionType;
        public AudioClip audioClip;
        public GameObject lookAtTarget;
    }

    public enum InstructionType { Video, Spawn }

    // Enum for all available Functions for the conditions
    public enum FunctionOption
    {
        A_ButtonPress,
        RotateLeft,
        RotateRight,
        Teleport,
        ContinuousMove
    };

    // Introduction to the new lesson. What is the lesson about?
    [SerializeField]
    private AudioClip intro;

    // Instruction für the lesson
    [SerializeField]
    //private AudioClip instructionAudio;
    private Instruction[] instructions;

    // Video that shows how the lesson can be completed
    [SerializeField]
    private VideoClip instructionVideo;

    // Conditions that need to be met to complete the lesson
    [SerializeField]
    private FunctionOption[] conditions;

    // Audio that is being played when Lesson is completed
    [SerializeField]
    private AudioClip congratulation;

    public AudioClip getIntro()
    {
        return intro;
    }

    public Instruction[] getInstructions()
    {
        return instructions;
    }

    public VideoClip getInstructionVideo()
    {
        return instructionVideo;
    }

    public FunctionOption[] getConditions()
    {
        return conditions;
    }
    
    public AudioClip getCongratulation()
    {
        return congratulation;
    }
}
