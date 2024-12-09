using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CardManager playerCards;
    public CardManager aiCards;
    // Player and AI attributes
    public int playerMana = 1;
    public int aiMana = 1;
    public int maxHandSize = 5;

    public List<CardSlot> playerHand = new List<CardSlot>();
    public List<CardSlot> aiHand = new List<CardSlot>();

    public int playerHealth = 20;
    public int aiHealth = 20;

    public bool isPlayerTurn = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        InitializeGame();
        StartTurn();
    }

    private void InitializeGame()
    {
        // Draw initial cards for both player and AI
        for (int i = 0; i < maxHandSize; i++)
        {
        }
    }

    private void StartTurn()
    {
        if (isPlayerTurn)
        {
            Debug.Log("Player's Turn");
        }
        else
        {
            Debug.Log("AI's Turn");
        }
    }

    private void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        StartTurn();
    }

    private Cards GetCardData(CardSlot cardSlot)
    {
        return cardSlot.GetComponent<CardDisplay>().card;
    }
}
