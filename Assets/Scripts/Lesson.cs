using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class Lesson
{
    // Introduction to the new lesson. What is the lesson about?
    [SerializeField]
    private AudioClip intro;

    // Instruction für the lesson
    [SerializeField]
    private AudioClip instructionAudio;

    // Video that shows how the lesson can be completed
    [SerializeField]
    private VideoClip instructionVideo;

    // Render Texture necessary for playing the instruction video
    [SerializeField]
    private RenderTexture renderTexture;

    Lesson(AudioClip intro, AudioClip instructionAudio, VideoClip insturctionVideo)
    {
        this.intro = intro;
        this.instructionAudio = instructionAudio;
        this.instructionVideo = insturctionVideo;
    }

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

    public RenderTexture getRenderTexure()
    {
        return renderTexture;
    }
}
