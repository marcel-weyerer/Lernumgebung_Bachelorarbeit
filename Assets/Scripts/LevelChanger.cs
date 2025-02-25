using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    private Animator animator;

    private string levelToLoad;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeToLevel(string levelName)
    {
        // Fade out view before switching scene
        levelToLoad = levelName;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
