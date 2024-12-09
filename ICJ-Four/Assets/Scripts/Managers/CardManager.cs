using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<CardSlot> deck = new List<CardSlot>();
    public List<CardSlot> discardPile = new List<CardSlot>();
    public List<CardSlot> playedPile = new List<CardSlot>();
    public List<CardSlot> Hand = new List<CardSlot>();
    public Transform[] cardSlots;
    public Transform[] placeableSlots;
    public bool[] availableSlots;
    public bool[] availablePlaceableSlots;
    public TMP_Text deckSizeText;
    public TMP_Text discardSizeText;
	[Foldout("Settings")] public float onHoverPopAmount;
	[Foldout("Settings")] public float onPlayPopAmount;
	void Start()
    {
        
    }

    void Update()
    {
        if (deckSizeText != null)
            deckSizeText.text = deck.Count.ToString();
        if (discardSizeText != null)
            discardSizeText.text = discardPile.Count.ToString();
    }
    public void DrawCard()
    {
        if (deck.Count >= 1)
        {
            CardSlot randCard = deck[Random.Range(0,deck.Count)];
            for (int i = 0; i < availableSlots.Length; i++)
            {
                if (availableSlots[i] == true)
                {
                    randCard.gameObject.SetActive(true); // incase its disabled
                    randCard.handIndex = i;

                    randCard.transform.SetParent(cardSlots[i]);
                    randCard.transform.localPosition = Vector3.zero;
                    randCard.transform.localScale = Vector3.one;
                    randCard.hasBeenPlayed = false;

                    availableSlots[i] = false;
                    Hand.Add(randCard);
                    deck.Remove(randCard);
                    return;
                }
            }
        }
    } 
    public void Shuffle()
    {
        if (discardPile.Count >= 1)
        {
            foreach(CardSlot card in discardPile)
            {
                deck.Add(card);
            }
            discardPile.Clear();
        }
    }
}
