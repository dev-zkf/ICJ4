using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Expandable]
    public Cards card;

	public TMP_Text NameText;
	public Image CardImage;
	[DisableIf("isMana")] public Image Nametag;
	[DisableIf("isMana")] public Image ArtworkImage;
	[DisableIf("isMana")] public Image ArtworkBackground;
	[DisableIf("isMana")] public TMP_Text StatsText;
    [EnableIf("isMana")] public Image Foreground;
    [EnableIf("isMana")] public Image ManaBox;
    [EnableIf("isMana")] public Image Border;
	public TMP_Text ManaText;

    public bool isMana = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            NameText.text = card.Name;

            ArtworkImage.sprite = card.Artwork;
            ArtworkBackground.sprite = card.ArtBackground;
            CardImage.sprite = card.CardImage;
            Nametag.sprite = card.Nametag;
            if (isMana)
            {
                Foreground.sprite = card.Foreground;
                ManaBox.sprite = card.ManaBox;
                Border.sprite = card.Border;
            }

            ManaText.text = card.ManaCost.ToString();
            StatsText.text = $"{card.Attack.ToString()}/{card.Health.ToString()}";
        }
        catch(System.Exception e)
        {
            Debug.Log($"{e}: bruh fine dont work then");
        }
    }
}
