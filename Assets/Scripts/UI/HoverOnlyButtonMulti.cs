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
        string targetName = isP1 ? "EventSystemP1" : "EventSystemP2";

        if (targetEventSystem == null)
        {
            GameObject found = GameObject.Find(targetName);
            if (found != null)
            {
                targetEventSystem = found.GetComponent<EventSystem>();
                Debug.Log($"{gameObject.name} found its EventSystem: {targetEventSystem.name}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} couldn't find {targetName} in scene.");
            }
        }
    }


    void Update()
    {
        if (targetEventSystem == null)
        {
            Debug.LogWarning($"{gameObject.name}: targetEventSystem is NULL.");
            return;
        }

        GameObject selected = targetEventSystem.currentSelectedGameObject;
        if (selected != gameObject)
        {
            // Uncomment for debugging if needed
            // Debug.Log($"{gameObject.name}: not selected. CurrentSelected = {selected?.name}");
            return;
        }

        if (gs == null)
        {
            gs = FindObjectOfType<GameSettings>();
            if (gs == null)
            {
                Debug.LogWarning($"{gameObject.name}: GameSettings (gs) is NULL.");
                return;
            }
        }

        bool clicked = false;

        if (isP1)
        {
            if (gs.p1ControllerIsPS && Input.GetButtonDown("Jump1"))
            {
                Debug.Log($"{gameObject.name}: P1 PS input detected");
                clicked = true;
            }
            else if (!gs.p1ControllerIsPS && Input.GetButtonDown("Jump1X"))
            {
                Debug.Log($"{gameObject.name}: P1 Xbox input detected");
                clicked = true;
            }
        }
        else // Player 2
        {
            if (gs.p2ControllerIsPS && Input.GetButtonDown("Jump2"))
            {
                Debug.Log($"{gameObject.name}: P2 PS input detected");
                clicked = true;
            }
            else if (!gs.p2ControllerIsPS && Input.GetButtonDown("Jump2X"))
            {
                Debug.Log($"{gameObject.name}: P2 Xbox input detected");
                clicked = true;
            }
        }

        if (clicked)
        {
            Debug.Log($"{gameObject.name}: TriggerClick for {(isP1 ? "P1" : "P2")}");
            TriggerClick();
        }
    }



    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        targetEventSystem?.SetSelectedGameObject(gameObject);
    }
}
