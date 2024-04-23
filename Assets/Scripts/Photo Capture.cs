using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PhotoCapture : MonoBehaviour
{
    [SerializeField]
    private Image photoDisplayArea;

    [SerializeField]
    private GameObject photo;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip clip;

    [SerializeField]
    private Camera photoCamera;

    // Texture of photo
    private Texture2D renderResult;

    // Flag for taking a photo
    private bool takePhoto;

    // Start is called before the first frame update
    void Start()
    {
        takePhoto = false;

        GetComponent<XRGrabInteractable>().activated.AddListener(StartPictureCapture);
    }

    // Update is called once per frame
    void Update()
    {
        if (takePhoto)
        {
            takePhoto = false;
            StartCoroutine(CapturePhoto());
        }
    }

    private void StartPictureCapture(ActivateEventArgs args)
    {
        takePhoto = true;

        source.clip = clip;
        source.Play();
    }

    /*private void StartPictureCapture()
    {
        takePhoto = true;

        source.clip = clip;
        source.Play();
    }*/

    private IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        // Create screenshot objects
        RenderTexture renderTexture = RenderTexture.GetTemporary(photoCamera.pixelWidth, photoCamera.pixelHeight, 24);
        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
        renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);

        // Prepare Screenshot
        RenderTexture currentTexture = photoCamera.targetTexture;
        photoCamera.targetTexture = renderTexture;

        // Manually render scene
        photoCamera.Render();

        // Read pixels of screenshot
        RenderTexture.active = renderTexture;
        renderResult.ReadPixels(rect, 0, 0);
        renderResult.Apply();

        // Reset active camera texture and render texture
        RenderTexture.ReleaseTemporary(renderTexture);
        RenderTexture.active = null;
        photoCamera.targetTexture = currentTexture;

        ShowPhoto();
    }

    private void ShowPhoto()
    {
        // Unity UI needs a sprite to show the photo
        Sprite photoSprite = Sprite.Create(renderResult, new Rect(0, 0, renderResult.width, renderResult.height), new Vector2(0.5f, 0.5f), 100f);
        photoDisplayArea.sprite = photoSprite;

        photo.GetComponent<Animator>().SetTrigger("StartFade");
    }
}
