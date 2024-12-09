using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Cards card;

    public Text NameText;
    public Text DescriptionText;

    public Image ArtworkImage;

    public Text ManaText;
    public Text AttackText;
    public Text healthText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NameText.text = card.Name;
        DescriptionText.text = card.Description;

        ArtworkImage.sprite = card.Artwork;

        ManaText.text = card.ManaCost.ToString();
        AttackText.text = card.Attack.ToString();
        healthText.text = card.Health.ToString();
    }
}
