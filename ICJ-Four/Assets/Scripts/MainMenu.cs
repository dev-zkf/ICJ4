using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Scene] public int scene;
    public AudioClip onClickSFX;
    public AudioClip onHoverSFX;
    public void PlayGame()
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void playEnterSFX()
    {
		SoundFXManagergerg.Instance.PlaySoundFXClip(onClickSFX, transform, 1f);
	}
    public void playHoverSFX()
    {
		SoundFXManagergerg.Instance.PlaySoundFXClip(onHoverSFX, transform, 1f);
	}
}
