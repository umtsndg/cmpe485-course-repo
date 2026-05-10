using UnityEngine;
using UnityEngine.Video;

public class MainMenuVideoController : MonoBehaviour
{
    [Header("Video Player")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Videos")]
    [SerializeField] private VideoClip introVideo;
    [SerializeField] private VideoClip menuLoopVideo;

    [Header("UI")]
    [SerializeField] private GameObject menuUI;

    private void Start()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }

        PlayIntroVideo();
    }

    private void Update()
    {
        if (menuUI != null && !menuUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
            {
                videoPlayer.loopPointReached -= OnIntroFinished;
                OnIntroFinished(videoPlayer);
            }
        }
    }

    private void PlayIntroVideo()
    {
        videoPlayer.loopPointReached -= OnIntroFinished;
        videoPlayer.loopPointReached += OnIntroFinished;

        videoPlayer.isLooping = false;
        videoPlayer.clip = introVideo;
        videoPlayer.Play();
    }

    private void OnIntroFinished(VideoPlayer source)
    {
        videoPlayer.loopPointReached -= OnIntroFinished;

        PlayMenuLoopVideo();

        if (menuUI != null)
        {
            menuUI.SetActive(true);
        }
    }

    private void PlayMenuLoopVideo()
    {
        videoPlayer.isLooping = true;
        videoPlayer.clip = menuLoopVideo;
        videoPlayer.Play();
    }
}