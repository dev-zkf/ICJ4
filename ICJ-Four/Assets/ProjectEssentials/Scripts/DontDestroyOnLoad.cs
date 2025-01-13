using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
	public bool dontDestroyOnLoad = true;
	public static DontDestroyOnLoad Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this.gameObject);

		if (dontDestroyOnLoad)
			DontDestroyOnLoad(this.gameObject);
	}
}
