using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class Lesson
{
    // Enum for all available Functions for the conditions
    public enum FunctionOption
    {
        A_ButtonPress,
        RotateLeft,
        RotateRight
    };

    // Introduction to the new lesson. What is the lesson about?
    [SerializeField]
    private AudioClip intro;

    // Instruction für the lesson
    [SerializeField]
    private AudioClip instructionAudio;

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

    public AudioClip getInstructionAudio()
    {
        return instructionAudio;
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
