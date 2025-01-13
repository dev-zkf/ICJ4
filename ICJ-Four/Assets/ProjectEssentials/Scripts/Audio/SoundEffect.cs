using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundEffect
{
	public AudioClip[] clips;
	public AudioMixerGroup audioGroup;
	public float volume = 0.5f;
	public float pitch = 1f;
	public float spatialBlend = 1f;

	public AudioClip GetClip()
	{
		return clips[Random.Range(0, clips.Length)];
	}
}