using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
	[SerializeField] public AudioMixer audioMixer;

	// Singleton instance
	public static SoundMixerManager Instance;

	private void Awake()
	{
		// Ensure only one instance of SoundMixerManager exists
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);  // Keep this object across scenes
		}
		else
		{
			Destroy(gameObject);  // Destroy any duplicates
		}

		// Load saved volume settings when the game starts or a new scene is loaded
		LoadVolumeSettings();
	}
	private void Start()
	{
		LoadVolumeSettings(); // load again ig??
	}

	// Set Master Volume with logarithmic scale (0 to 1 range)
	public void SetMasterVolume(float level)
	{
		audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
		PlayerPrefs.SetFloat("MasterVolume", level);  // Save setting to PlayerPrefs
		PlayerPrefs.Save();  // Ensure the setting is saved
	}

	// Set Sound FX Volume
	public void SetSoundFXVolume(float level)
	{
		audioMixer.SetFloat("SoundFxVolume", Mathf.Log10(level) * 20f);
		PlayerPrefs.SetFloat("SoundFxVolume", level);  // Save setting to PlayerPrefs
		PlayerPrefs.Save();  // Ensure the setting is saved
	}

	// Set Music Volume
	public void SetMusicVolume(float level)
	{
		audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
		PlayerPrefs.SetFloat("MusicVolume", level);  // Save setting to PlayerPrefs
		PlayerPrefs.Save();  // Ensure the setting is saved
	}

	// Load saved volume settings from PlayerPrefs
	private void LoadVolumeSettings()
	{
		if (PlayerPrefs.HasKey("MasterVolume"))
		{
			float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
			audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20f);
			try
			{
				SetSliders.instance.SetMasterSlider(masterVolume);
			}
			catch(Exception e)
			{
				Debug.Log($"{e.Message}: Sliders prob not loaded yet");
			}
		}

		if (PlayerPrefs.HasKey("SoundFxVolume"))
		{
			float soundFxVolume = PlayerPrefs.GetFloat("SoundFxVolume");
			audioMixer.SetFloat("SoundFxVolume", Mathf.Log10(soundFxVolume) * 20f);
			try
			{
				SetSliders.instance.SetSfxSlider(soundFxVolume);
			}
			catch (Exception e)
			{
				Debug.Log($"{e.Message}: Sliders prob not loaded yet");
			}
		}

		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
			audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20f);
			try
			{
				SetSliders.instance.SetMusicSlider(musicVolume);
			}
			catch (Exception e)
			{
				Debug.Log($"{e.Message}: Sliders prob not loaded yet");
			}
		}
	}
}
