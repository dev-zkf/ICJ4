using System.Collections;
using System.Collections.Generic;
using TMPro;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class MatchManager : MonoBehaviour
{
	public TMP_Text DrawCount;
	public TMP_Text DiscardCount;
	public static MatchManager instance;
    public SoundEffect NuhUhSFX;

    [SerializeField, Foldout("Settings")] private int drawsPerTurn = 5;
	[SerializeField, Foldout("Settings")] private int discardsPerTurn = 5;
	[SerializeField, Foldout("Settings")] public int discardMana = 2;
	[SerializeField, Scene, Foldout("Settings")] public int sceneOnWin;
	[SerializeField, Scene, Foldout("Settings")] public int sceneOnLoss;
	[SerializeField, Scene, Foldout("Settings")] public int mainMenu;

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

	private void Update()
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
		if (turn <= 0) turn = turnCount;
        if (!isPlayerTurn)
		{
			aiState = AiStates.Draw;
			StartCoroutine(HandleAiTurn());
		}
	}
	public void LoadMenu()
	{
        SceneManager.LoadScene(mainMenu);
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
			StartTurn();
		}
	}


    private void HandleGameStates()
	{


		if (CardManager.instance.deck.Count <= 0)
		{
			CardManager.instance.Shuffle();
		}

		if (isPlayerTurn && draws <= 0 && CardManager.instance.Hand.Count == 0 && !winnerSelected && turn > 0)
		{
			isPlayerTurn = false;
			EndTurn();
		}

        if (discards <= 0 && CardManager.instance.Hand.Count > 0)
        {
            // Iterate through the hand safely by copying it to an array
            var handCopy = CardManager.instance.Hand.ToArray();

            foreach (var cardSlot in handCopy)
            {
                if (cardSlot.GetCardData().ManaCost > playerMana && !cardSlot.GetCardData().ManaCard && CardManager.instance.Hand.Count > 0 && !cardSlot.played)
                {
					// Exit the loop if the player has no more cards or health is too low
                    if (CardManager.instance.Hand.Count == 0 || playerHealth <= 0)
                    {
                        break;
                    }
                    Debug.Log($"Damage discard due to no mana, discards {cardSlot.GetCardData().name} {cardSlot.GetCardData().ManaCost} | {playerMana}");
                    cardSlot.AntiSoftLockDiscard();
                }
            }
        }


        if (aiHealth <= 0 && playerHealth > 0)
		{
			aiHealth = 0;
			// Player Won Entire Game
			SceneManager.LoadScene(sceneOnWin);
		}
		else if (playerHealth <= 0 && aiHealth > 0)
		{
			playerHealth = 0;
			// Ai Won Entire Game
			SceneManager.LoadScene(sceneOnLoss);
		}
		else if (playerHealth <= 0 && aiHealth <= 0)
		{
			playerHealth++;
			aiHealth++;
			Debug.Log("tie giving hp back to conclude match");
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
		int playerLeftoverDamage;
		int aiLeftoverDamage;
		// Precalculate potential damage to apply to loser's real HP

		if (aiHP <= 0)
		{
			playerLeftoverDamage = Mathf.Max(0, playerATK);
		}
		else
		{
			playerLeftoverDamage = Mathf.Max(0, playerATK - aiHP);
		}


		if (playerHP <= 0)
		{
			aiLeftoverDamage = Mathf.Max(0, aiATK);
		}
		else
		{
			aiLeftoverDamage = Mathf.Max(0, aiATK - playerHP);
		}

		playerHP -= aiATK;
		aiHP -= playerATK;

        // Check if the player wins
        if (playerHP > aiHP && playerATK >= aiHP || playerHP == aiHP && playerATK > aiATK)
        {
            if (winnerSelected) return;
            Debug.Log("Player Wins!");
            PlayerWin(playerLeftoverDamage);
            winnerSelected = true;
            return;
        }

        // Check if the AI wins
        if (aiHP > playerHP && aiATK >= playerHP || playerHP == aiHP && aiATK > playerATK)
        {
            if (winnerSelected) return;
            Debug.Log("AI Wins!");
            AiWin(aiLeftoverDamage);
            winnerSelected = true;
            return;
        }

        // Check for ties
        if (playerHP == aiHP && playerATK == aiATK || playerHP <= aiATK && aiHP <= playerATK)
        {
            if (winnerSelected) return;
            Debug.Log("It's a Tie!");
			GameManager.Instance.gameState.text = "Tie";
            playerHealth -= 2;
            aiHealth -= 2;
            isPlayerTurn = !isPlayerTurn;
            winnerSelected = true;
            StartCoroutine(SlightResetDelay(2f));
            return;
        }

        // Default case: No valid winner or tie (shouldn't happen if logic is correct)
        Debug.LogError("Unexpected state: Softlocked or invalid game state.");
		GameManager.Instance.gameState.text = "Error: Invalid State";
	}


	// MAKE THIS JUST A SINGLE FUNCTION WHEN I CAN BE ASKED USING SWITCHES
	private void PlayerWin(int playerLeftoverDamage)
	{
		aiHealth -= playerLeftoverDamage; // Apply leftover damage only once
		winnerSelected = true;
		isPlayerTurn = true;
		if ( GameManager.Instance != null ) 
			GameManager.Instance.gameState.text = "Player won";
		StartCoroutine(SlightResetDelay(2f));

	}

	private void AiWin(int aiLeftoverDamage)
	{
		playerHealth -= aiLeftoverDamage; // Apply leftover damage only once
		winnerSelected = true;
		isPlayerTurn = false;
		if (GameManager.Instance != null)
			GameManager.Instance.gameState.text = "AI won";
		StartCoroutine(SlightResetDelay(2f));
	}






	private void UpdateUI()
	{
		DrawCount.text = $"Draws [{draws}]";
		DiscardCount.text = $"Discards [{discards}]";
		try
		{
			GameManager.Instance.gameState.text = $""; // "hides text box"
			GameManager.Instance.firstPlayerManaCount.text = playerMana.ToString();
			GameManager.Instance.secondPlayerManaCount.text = aiMana.ToString();

			GameManager.Instance.firstPlayerHpCount.text = playerHealth.ToString();
			GameManager.Instance.secondPlayerHpCount.text = aiHealth.ToString();
		}
		catch { }

	}

	void UpdateFieldSafely(Action updateAction)
	{
		try
		{
			updateAction();
		}
		catch (Exception e)
		{
			Debug.Log($"Failed to update field: {e.Message}");
		}
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
        Debug.Log("Resetting game state after delay.");
        ClearGame();
        yield return new WaitForSeconds(delay);


		winnerSelected = false;
		if (GameManager.Instance != null)
			GameManager.Instance.gameState.text = "";
        Debug.Log($"Starting next turn. isPlayerTurn: {isPlayerTurn}");
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
					while (draws > 0 && CardManager.instance.aiHand.Count < 5 && CardManager.instance.deck.Count > 0)
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
							cardSlot.CastMana();
							yield return new WaitForSeconds(delay);
						}
					}
					aiState = AiStates.CastCards;
					break;

				case AiStates.CastCards:
					foreach (var cardSlot in CardManager.instance.aiHand.ToArray())
					{
						var card = cardSlot.GetComponent<CardDisplay>().card;
						if (card.ManaCost <= aiMana)
						{
							cardSlot.AIPlayCard();
							yield return new WaitForSeconds(delay);
						}
						else if (card.ManaCost > aiMana)
						{
							if (discards <= 0 && !cardSlot.GetCardData().ManaCard)
							{
                                if (CardManager.instance.aiHand.Count == 0 || aiHealth <= 0)
                                {
									yield break; // hope this does smthing
                                }
                                Debug.Log($"Damage discard due to no mana, discards {cardSlot.GetCardData().name} {cardSlot.GetCardData().ManaCost} | {aiMana}");
								cardSlot.AntiSoftLockDiscard();
							}
							else
							{
								Debug.Log($"Discarded {cardSlot.GetCardData().name} {cardSlot.GetCardData().ManaCost} | {aiMana}");
								cardSlot.MoveToDiscardPile();
							}
							yield return new WaitForSeconds(delay);
						}
					}
					if (CardManager.instance.aiHand.Count > 0)
						aiState = AiStates.Discard;
					else 
						aiState = AiStates.EndTurn;
					break;

				case AiStates.Discard: // for now discard is skipped unless somehow has cards in hand
					foreach (var cardSlot in CardManager.instance.aiHand.ToArray())
					{
						Debug.Log($"Discarded {cardSlot.GetCardData().name} {cardSlot.GetCardData().ManaCost} | {aiMana}");
						cardSlot.MoveToDiscardPile();
						yield return new WaitForSeconds(delay);
					}
					aiState = AiStates.EndTurn;
					break;

				case AiStates.EndTurn:
					isPlayerTurn = true;
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
