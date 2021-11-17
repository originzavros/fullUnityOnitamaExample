using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = (AudioManager)FindObjectOfType(typeof(AudioManager));
            if(instance == null)
            {
                instance = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public AudioSource MusicSource;
    public AudioSource SoundClips;

    public AudioClip startMove;
    public AudioClip endMove;

    public float clipVolume = .5f;

    void Start()
    {
        instance.PlayMusic();
    }

    public void PlayMusic()
    {
        if(MusicSource.isPlaying) return;
        MusicSource.Play();
    }

    public static void PlayClip(string clipId)
    {
        if(clipId == "startMove")
        {
            instance.SoundClips.PlayOneShot(instance.startMove, instance.clipVolume);
        }
        if(clipId == "endMove")
        {
            instance.SoundClips.PlayOneShot(instance.endMove, instance.clipVolume);
        }
    }

}
