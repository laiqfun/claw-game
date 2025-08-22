using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip buttonClip;
    public AudioClip clawClip;
    public AudioClip coinClip;
    public AudioClip greatClip;
    public AudioClip successClip;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayButtonClip()
    {
        AudioSource.PlayClipAtPoint(buttonClip, Camera.main.transform.position);
    }

    public void PlayClawClip()
    {
        AudioSource.PlayClipAtPoint(clawClip, Camera.main.transform.position);
    }

    public void PlayCoinClip()
    {
        AudioSource.PlayClipAtPoint(coinClip, Camera.main.transform.position);
    }

    public void PlayGreatClip()
    {
        AudioSource.PlayClipAtPoint(greatClip, Camera.main.transform.position);
    }
    
    public void PlaySuccessClip()
    {
        AudioSource.PlayClipAtPoint(successClip, Camera.main.transform.position);
    }
}
