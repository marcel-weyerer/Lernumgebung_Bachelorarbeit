using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerAnimation(string trigger)
    {
        animator.SetTrigger(trigger);
    }
}
