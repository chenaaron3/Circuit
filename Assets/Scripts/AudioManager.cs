using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip[] clips;
    AudioSource[] sources;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        sources = new AudioSource[clips.Length];
        for (int j = 0; j < clips.Length; j++)
        {
            sources[j] = gameObject.AddComponent<AudioSource>();
            sources[j].clip = clips[j];
        }
    }

    public void PlaySound(string name)
    {
        if (name.Equals("Complete"))
        {
            MuteAll();
        }
        foreach (AudioSource source in sources)
        {
            if (source.clip.name.Equals(name))
            {
                source.Play();
                return;
            }
        }
    }

    public void MuteAll()
    {
        foreach (AudioSource source in sources)
        {
            source.Stop();
        }
    }
}
