using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField] VideoClip NextVideo;
    [SerializeField] GameObject RTObject,SkipButton;

    [SerializeField] int index = 1;
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // Add an event listener for when the video finishes playing
        videoPlayer.loopPointReached += EndReached;
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        if(index==0)
        {
            SkipButton.SetActive(PlayerPrefs.GetInt("FirstTime?", 0) == 1);
            PlayerPrefs.SetInt("FirstTime?", 1);
            videoPlayer.clip = NextVideo;
            videoPlayer.Play();
            index = 1;
        }
        else
        {
            //RTObject.SetActive(false);
        }
        // Do something when the video finishes
        Debug.Log("Video has ended!");
    }

    public void LoadLevel(int i)
    {
        SceneManager.LoadSceneAsync(i);
    }

    public void Skip()
    {
        videoPlayer.Stop();
        RTObject.SetActive(false);
    }
}
