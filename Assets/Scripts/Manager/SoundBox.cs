using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundBox : MonoBehaviour
{
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public AudioClip[] preLoadClips;
}
