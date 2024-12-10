using UnityEngine;
using System.Collections;

public class CardSlot : MonoBehaviour
{
	public int handIndex;
	public bool played;

	public enum Owner { Player, AI}

	public Owner owner;
	public void PlayCard()
	{
		if (played || !GameManager.instance.isPlayerTurn) return;

		for (int i = 0; i < CardManager.instance.availablePlaceableSlots.Length; i++)
		{
			var cardData = GetCardData();

			if (cardData.ManaCard)
			{
				PlayManaCard();
				return;
			}

			if (CardManager.instance.availablePlaceableSlots[i] && cardData.ManaCost <= GameManager.instance.playerMana)
			{
				GameManager.instance.playerMana -= cardData.ManaCost;
				StartCoroutine(MoveToPlayAreaWithDelay(i));
				played = true;
				owner = Owner.Player;
				CardManager.instance.availablePlaceableSlots[i] = false;
				return;
			}
		}

		PopDown();
	}

	public void AiCastMana()
	{
		if (played) return;

		GameManager.instance.aiMana += GetCardData().ManaCost;
		MoveToDiscard();
		Debug.Log($"AI cast mana from card {GetCardData().name}");
	}

	public void AIPlayCard()
	{
		if (played || GameManager.instance.isPlayerTurn || GetCardData().ManaCard) return;

		for (int i = 0; i < CardManager.instance.AI_availablePlaceableSlots.Length; i++)
		{
			if (CardManager.instance.AI_availablePlaceableSlots[i])
			{
				GameManager.instance.aiMana -= GetCardData().ManaCost;
				MoveToPlayArea(CardManager.instance.AI_placeableSlots[i]);
				CardManager.instance.AI_availablePlaceableSlots[i] = false;
				played = true;
				owner = Owner.AI;
				Debug.Log($"AI played card {GetCardData().name}");
				return;
			}
		}
	}

	public void MoveToDiscardPile()
	{
		if (played || GameManager.instance.discards <= 0) return;

		GameManager.instance.discards--;

		if (GameManager.instance.isPlayerTurn)
		{
			GameManager.instance.playerMana += GameManager.instance.discardMana;
			CardManager.instance.availableSlots[handIndex] = true;
		}
		else
		{
			GameManager.instance.aiMana += GameManager.instance.discardMana;
			CardManager.instance.AI_availableSlots[handIndex] = true;
		}

		MoveToDiscard();
	}

	public void PopUp()
	{
		transform.localPosition = Vector3.up * CardManager.instance.onHoverPopAmount * Screen.height;
	}

	public void PopDown()
	{
		transform.localPosition = Vector3.zero;
	}

	private void PlayManaCard()
	{
		GameManager.instance.playerMana += GetCardData().ManaCost;
		MoveToDiscard();
		Debug.Log($"Played mana card {GetCardData().name}");
	}

	private void MoveToPlayArea(Transform playArea)
	{
		if (GameManager.instance.isPlayerTurn)
		{
			CardManager.instance.Hand.Remove(this);
		}
		else
		{
			CardManager.instance.aiHand.Remove(this);
		}
		CardManager.instance.playedPile.Add(this);
		transform.SetParent(playArea);
		transform.localPosition = Vector3.zero;
		transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
	}

	private void MoveToDiscard()
	{
		if (GameManager.instance.isPlayerTurn)
		{
			CardManager.instance.Hand.Remove(this); 
		}
		else
		{
			CardManager.instance.aiHand.Remove(this); 
		}
		CardManager.instance.discardPile.Add(this);
		gameObject.SetActive(false);
	}

	public Cards GetCardData()
	{
		return GetComponent<CardDisplay>().card;
	}

	private IEnumerator MoveToPlayAreaWithDelay(int slotIndex)
	{
		yield return new WaitForSeconds(0.2f);
		MoveToPlayArea(CardManager.instance.placeableSlots[slotIndex]);
	}
}
