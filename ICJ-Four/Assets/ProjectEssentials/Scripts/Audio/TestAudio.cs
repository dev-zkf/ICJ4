using UnityEngine;

public class TestAudio : MonoBehaviour
{
    public SoundEffect onAttackSE;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PlaySoundEffect(onAttackSE, this.gameObject);
        }
    }
}
