using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSoundScript : MonoBehaviour
{
    public AudioClip[] clips;

    public void playSound (int clip)
    {
        GetComponent<AudioSource>().clip = clips[clip];
        GetComponent<AudioSource>().Play();
    }
}
