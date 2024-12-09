using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

	bool raised;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.gameObject.transform.childCount > 0 && !raised)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				if (gameObject.transform.GetChild(i).gameObject.activeSelf == true)
				{
					gameObject.transform.GetChild(i).GetComponentInChildren<CardSlot>().PopUp();
					raised = true;
				}
			}
		}
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.gameObject.transform.childCount > 0 && raised)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				if (gameObject.transform.GetChild(i).gameObject.activeSelf == true)
				{
					gameObject.transform.GetChild(i).GetComponentInChildren<CardSlot>().PopDown();
					raised = false;
				}
			}
		}
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.gameObject.transform.childCount > 0)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				if (gameObject.transform.GetChild(i).gameObject.activeSelf == true)
				{
					gameObject.transform.GetChild(i).GetComponentInChildren<CardSlot>().PlayCard();
				}
			}
		}
	}
}
