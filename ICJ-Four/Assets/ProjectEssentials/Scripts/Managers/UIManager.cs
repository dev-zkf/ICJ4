using UnityEngine;

public class UIManager : MonoBehaviour
{
	[Header("Main Panels")]
	[SerializeField] GameObject Panel_MainMenu;
	[SerializeField] GameObject Panel_HUD;

	[Header("Singleton")]
	public bool dontDestroyOnLoad = true;

	public static UIManager Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this.gameObject);

		if (dontDestroyOnLoad)
			DontDestroyOnLoad(this.gameObject);
	}

	public void ToggleMainMenuPanel(bool buh)
	{
		Panel_MainMenu.SetActive(buh);
		Panel_HUD.SetActive(!buh);
	}
}