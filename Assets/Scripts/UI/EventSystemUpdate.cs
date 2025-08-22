using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemUpdate : EventSystem
{
    public void ManualUpdate()
    {
        base.Update();
    }
}
