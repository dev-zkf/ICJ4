using UnityEngine;
using UnityEngine.UI;

public class SetSliders : MonoBehaviour
{
	public static SetSliders instance;
	public Slider masterSlider;
	public Slider sfxSlider;
	public Slider musicSlider;

	public void Awake()
	{
		if (instance == null)
			instance = this;
	}
	public void SetMasterSlider(float a)
	{
		masterSlider.value = a;
	}
	public void SetSfxSlider(float a)
	{
		sfxSlider.value = a;
	}
	public void SetMusicSlider(float a)
	{
		musicSlider.value = a;
	}

}
