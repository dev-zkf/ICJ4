using System.Collections;
using TMPro;
using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public TMP_Text ManaText;
	public TMP_Text AiManaText;
	public TMP_Text DrawCount;
	public TMP_Text DiscardCount;

	public static GameManager instance;

	[SerializeField, Foldout("Settings")] private int drawsPerTurn = 5;
	[SerializeField, Foldout("Settings")] private int discardsPerTurn = 5;
	[SerializeField, Foldout("Settings")] public int discardMana = 2;

	[Foldout("Player Settings")] public int playerHealth = 20;
	[Foldout("Player Settings")] public int playerMana = 10;

	[Foldout("Ai Settings")] public int aiHealth = 20;
	[Foldout("Ai Settings")] public int aiMana = 10;

	public bool isPlayerTurn = true;

	public int draws;
	public int discards;

	private AiStates aiState;

	// Track the cards played by the player and AI
	public List<CardSlot> playerCardsOnField = new List<CardSlot>();
	public List<CardSlot> aiCardsOnField = new List<CardSlot>();

	bool playerDone = false;
	bool aiDone = false;

	private void Awake()
	{
		if (instance == null) instance = this;
	}

	private void Start()
	{
		draws = drawsPerTurn;
		discards = discardsPerTurn;
		InitializeGame();
		StartTurn();
	}

	private void FixedUpdate()
	{
		UpdateUI();
		HandleGameStates();
	}

	private void InitializeGame()
	{
		Debug.Log("Game initialized");
	}

	private void StartTurn()
	{
		draws = drawsPerTurn;
		discards = discardsPerTurn;

		if (isPlayerTurn)
		{
			Debug.Log("Player's turn");
		}
		else
		{
			aiState = AiStates.Draw;
			StartCoroutine(HandleAiTurn());
			Debug.Log("AI's turn");
		}
	}

	private void EndTurn()
	{
		isPlayerTurn = !isPlayerTurn;
		StartTurn();
	}

	private void HandleGameStates()
	{
		if (CardManager.instance.deck.Count == 0)
			CardManager.instance.Shuffle();

		if (draws == 0 && CardManager.instance.Hand.Count == 0 && isPlayerTurn)
		{
			EndTurn();
			playerDone = true;
		}

		// Clear previous card lists
		playerCardsOnField.Clear();
		aiCardsOnField.Clear();

		// Sort cards into player and AI lists
		foreach (var cardSlot in CardManager.instance.playedPile)
		{
			if (cardSlot.owner == CardSlot.Owner.Player)
			{
				playerCardsOnField.Add(cardSlot);
			}
			else if (cardSlot.owner == CardSlot.Owner.AI)
			{
				aiCardsOnField.Add(cardSlot);
			}
		}

		// Resolve battles if both sides have cards
		if (playerCardsOnField.Count > 0 && aiCardsOnField.Count > 0 && playerDone && aiDone)
		{
			ResolveBattles();
		}
	}
	private void ResolveBattles()
	{

	}
	private void UpdateUI()
	{
		ManaText.text = playerMana.ToString();
		AiManaText.text = aiMana.ToString();
		DrawCount.text = draws.ToString();
		DiscardCount.text = discards.ToString();
	}

	private IEnumerator HandleAiTurn()
	{
		float delay = 0.4f;

		while (!isPlayerTurn)
		{
			switch (aiState)
			{
				case AiStates.Draw:
					while (draws > 0)
					{
						CardManager.instance.DrawCard(false);
						Debug.Log($"AI drew a card. AI hand count: {CardManager.instance.aiHand.Count}");
						yield return new WaitForSeconds(delay);
					}
					aiState = AiStates.CastMana;
					break;

				case AiStates.CastMana:
					foreach (var cardSlot in CardManager.instance.aiHand.ToArray()) // Use ToArray to avoid collection modification issues
					{
						var card = cardSlot.GetComponent<CardDisplay>().card;
						if (card.ManaCard)
						{
							cardSlot.AiCastMana();
							Debug.Log($"AI cast mana from {card.name}");
							yield return new WaitForSeconds(delay);
						}
					}
					aiState = AiStates.CastCards;
					break;

				case AiStates.CastCards:
					bool cardPlayed = false;
					foreach (var cardSlot in CardManager.instance.aiHand.ToArray()) // Use ToArray for safe iteration
					{
						var card = cardSlot.GetComponent<CardDisplay>().card;
						if (card.ManaCost <= aiMana)
						{
							cardSlot.AIPlayCard();
							Debug.Log($"AI played {card.name}");
							cardPlayed = true;
							yield return new WaitForSeconds(delay);
						}
					}

					// Exit CastCards if no cards can be played
					if (!cardPlayed)
					{
						aiState = AiStates.Discard;
					}
					break;

				case AiStates.Discard:
					foreach (var cardSlot in CardManager.instance.aiHand.ToArray())
					{
						Debug.Log($"AI discarded {cardSlot.GetComponent<CardDisplay>().card.name}");
						cardSlot.MoveToDiscardPile();
						yield return new WaitForSeconds(delay);
					}
					aiState = AiStates.EndTurn;
					break;

				case AiStates.EndTurn:
					EndTurn();
					aiDone = true;
					break;
			}

			yield return null;
		}
	}


	private enum AiStates
	{
		Draw,
		CastMana,
		CastCards,
		Discard,
		EndTurn
	}
}
