using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PhotoCapture : MonoBehaviour
{
    private GameObject picture;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip enabledAudio;

    [SerializeField]
    private AudioClip disabledAudio;

    // Flag to enable taking photo
    private bool photoTakingEnabled;

    [SerializeField]
    private Camera photoCamera;

    // Texture of photo
    private Texture2D renderResult;

    // Start is called before the first frame update
    void Start()
    {
        photoTakingEnabled = false;

        GetComponent<XRGrabInteractable>().activated.AddListener(StartPictureCapture);
    }

    public void SetPicture(GameObject picture)
    {
        this.picture = picture;
    }

    public void SetPhotoTakingEnabled()
    {
        photoTakingEnabled = true;
    }

    private void StartPictureCapture(ActivateEventArgs args)
    {
        // When flag is set take a picture, otherwise just play disabledAudio sound
        if (photoTakingEnabled)
        {
            // Enable processing intensive camera
            photoCamera.gameObject.SetActive(true);

            // Capture Photo
            StartCoroutine(CapturePhoto());

            source.clip = enabledAudio;
        }
        else
        {
            source.clip = disabledAudio;
        }

        source.Play();
    }

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

        // Disable processing intensive camera
        photoCamera.gameObject.SetActive(false);

        // Unset photoTakingEnabled flag
        photoTakingEnabled = false;

        ShowPhoto();
    }

    private void ShowPhoto()
    {
        if (picture != null)
        {
            // Unity UI needs a sprite to show the photo
            Sprite photoSprite = Sprite.Create(renderResult, new Rect(0, 0, renderResult.width, renderResult.height), new Vector2(0.5f, 0.5f), 100f);
            picture.GetComponent<Image>().sprite = photoSprite;

            picture.GetComponent<Animator>().SetTrigger("StartFade");
        }
    }
}
