using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private bool raised;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HasActiveChild() && !raised)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.GetComponentInChildren<CardSlot>().PopUp();
                    raised = true;
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (raised)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.GetComponentInChildren<CardSlot>().PopDown();
                    raised = false;
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && eventData.button == PointerEventData.InputButton.Left)
            {
                child.GetComponentInChildren<CardSlot>().PlayCard();
                // Reset raised state on play
                raised = false;
            }
            else if (child.gameObject.activeSelf && eventData.button == PointerEventData.InputButton.Right)
            {
                child.GetComponentInChildren<CardSlot>().MoveToDiscardPile();
			}
        }
    }

    private bool HasActiveChild()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
                return true;
        }
        return false;
    }
}
