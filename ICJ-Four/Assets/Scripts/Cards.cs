using UnityEngine;
using NaughtyAttributes;
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Cards : ScriptableObject
{
    public new string Name;
    public string Description;

    public Sprite Artwork;

    public bool ManaCard;
    public int ManaCost;
    [DisableIf("ManaCard")] public int Attack;
	[DisableIf("ManaCard")] public int Health;


    public void Print()
    {
        Debug.Log(name + ": " + Description + " The card costs: " + ManaCost);
    }

}
