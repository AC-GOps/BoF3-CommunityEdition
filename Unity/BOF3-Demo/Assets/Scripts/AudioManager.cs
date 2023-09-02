using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
           

public enum SFX
{
    Select,
    Back,
    Confirm,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip[] musicClips;
    public AudioClip[] effectClips;
    public AudioClip[] battleClips;
    public AudioClip[] hurtClips;

    public AudioSource backgroundMusic;
    public AudioSource soundEffect;
    public float musicVolume = 1f; // Volume for music
    [Range(0, 1)]
    public float sfxVolume = 1f;   // Volume for sound effects


    private int currentClipIndex;

    private void Awake()
    {
        // Create a singleton instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Set initial volumes
        backgroundMusic.volume = musicVolume;
        soundEffect.volume = sfxVolume;
    }

    // Play a music clip
    public void PlayMusic(int clipIndex)
    {
        backgroundMusic.DOFade(0, 1f).OnComplete(FadeMusicIn);
        currentClipIndex = clipIndex;

    }
    private void FadeMusicIn()
    {
        backgroundMusic.clip = musicClips[currentClipIndex];
        backgroundMusic.Play();
        backgroundMusic.DOFade(1, 1f);
    }

    // Play a sound effect clip
    public void PlaySFX(SFX clip)
    {
        soundEffect.PlayOneShot(effectClips[(int)clip]);
    }

    public void PlaySFXFromClip(AudioClip clip)
    {
        soundEffect.PlayOneShot(clip);
    }

    // Adjust music volume
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        backgroundMusic.volume = musicVolume;
    }

    // Adjust sound effects volume
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        soundEffect.volume = sfxVolume;
    }
}
