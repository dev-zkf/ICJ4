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

		int playerHP = 0, playerATK = 0;
		int aiHP = 0, aiATK = 0;

		foreach (var cardSlot in CardManager.instance.playedPile)
		{
			if (cardSlot.owner == CardSlot.Owner.Player)
			{
				playerHP += cardSlot.GetCardData().Health;
				playerATK += cardSlot.GetCardData().Attack;
			}
			else if (cardSlot.owner == CardSlot.Owner.AI)
			{
				aiHP += cardSlot.GetCardData().Health;
				aiATK += cardSlot.GetCardData().Attack;
			}
		}

		Debug.Log($"Battle Starts! Player: {playerHP} HP, {playerATK} ATK | AI: {aiHP} HP, {aiATK} ATK");



		int round = 1;
		int leftoverPlayerDamage = Mathf.Max(0, playerATK - aiHP); // Damage that Player ATK exceeds AI HP
		int leftoverAIDamage = Mathf.Max(0, aiATK - playerHP);     // Damage that AI ATK exceeds Player HP

		while (playerHP > 0 && aiHP > 0)
		{
			playerHP -= aiATK;
			aiHP -= playerATK;

			Debug.Log($"Battle Round {round}: Player {playerHP} HP | AI {aiHP} HP");
			round++;
		}

		// Check who reached 0 HP first
		if (playerHP <= 0 && aiHP <= 0)
		{
			if (playerHP > aiHP)
			{
				Debug.Log("AI reached 0 HP first - Player Wins!");
				aiHealth -= leftoverPlayerDamage;
				isPlayerTurn = true;
			}
			else if (aiHP > playerHP)
			{
				Debug.Log("Player reached 0 HP first - AI Wins!");
				playerHealth -= leftoverAIDamage;
				isPlayerTurn = false;
			}
			else
			{
				Debug.Log("Both reached 0 HP at the same time - It's a Tie!");
				isPlayerTurn = !isPlayerTurn;
			}
		}
		else if (playerHP > 0)
		{
			Debug.Log("Player Wins!");
			aiHealth -= leftoverPlayerDamage;
			isPlayerTurn = true;
		}
		else
		{
			Debug.Log("AI Wins!");
			playerHealth -= leftoverAIDamage;
			isPlayerTurn = false;
		}

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
