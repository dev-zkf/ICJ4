using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private bool raised;

    public AudioClip hoverSFX;
    public AudioClip clickSFX;
    public AudioClip discardSFX;

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
                    SoundFXManagergerg.Instance.PlaySoundFXClip(hoverSFX, transform, 1f);
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
				SoundFXManagergerg.Instance.PlaySoundFXClip(clickSFX, transform, 1f);
				// Reset raised state on play
				raised = false;
            }
            else if (child.gameObject.activeSelf && eventData.button == PointerEventData.InputButton.Right)
            {
                if (GameManager.instance.discards > 0)
				    SoundFXManagergerg.Instance.PlaySoundFXClip(discardSFX, transform, 1.5f);
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
