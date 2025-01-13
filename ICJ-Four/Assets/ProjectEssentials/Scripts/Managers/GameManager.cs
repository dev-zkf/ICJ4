using UnityEngine;

public class GameManager : MonoBehaviour
{

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