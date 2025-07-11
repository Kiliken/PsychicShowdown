using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverOnlyButton : Button
{
    protected override void Start()
    {
        base.Start();

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);


        Debug.Log("Pointer entered: " + gameObject.name);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {

        base.OnPointerClick(eventData);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        // Only block mouse-driven *hover* selection, not controller-based selection
        if (eventData is PointerEventData ped && ped.pointerId == -1)
        {
            // pointerId -1 = mouse, so block selection
            return;
        }

        base.OnSelect(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

    }
}
