using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class AudioData
{
    public string m_name;
    public AudioClip m_audioClip;

    public bool m_looping = false;

    [Range(0f, 1f)]
    public float m_volume = 1;

    [Range(0f, 1f)]
    public float m_spatial_blend = 1;

    [Range(0.3f, 3f)]
    public float m_pitch = 1;

    [Range(0f, 5f)]
    public float m_fadeInSpeed = 1;

    [Range(0f, 5f)]
    public float m_fadeOutSpeed = 1;

    public bool m_fade = false;

    [HideInInspector]
    public GameObject m_object;

    [HideInInspector]
    public AudioSource m_audioSource;
}