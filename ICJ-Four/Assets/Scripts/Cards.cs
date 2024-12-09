using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Cards : ScriptableObject
{
    public new string Name;
    public string Description;

    public Sprite Artwork;

    public int ManaCost;
    public int Attack;
    public int Health; 


    public void Print()
    {
        Debug.Log(name + ": " + Description + " The card costs: " + ManaCost);
    }

}
