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
        //Debug.Log("Trigger - Waypoint Entered");

        if (!other.gameObject.CompareTag("Player"))
            return;

        OnPlayerEnter?.Invoke();

        gameObject.SetActive(false);
    }
}
