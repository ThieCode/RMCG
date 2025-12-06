using GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Button closeBtn;

    private void OnEnable()
    {
        GetSavedPlayerSettings();
        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanges);
        soundSlider.onValueChanged.AddListener(OnSoundSliderValueChanges);
        closeBtn.onClick.AddListener(OnPauseMenuCloseBtnPressed);
    }

    private void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveListener(OnMusicSliderValueChanges);
        soundSlider.onValueChanged.RemoveListener(OnSoundSliderValueChanges);
        closeBtn.onClick.RemoveListener(OnPauseMenuCloseBtnPressed);
    }

    private void OnPauseMenuCloseBtnPressed()
    {
        GameEventBus.Raise(new PauseMenuCloseEvent());
        GameEventBus.Raise(new ButtonClickEvent());
    }

    private void GetSavedPlayerSettings()
    {
        var soundVolume = PlayerPrefs.GetFloat(UserDataManager.UserDataKeys.SoundVolume, 0.5f);
        soundSlider.SetValueWithoutNotify(soundVolume);
        var musicVolume = PlayerPrefs.GetFloat(UserDataManager.UserDataKeys.MusicVolume, 0.5f);
        musicSlider.SetValueWithoutNotify(musicVolume);
    }

    private void OnSoundSliderValueChanges(float arg0)
    {
        GameEventBus.Raise(new SoundVolumeChangedEvent(arg0));
    }

    private void OnMusicSliderValueChanges(float arg0)
    {
        GameEventBus.Raise(new MusicVolumeChangedEvent(arg0));
    }
}
