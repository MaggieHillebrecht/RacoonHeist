using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class SettingAnimationVideo : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject videoPanel;       
    public VideoPlayer videoPlayer;
    public RawImage backgroundImage;       

    [Header("Video Clips")]
    public VideoClip forwardVideo;
    public VideoClip reverseVideo;

    private Coroutine backgroundCoroutine;
    private bool isPlayingForward = true;

    public void PlayForward()
    {
        isPlayingForward = true;
        PlayVideo(forwardVideo);
    }

    public void Exit()
    {
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        isPlayingForward = false;
        PlayVideo(reverseVideo);
    }

    private void PlayVideo(VideoClip clip)
    {        
        if (videoPanel != null)
            videoPanel.SetActive(true);

        StartCoroutine(PlayVideoNextFrame(clip));
    }

    private IEnumerator PlayVideoNextFrame(VideoClip clip)
    {
        yield return null; // wait one frame so VideoPlayer is fully enabled

        if (videoPlayer != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.Stop();
            videoPlayer.Play();

            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.loopPointReached += OnVideoFinished;

            if (backgroundCoroutine != null)
                StopCoroutine(backgroundCoroutine);

            if (isPlayingForward)
                backgroundCoroutine = StartCoroutine(ShowBackgroundWithDelay(2.3f));
        }
    }

    private IEnumerator ShowBackgroundWithDelay(float secondsBeforeEnd)
    {
        while (!videoPlayer.isPrepared)
            yield return null;

        double delay = videoPlayer.length - secondsBeforeEnd;

        if (delay < 0)
            delay = 0;

        yield return new WaitForSeconds((float)delay);

        if (isPlayingForward && backgroundImage != null)
            backgroundImage.gameObject.SetActive(true);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (isPlayingForward)
        {
            if (backgroundImage != null)
                backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            if (videoPanel != null)
                videoPanel.SetActive(false);
        }

        vp.loopPointReached -= OnVideoFinished;
    }
}
