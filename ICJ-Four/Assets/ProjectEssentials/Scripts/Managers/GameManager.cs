using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Foldout("Hud Obj Refrences")] public TMP_Text firstPlayerName, firstPlayerHpCount, firstPlayerManaCount, secondPlayerName, secondPlayerHpCount, secondPlayerManaCount, gameState;
	[Foldout("Game Sfx")] public SoundEffect hoverSFX, clickSFX, discardSFX;
	public bool dontDestroyOnLoad = true;
	public static GameManager Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this.gameObject);

		if (dontDestroyOnLoad)
			DontDestroyOnLoad(this.gameObject);
	}
	void Update()
	{

	}
}