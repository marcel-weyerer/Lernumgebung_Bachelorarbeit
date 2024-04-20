using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    // Event that is triggered when Player has entered the collider
    public event Action OnPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        OnPlayerEnter?.Invoke();

        gameObject.SetActive(false);
    }
}
