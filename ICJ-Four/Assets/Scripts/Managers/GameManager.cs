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

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        draws = drawsPerTurn;
        discards = discardsPerTurn;
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
        StartTurn();
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

        if (playerCardsOnField.Count > 0 && aiCardsOnField.Count > 0 && draws <= 0 && CardManager.instance.Hand.Count <= 0 && CardManager.instance.aiHand.Count <= 0)
        {
            // Resolve battles if both sides have cards
            if (playerCardsOnField.Count > 0 && aiCardsOnField.Count > 0)
            {
                int P_HP = 0, P_ATK = 0; // Player's total HP and Attack
                int A_HP = 0, A_ATK = 0; // AI's total HP and Attack

                StartCoroutine(ResolveBattles(P_HP, P_ATK, A_HP, A_ATK));
            }
        }
        else if (draws <= 0 && CardManager.instance.Hand.Count <= 0 && isPlayerTurn)
        {
            EndTurn();
        }

    }
    IEnumerator ResolveBattles(int P_HP, int P_ATK, int A_HP, int A_ATK)
    {
        Debug.Log("Fight Started");

        // Calculate Player's total stats
        foreach (var card in playerCardsOnField)
        {
            P_HP += card.GetCardData().Health;
            P_ATK += card.GetCardData().Attack;
        }

        // Calculate AI's total stats
        foreach (var card in aiCardsOnField)
        {
            A_HP += card.GetCardData().Health;
            A_ATK += card.GetCardData().Attack;
        }

        Debug.Log($"Battle Starts! Player: {P_HP} HP, {P_ATK} ATK | AI: {A_HP} HP, {A_ATK} ATK");

        // Fight loop
        while (P_HP > 0 && A_HP > 0)
        {
            // Player attacks AI
            A_HP -= P_ATK;
            if (A_HP <= 0)
            {
                Debug.Log("Player Wins!");
                isPlayerTurn = true;
                // Optionally handle game over state here
                break;
            }

            // AI attacks Player
            P_HP -= A_ATK;
            if (P_HP <= 0)
            {
                Debug.Log("AI Wins!");
                isPlayerTurn = false;
                EndTurn(); // Call EndTurn here to transition to the next turn
                           // Optionally handle game over state here
                break;
            }

            // Optional delay for visuals if needed
            yield return new WaitForSeconds(0.5f);
        }

        // Reset the game state or prepare for the next turn
        StartCoroutine(SlightResetDelay(0.1f));
    }
    private void UpdateUI()
    {
        ManaText.text = playerMana.ToString();
        AiManaText.text = aiMana.ToString();
        DrawCount.text = draws.ToString();
        DiscardCount.text = discards.ToString();
    }

    private IEnumerator SlightResetDelay(float delay)
    {
        playerCardsOnField.Clear();
        aiCardsOnField.Clear();
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < CardManager.instance.playedPile.Count; i++)
        {
            CardManager.instance.playedPile[i].DiscardCardsBecauseISaySo();
            CardManager.instance.availablePlaceableSlots[i] = true;
            CardManager.instance.AI_availablePlaceableSlots[i] = true;
        }
        InitializeGame();
        CardManager.instance.Shuffle();
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
