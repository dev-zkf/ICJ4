using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuEffects : MonoBehaviour
{
    private bool raised = false;
    public bool clickable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float scaleAmount;
    public float upAmount;

    private Vector3 scale;
    private Vector3 pos;
    private int orderInLayer;
    void Start()
    {
        scale = transform.localScale;
        pos = transform.localPosition;
        orderInLayer = GetComponent<Canvas>().sortingOrder;
    }
    public void OnPointerEnter()
    {
        if (!raised)
        { 
            this.GetComponent<Canvas>().sortingOrder++;
            transform.localScale = Vector3.one * scaleAmount;
            transform.localPosition = Vector3.up * upAmount;
            raised = true;
        }
    }

    public void OnPointerExit()
    {
        if (raised)
        {
            this.GetComponent<Canvas>().sortingOrder = orderInLayer;
            transform.localScale = scale;
            transform.localPosition = pos;
            raised = false;
        }
    }
}

