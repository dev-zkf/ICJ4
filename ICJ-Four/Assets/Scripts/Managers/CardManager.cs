using NaughtyAttributes;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
	public static CardManager instance;

	[Header("Card Collections")]
	public List<CardSlot> deck = new List<CardSlot>();
	public List<CardSlot> discardPile = new List<CardSlot>();
	public List<CardSlot> playedPile = new List<CardSlot>();
	public List<CardSlot> Hand = new List<CardSlot>();
	public List<CardSlot> aiHand = new List<CardSlot>();

	[Header("Player and AI Slots")]
	public Transform[] cardSlots;
	public Transform[] aiSlots;

	public Transform[] placeableSlots;
	public bool[] availableSlots;
	public bool[] availablePlaceableSlots;

	public Transform[] AI_placeableSlots;
	public bool[] AI_availableSlots;
	public bool[] AI_availablePlaceableSlots;

	[Header("UI Elements")]
	public TMP_Text deckSizeText;
	public TMP_Text discardSizeText;

	[Header("Settings")]
	[Foldout("Settings")] public float onHoverPopAmount;
	[Foldout("Settings")] public float onPlayPopAmount;
	[Foldout("Settings")] public float UpAmount;

	private void Awake()
	{
		if (instance == null)
			instance = this;
	}

	private void Update()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		if (deckSizeText != null) deckSizeText.text = deck.Count.ToString();
		if (discardSizeText != null) discardSizeText.text = discardPile.Count.ToString();
	}

	public void DrawCard(bool isPlayer)
	{
		if (deck.Count < 1 || GameManager.instance.draws <= 0) return;
		if (isPlayer && !GameManager.instance.isPlayerTurn) return;
		if (!isPlayer && GameManager.instance.isPlayerTurn) return;
		CardSlot randCard = deck[Random.Range(0, deck.Count)];
		Transform[] slots = isPlayer ? cardSlots : aiSlots;
		bool[] slotAvailability = isPlayer ? availableSlots : AI_availableSlots;
		List<CardSlot> hand = isPlayer ? Hand : aiHand;

		for (int i = 0; i < slotAvailability.Length; i++)
		{
			if (slotAvailability[i] && GameManager.instance.draws > 0)
			{
				GameManager.instance.draws--;
				AssignCardToSlot(randCard, slots[i], i, hand, slotAvailability, isPlayer);
				deck.Remove(randCard);
				return;
			}
		}
	}

	private void AssignCardToSlot(CardSlot card, Transform slot, int index, List<CardSlot> hand, bool[] slotAvailability, bool isPlayer)
	{
		card.gameObject.SetActive(true);
		card.played = false;
		card.handIndex = index;
		card.owner = isPlayer ? CardSlot.Owner.Player : CardSlot.Owner.AI;
		card.transform.SetParent(slot);
		card.transform.localPosition = Vector3.zero;
		card.transform.localScale = Vector3.one;

		slotAvailability[index] = false;
		hand.Add(card);
	}

	public void Shuffle()
	{
		if (discardPile.Count < 1) return;

		foreach (CardSlot card in discardPile)
		{
			deck.Add(card);
		}
		discardPile.Clear();
	}
}
