using UnityEngine;
using NaughtyAttributes;

public class LevelChangeTrigger : MonoBehaviour
{
	[SerializeField] public Levels LevelToChangeTo;

	public void IntializeLevelChange()
	{
		LevelManager.Instance.ChangeLevel(LevelToChangeTo);
	}
}
