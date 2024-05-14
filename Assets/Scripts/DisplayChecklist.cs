using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayChecklist : MonoBehaviour
{
    // TextMeshPro Component
    [SerializeField]
    private TMP_Text list;

    public void DisplayList(string[] tasks)
    {
        var checklist = "";

        foreach (var task in tasks)
        {
            checklist += "\u2022 " + task + "\n";
        }

        list.text = checklist;
    }

    public void ResetList()
    {
        list.text = "";
    }
}
