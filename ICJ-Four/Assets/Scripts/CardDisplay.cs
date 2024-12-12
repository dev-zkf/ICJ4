using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Expandable]
    public Cards card;

    public TMP_Text NameText;
    public TMP_Text DescriptionText;

    public Image ArtworkImage;
    public Image ArtBackground;
    public Image Background;
    public Image Nametag;
    public Image DescriptionBox;

    public TMP_Text ManaText;
    public TMP_Text AttackText;
    public TMP_Text healthText;
    public TMP_Text statsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            NameText.text = card.Name;
            DescriptionText.text = card.Description;

            ArtworkImage.sprite = card.Artwork;
            ArtBackground.sprite = card.ArtBackground;
            Background.sprite = card.Background;
            Nametag.sprite = card.Nametag;
            DescriptionBox.sprite = card.DescriptionBox;

            ManaText.text = card.ManaCost.ToString();
            AttackText.text = card.Attack.ToString();
            healthText.text = card.Health.ToString();
            statsText.text = $"{card.Attack.ToString()}/{card.Health.ToString()}";
        }
        catch(System.Exception e)
        {
            Debug.Log($"{e}: bruh fine dont work then");
        }
    }
}
