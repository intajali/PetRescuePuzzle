using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public enum ClipTypes
{
    STORY,
    MENU,
    GAMEPLAY,
    BOMB,
    GUN,
    FIRE,
    BUTTON_CLICK,
    TILE_CLEAR,
    LEVELCOMPLETE,
    LEVELFAILED,
    BUY,
    PISTOL,
    RIFLE,
    BUBBLE_GUN,
    
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource bgAudioSource;
    public AudioSource commonAudioSource;

    [Header("AudioClips")]
    public AudioClipDataContainer clipDataContainer;


    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private bool isMuted = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---------------- INIT ----------------

    public void ApplySavedSettings(float music, float sfx, bool mute)
    {
        musicVolume = Mathf.Clamp01(music);
        sfxVolume = Mathf.Clamp01(sfx);
        isMuted = mute;

        bgAudioSource.volume = mute ? 0f : musicVolume;
        commonAudioSource.volume = mute ? 0f : 1f;
    }

    /// <summary>
    /// Play Background Music
    /// </summary>
    /// <param name="clipTypes"></param>
    public void PlayBGMusic(ClipTypes clipTypes)
    {
        if (isMuted) return;

        ClipData clipData = clipDataContainer.GetClipByType(clipTypes);
        if (clipData == null || clipData.clip == null) return;

        bgAudioSource.Stop();
        bgAudioSource.clip = clipData.clip;
        bgAudioSource.loop = true;
        bgAudioSource.volume = musicVolume;
        bgAudioSource.Play();
    }


   

    /// <summary>
    /// Set Music Volume
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (!isMuted)
            bgAudioSource.volume = musicVolume;
    }

    // ---------------- SFX ----------------

    public void PlayCommonClip(ClipTypes clipTypes)
    {
        if (isMuted) return;

        ClipData clipData = clipDataContainer.GetClipByType(clipTypes);
        if (clipData == null || clipData.clip == null) return;

        commonAudioSource.PlayOneShot(clipData.clip, sfxVolume);
    }


    /// <summary>
    /// Set SFX Volume
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }


  

    // ---------------- MUTE ----------------

    public void SetMute(bool mute)
    {
        isMuted = mute;

        bgAudioSource.volume = mute ? 0f : musicVolume;
        commonAudioSource.volume = mute ? 0f : 1f;
    }

}
