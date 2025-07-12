using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverOnlyButton : Button
{
    private GameSettings gs;

    protected override void Start()
    {
        base.Start();
        gs = FindObjectOfType<GameSettings>();
    }

    void Update()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
        {
            return; // Exit if no EventSystem or no selected GameObject
        }
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            if (gs == null) return;

            bool clicked = false;

            if (gs.p1ControllerIsPS)
            {
                if (Input.GetButtonDown("Jump1"))
                {
                    clicked = true;
                    Debug.Log("p1ps");
                }
            }
            else
            {
                if (Input.GetButtonDown("Jump1X"))
                {
                    clicked = true;
                    Debug.Log("p1x");
                }
            }

            if (gs.p2ControllerIsPS)
            {
                if (Input.GetButtonDown("Jump2"))
                {
                    clicked = true;
                    Debug.Log("p2ps");
                }
                }
            else
            {
                if (Input.GetButtonDown("Jump2X"))
                {
                    clicked = true;
                    Debug.Log("p2x");
                }
            }

            if (clicked) TriggerClick();
        }
    }

    void TriggerClick()
    {

        onClick.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);


        Debug.Log("Pointer entered: " + gameObject.name);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // Allow only mouse clicks
        if (eventData.pointerId == -1)
        {
            base.OnPointerClick(eventData);
        }



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

    public override void OnSubmit(BaseEventData eventData)
    {
        // Block Unity's default submit input (A/X/Cross)
        Debug.Log("Blocked default submit");
    }

}
