using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayChecklist : MonoBehaviour
{
    // TextMeshPro Component
    [SerializeField]
    private TMP_Text list;

    // Audio Source for writing sound effect
    [SerializeField]
    private AudioSource audioSource;

    // Audio Clip for showing checklist
    [SerializeField]
    private AudioClip showClip;

    // Audio Clip for hiding checklist
    [SerializeField]
    private AudioClip hideClip;

    public void DisplayList(string[] tasks)
    {
        string checklist = "";

        // Combine all strings to a bullet point list
        foreach (var task in tasks)
        {
            checklist += "\u2022 " + task + "\n\n";
        }

        // remove trailing newline
        checklist = checklist.TrimEnd();

        list.text = checklist;

        audioSource.clip = showClip;
        audioSource.Play();
    }

    public void ResetList()
    {
        list.text = "";

        audioSource.clip = hideClip;
        audioSource.Play();
    }
}
