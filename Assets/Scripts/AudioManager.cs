using GameEvents;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip cardFlipSFX;
    [SerializeField] private AudioClip paperTornSFX;
    [SerializeField] private AudioClip buttonClickSFX;

    private void OnEnable()
    {
        StartCoroutine(InitializeRoutine());

        // Events Subscription
        GameEventBus.Subscribe<ButtonClickEvent>(OnButtonClicked);
        GameEventBus.Subscribe<CardFlippedEvent>(OnCardFlipped);

        GameEventBus.Subscribe<MusicVolumeChangedEvent>(OnMusicVolumeChanged);
        GameEventBus.Subscribe<SoundVolumeChangedEvent>(OnSoundVolumeChanged);
    }

    IEnumerator InitializeRoutine()
    {
        yield return new WaitForEndOfFrame();
        GetSavedPlayerSettings();

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    private void GetSavedPlayerSettings()
    {
        var soundVolume = PlayerPrefs.GetFloat(UserDataManager.UserDataKeys.SoundVolume, 0.5f);
        mixer.SetFloat("SoundVolume", Mathf.Log10(Mathf.Clamp(soundVolume, 0.0001f, 1)) * 20f);
        Debug.Log("Loading Sound: " + soundVolume);

        var musicVolume = PlayerPrefs.GetFloat(UserDataManager.UserDataKeys.MusicVolume, 0.5f);
        mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(musicVolume, 0.0001f, 1)) * 20f);
        Debug.Log("Loading music: " + musicVolume);
    }

    private void OnSoundVolumeChanged(SoundVolumeChangedEvent @event)
    {
        mixer.SetFloat("SoundVolume", Mathf.Log10(Mathf.Clamp(@event.Volume, 0.0001f, 1)) * 20f);
        PlayerPrefs.SetFloat(UserDataManager.UserDataKeys.SoundVolume, @event.Volume);
        Debug.Log("Saving Sound: " + @event.Volume);
    }

    private void OnMusicVolumeChanged(MusicVolumeChangedEvent @event)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(@event.Volume, 0.0001f, 1)) * 20f);
        PlayerPrefs.SetFloat(UserDataManager.UserDataKeys.MusicVolume, @event.Volume);
        Debug.Log("Saving Music: " + @event.Volume);
    }

    private void OnCardFlipped(CardFlippedEvent @event)
    {
        PlaySFX(cardFlipSFX);
        PlaySFX(paperTornSFX);
    }

    private void OnButtonClicked(ButtonClickEvent @event)
    {
        PlaySFX(buttonClickSFX);
    }

    private void OnDisable()
    {
        // Events Unsubscription
        GameEventBus.Unsubscribe<ButtonClickEvent>(OnButtonClicked);
        GameEventBus.Unsubscribe<CardFlippedEvent>(OnCardFlipped);

        GameEventBus.Unsubscribe<MusicVolumeChangedEvent>(OnMusicVolumeChanged);
        GameEventBus.Unsubscribe<SoundVolumeChangedEvent>(OnSoundVolumeChanged);
    }

    public void PlaySFX(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }
}
