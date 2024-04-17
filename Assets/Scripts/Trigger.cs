using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    // Event that is triggered when Player has entered the collider
    public event Action OnPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        Debug.Log("Event Invoked");

        OnPlayerEnter?.Invoke();

        gameObject.SetActive(false);
    }
}
