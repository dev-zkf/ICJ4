using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using NaughtyAttributes;
public class CardSlot : MonoBehaviour
{
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
			transform.localPosition += Vector3.up * CardManager.instance.onPlayPopAmount * Screen.height;
			hasBeenPlayed = true;
			CardManager.instance.availableSlots[handIndex] = true;
			Invoke("MoveToDiscardPile", 0.5f);
		}
	}
	public void PopUp()
	{
		transform.localPosition = Vector3.zero;
		transform.localPosition += Vector3.up * CardManager.instance.onHoverPopAmount * Screen.height;
	}
	public void PopDown()
	{
		transform.localPosition -= Vector3.up * CardManager.instance.onHoverPopAmount * Screen.height;
	}
	/*
	private void OnMouseDown()
	{
		if (!hasBeenPlayed)
        {
            transform.position += Vector3.up * 5;
            hasBeenPlayed = true;
            CardManager.instance.availableSlots[handIndex] = true;
            Invoke("MoveToDiscardPile", 2f);
        }
	}
    */
	void MoveToDiscardPile()
    {
        CardManager.instance.discardPile.Add(this);
        gameObject.SetActive(false);
    }
}
