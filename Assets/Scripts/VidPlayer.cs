using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] string videoFieldName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayVideo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayVideo(){
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        if(videoPlayer){
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFieldName);
            Debug.Log(videoPath);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }
    
}
