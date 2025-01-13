using NaughtyAttributes;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
	[SerializeField] AudioSource seSource;

	[SerializeField, Foldout("Audio Settings")] AudioMixer audioMixer;
	[SerializeField, Foldout("Audio Settings")] Slider masterSlider;
	[SerializeField, Foldout("Audio Settings")] Slider soundEffectSlider;
	[SerializeField, Foldout("Audio Settings")] Slider musicSlider;

	[Header("Singleton")]
	public bool dontDestroyOnLoad = true;
	public static AudioManager Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this.gameObject);

		if (dontDestroyOnLoad)
			DontDestroyOnLoad(this.gameObject);
	}

	private void Start()
	{
		LoadVolumeSettings();
	}

	public void PlaySoundEffect(SoundEffect seToPlay)
	{
		seSource.spatialBlend = 0;
		seSource.pitch = seToPlay.pitch;
		seSource.outputAudioMixerGroup = seToPlay.audioGroup;

		AudioClip clip = seToPlay.GetClip();

		if (clip != null)
			seSource.PlayOneShot(clip, seToPlay.volume);
	}

	public void PlaySoundEffect(SoundEffect seToPlay, GameObject objToPlayFrom)
	{
		AudioSource objSource = objToPlayFrom.GetComponent<AudioSource>();

	
		if (objSource == null)
			objSource = objToPlayFrom.AddComponent<AudioSource>();

		objSource.volume = seToPlay.volume;
		objSource.pitch = seToPlay.pitch;
		objSource.spatialBlend = seToPlay.spatialBlend;
		objSource.outputAudioMixerGroup = seToPlay.audioGroup;

		AudioClip clip = seToPlay.GetClip();

		if (clip != null)
			objSource.PlayOneShot(clip);
	}

	// Set Master Volume with logarithmic scale (0.0001 to 1 range)
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
				SetMasterSlider(masterVolume);
			}
			catch (Exception e)
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
				SetSfxSlider(soundFxVolume);
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
				SetMusicSlider(musicVolume);
			}
			catch (Exception e)
			{
				Debug.Log($"{e.Message}: Sliders prob not loaded yet");
			}
		}
	}
	public void SetMasterSlider(float Master)
	{
		masterSlider.value = Master;
	}
	public void SetSfxSlider(float Sfx)
	{
		soundEffectSlider.value = Sfx;
	}
	public void SetMusicSlider(float Music)
	{
		musicSlider.value = Music;
	}
}