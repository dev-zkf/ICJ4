using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteractionUnit : MonoBehaviour, IPointerClickHandler
{
    public bool clickable;

	public void Start()
	{
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		// Check if there is any active CardSlot in this object or its children
		CardSlot[] cardSlots = GetComponentsInChildren<CardSlot>();
		foreach (CardSlot cardSlot in cardSlots)
		{
			if (cardSlot.gameObject.activeInHierarchy)
			{
				if (eventData.button == PointerEventData.InputButton.Left && clickable)
				{
					cardSlot.PlayCard();
					return; // Exit after handling the first valid CardSlot
				}
				else if (eventData.button == PointerEventData.InputButton.Right && clickable)
				{
					if (MatchManager.instance.discards > 0)
					{
						cardSlot.MoveToDiscardPile();
						return; // Exit after handling the first valid CardSlot
					}
				}
			}
		}
	}
}
