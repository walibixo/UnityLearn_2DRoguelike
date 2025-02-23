using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, bool randomPitch = false, float volumeScale = 1.0f)
    {
        if (_audioSource.isPlaying)
        {
            volumeScale *= 0.2f;
        }

        if (randomPitch)
        {
            _audioSource.pitch = (float)Math.Pow(1.059463f, UnityEngine.Random.Range(-6, 6));
        }

        _audioSource.PlayOneShot(clip, volumeScale);
    }
}