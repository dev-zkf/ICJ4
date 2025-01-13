using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	[Header("Level Data")]
	[SerializeField] private LevelData[] GameLevels;
	[SerializeField] private LevelData MainMenu;

	[Header("Singleton")]
	public bool dontDestroyOnLoad = true;
	public static LevelManager Instance { get; private set; }

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
	private void Start()
	{
		ChangeLevel(Levels.MainMenu);
	}
	private void OnLevelWasLoaded(int level)
	{
		if (level == MainMenu.levelIndex)
		{
			// buh, load Main Menu from ui manager
			UIManager.Instance.ToggleMainMenuPanel(true);
		} 
		else
		{
			// disable main menu panel from ui manager
			UIManager.Instance.ToggleMainMenuPanel(false);
		}
	}
	public void ChangeLevel(Levels newLevel)
	{
		if (newLevel == Levels.MainMenu)
		{
			SceneManager.LoadScene(MainMenu.levelIndex);
			return;
		}

		foreach (LevelData levelData in GameLevels)
		{
			if (levelData.level == newLevel)
			{
				SceneManager.LoadScene(levelData.levelIndex);
				return;
			}
		}
		Debug.LogWarning("ERROR: " + newLevel.ToString() + " was not found!");
	}
	public void QuitGame()
	{
		Application.Quit();
	}
}

[Serializable]
public class LevelData
{
	public Levels level;
	[Scene] public int levelIndex; 
}
public enum Levels
{
	MainMenu,
	Level01,
	Tutorial
}