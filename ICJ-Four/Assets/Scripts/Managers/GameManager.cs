using System.Collections;
using System.Collections.Generic;
using TMPro;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public TMP_Text ManaText;
	public TMP_Text AiManaText;
	public TMP_Text DrawCount;
	public TMP_Text DiscardCount;
	public TMP_Text P_HealthText;
	public TMP_Text A_HealthText;
	public TMP_Text STATEtext;
	public static GameManager instance;

	[SerializeField, Foldout("Settings")] private int drawsPerTurn = 5;
	[SerializeField, Foldout("Settings")] private int discardsPerTurn = 5;
	[SerializeField, Foldout("Settings")] public int discardMana = 2;
	[SerializeField, Scene, Foldout("Settings")] public int sceneOnWin;
	[SerializeField, Scene, Foldout("Settings")] public int sceneOnLoss;

	[Foldout("Player Settings")] public int playerHealth = 20;
	[Foldout("Player Settings")] public int playerMana = 10;

	[Foldout("Ai Settings")] public int aiHealth = 20;
	[Foldout("Ai Settings")] public int aiMana = 10;

	public bool isPlayerTurn = true;
	public int turnCount = 2;

	public int draws;
	public int discards;
	public int turn;

	private AiStates aiState;

	private bool winnerSelected;
	private void Awake()
	{
		if (instance == null) instance = this;
	}

	private void Start()
	{
		InitializeGame();
	}

	private void FixedUpdate()
	{
		UpdateUI();
		HandleGameStates();
	}

	private void InitializeGame()
	{
		Debug.Log("Game initialized");
		STATEtext.text = $""; // "hides text box"
		turn = turnCount;
		StartTurn();
	}

	private void StartTurn()
	{
		draws = drawsPerTurn;
		discards = discardsPerTurn;
		Debug.Log(isPlayerTurn ? "Player's turn" : "AI's turn");

		if (!isPlayerTurn)
		{
			aiState = AiStates.Draw;
			StartCoroutine(HandleAiTurn());
		}
	}

	private void EndTurn()
	{
		turn--;
		if (turn <= 0)
		{
			ResolveBattles();
		}
		else
		{
			isPlayerTurn = !isPlayerTurn;
			StartTurn();
		}
	}

	private void HandleGameStates()
	{
		if (CardManager.instance.deck.Count <= 0)
		{
			CardManager.instance.Shuffle();
		}

		if (isPlayerTurn && draws <= 0 && CardManager.instance.Hand.Count == 0)
		{
			EndTurn();
		}
		
		if (aiHealth <= 0)
		{
			aiHealth = 0;
			// Player Won Entire Game
			SceneManager.LoadScene(sceneOnWin);
		}
		else if (playerHealth <= 0)
		{
			playerHealth = 0;
			// Ai Won Entire Game
			SceneManager.LoadScene(sceneOnLoss);
		}
	}

	private void ResolveBattles()
	{
		// Initialize player and AI stats
		int playerHP = 0, playerATK = 0;
		int aiHP = 0, aiATK = 0;

		// Accumulate stats from played cards
		foreach (var cardSlot in CardManager.instance.playedPile)
		{
			var cardData = cardSlot.GetCardData(); // Cache card data for better readability

			if (cardSlot.owner == CardSlot.Owner.Player)
			{
				playerHP += cardData.Health;
				playerATK += cardData.Attack;
			}
			else if (cardSlot.owner == CardSlot.Owner.AI)
			{
				aiHP += cardData.Health;
				aiATK += cardData.Attack;
			}
		}

		Debug.Log($"Battle Starts! Player: {playerHP} HP, {playerATK} ATK | AI: {aiHP} HP, {aiATK} ATK");

		int round = 1;

		int leftoverPlayerDamage = Mathf.Max(0, playerATK - aiHP); // Damage Player dealt beyond AI's HP
		int leftoverAIDamage = Mathf.Max(0, aiATK - playerHP);     // Damage AI dealt beyond Player's HP

		// Simulate battle until one side's HP drops to zero
		while (playerHP > 0 && aiHP > 0)
		{
			// Each side attacks simultaneously
			aiHP = Mathf.Max(0, aiHP - playerATK);
			playerHP = Mathf.Max(0, playerHP - aiATK);

			Debug.Log($"Round {round}: Player {playerHP} HP | AI {aiHP} HP");
			round++;
		}

		// Calculate leftover damage

		Debug.Log(leftoverAIDamage);
		Debug.Log(leftoverPlayerDamage);


		// Determine the winner
		if (playerHP > 0 && aiHP == 0 && !winnerSelected)
		{
			Debug.Log("Player Wins!");
			aiHealth -= leftoverPlayerDamage; // Apply leftover damage to AI's total health
			isPlayerTurn = true;
			winnerSelected = true;
			STATEtext.text = $"Player won";
		}
		else if (aiHP > 0 && playerHP == 0 && !winnerSelected)
		{
			Debug.Log("AI Wins!");
			playerHealth -= leftoverAIDamage; // Apply leftover damage to Player's total health
			isPlayerTurn = false;
			winnerSelected = true;
			STATEtext.text = $"AI won";
		}
		else if (!winnerSelected)
		{
			Debug.Log("It's a Tie!");
			STATEtext.text = $"Tie";
			isPlayerTurn = !isPlayerTurn; // Alternate turn in case of a tie
		}
		// Reset the game state with a delay
		StartCoroutine(SlightResetDelay(2f));
	}





	private void UpdateUI()
	{
		ManaText.text = playerMana.ToString();
		AiManaText.text = aiMana.ToString();
		DrawCount.text = draws.ToString();
		DiscardCount.text = discards.ToString();
		P_HealthText.text = playerHealth.ToString();
		A_HealthText.text = aiHealth.ToString();
	}

	private void ClearGame()
	{
		foreach (var cardSlot in CardManager.instance.playedPile.ToList())
		{
			cardSlot.DiscardCardsBecauseISaySo();
		}

		for (int i = 0; i < CardManager.instance.availablePlaceableSlots.Length; i++)
		{
			CardManager.instance.availablePlaceableSlots[i] = true;
		}

		for (int i = 0; i < CardManager.instance.AI_availablePlaceableSlots.Length; i++)
		{
			CardManager.instance.AI_availablePlaceableSlots[i] = true;
		}
	}

	private IEnumerator SlightResetDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		ClearGame();
		turn = turnCount;
		StartTurn();
		STATEtext.text = "";
		winnerSelected = false;
	}

	private IEnumerator HandleAiTurn()
	{
		float delay = 0.5f;

		while (!isPlayerTurn)
		{
			switch (aiState)
			{
				case AiStates.Draw:
					while (draws > 0 && CardManager.instance.aiHand.Count < 5)
					{
						CardManager.instance.DrawCard(false);
						yield return new WaitForSeconds(delay);
					}
					aiState = AiStates.CastMana;
					break;

				case AiStates.CastMana:
					foreach (var cardSlot in CardManager.instance.aiHand.ToArray())
					{
						var card = cardSlot.GetComponent<CardDisplay>().card;
						if (card.ManaCard)
						{
							cardSlot.AiCastMana();
							yield return new WaitForSeconds(delay);
						}
					}
					aiState = AiStates.CastCards;
					break;

				case AiStates.CastCards:
					bool cardPlayed = false;
					foreach (var cardSlot in CardManager.instance.aiHand.ToArray())
					{
						var card = cardSlot.GetComponent<CardDisplay>().card;
						if (card.ManaCost <= aiMana)
						{
							cardSlot.AIPlayCard();
							cardPlayed = true;
							yield return new WaitForSeconds(delay);
						}
					}
					if (!cardPlayed)
					{
						aiState = AiStates.Discard;
					}
					break;

				case AiStates.Discard:
					foreach (var cardSlot in CardManager.instance.aiHand.ToArray())
					{
						cardSlot.MoveToDiscardPile();
						yield return new WaitForSeconds(delay);
					}
					aiState = AiStates.EndTurn;
					break;

				case AiStates.EndTurn:
					EndTurn();
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
