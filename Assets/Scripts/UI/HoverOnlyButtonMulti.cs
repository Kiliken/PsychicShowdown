// Multiplayer version for multi-EventSystem
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HoverOnlyButtonMulti : HoverOnlyButton
{
    public EventSystem targetEventSystem;
    public bool isP1;
    

    protected override void Start()
    {
        base.Start();
        if (targetEventSystem == null)
        {
            string targetName = isP1 ? "EventSystemP1" : "EventSystemP2";
            GameObject found = GameObject.Find(targetName);
            if (found != null)
            {
                targetEventSystem = found.GetComponent<EventSystem>();
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} couldn't find {targetName} in scene.");
            }
        }
    }

    void Update()
    {
        void Update()
        {
            if (targetEventSystem == null || targetEventSystem.currentSelectedGameObject == null)
                return;

            if (targetEventSystem.currentSelectedGameObject == gameObject)
            {
                if (gs == null) return;

                bool clicked = false;

                if (gs.p1ControllerIsPS && Input.GetButtonDown("Jump1"))
                {
                    clicked = true;
                }
                else if (!gs.p1ControllerIsPS && Input.GetButtonDown("Jump1X"))
                {
                    clicked = true;
                }

                if (gs.p2ControllerIsPS && Input.GetButtonDown("Jump2"))
                {
                    clicked = true;
                }
                else if (!gs.p2ControllerIsPS && Input.GetButtonDown("Jump2X"))
                {
                    clicked = true;
                }

                if (clicked) TriggerClick();
            }
        }

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        targetEventSystem?.SetSelectedGameObject(gameObject);
    }
}
