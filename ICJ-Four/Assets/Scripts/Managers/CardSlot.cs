using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using NaughtyAttributes;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Collections;
public class CardSlot : MonoBehaviour
{
    public CardManager cardManager;
    public bool hasBeenPlayed;
    public int handIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
	public void PlayCard()
	{
		if (!hasBeenPlayed)
		{
            for (int i = 0; i < cardManager.availablePlaceableSlots.Length; i++)
            {
                if (cardManager.availablePlaceableSlots[i] == true)
                {
			        transform.localPosition += Vector3.up * cardManager.onPlayPopAmount * Screen.height;
			        hasBeenPlayed = true;
			        cardManager.availableSlots[handIndex] = true;
                    StartCoroutine(SlightDelay(i));
                    cardManager.availablePlaceableSlots[i] = false;
                    return;
                }
                else
                {
                    PopDown();
                }
            }
		}
	}
    public void PopUp()
    {
        transform.localPosition = Vector3.up * cardManager.onHoverPopAmount * Screen.height;
    }

    public void PopDown()
    {
        transform.localPosition = Vector3.zero; // Ensure it returns to the baseline
    }
    void MoveToPlayPile(int SlotIndex)
    {
            cardManager.playedPile.Add(this);
            cardManager.Hand.Remove(this);
            this.gameObject.transform.SetParent(cardManager.placeableSlots[SlotIndex]);
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            //gameObject.SetActive(false);
    }

   IEnumerator SlightDelay(int e)
   {
        yield return new WaitForSeconds(0.6f);
        MoveToPlayPile(e);
   }
}
