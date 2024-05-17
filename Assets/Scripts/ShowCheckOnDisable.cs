using System;
using UnityEngine;
using UnityEngine.UI;

public class ShowCheckOnDisable : MonoBehaviour
{
    // Button this checkmark belongs to
    [SerializeField]
    private Button button;

    // Update is called once per frame
    void Update()
    {
        if (button.interactable)
            GetComponent<Image>().enabled = false;
        else
            GetComponent<Image>().enabled = true;
    }
}
