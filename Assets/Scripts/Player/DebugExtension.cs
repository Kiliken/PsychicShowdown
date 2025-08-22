using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class DebugExtension
{
    public static void DebugWireSphere(Vector3 position, Color color, float radius, float duration = 0f)
    {
        int segments = 20;
        float angle = 0f;
        float increment = 360f / segments;

        Vector3 lastPoint = position + new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);
        for (int i = 1; i <= segments; i++)
        {
            angle = increment * i * Mathf.Deg2Rad;
            Vector3 nextPoint = position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Debug.DrawLine(lastPoint, nextPoint, color, duration);
            lastPoint = nextPoint;
        }

        // Repeat for vertical circles
        lastPoint = position + new Vector3(0, Mathf.Cos(0) * radius, Mathf.Sin(0) * radius);
        for (int i = 1; i <= segments; i++)
        {
            angle = increment * i * Mathf.Deg2Rad;
            Vector3 nextPoint = position + new Vector3(0, Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Debug.DrawLine(lastPoint, nextPoint, color, duration);
            lastPoint = nextPoint;
        }

        lastPoint = position + new Vector3(Mathf.Cos(0) * radius, Mathf.Sin(0) * radius, 0);
        for (int i = 1; i <= segments; i++)
        {
            angle = increment * i * Mathf.Deg2Rad;
            Vector3 nextPoint = position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Debug.DrawLine(lastPoint, nextPoint, color, duration);
            lastPoint = nextPoint;
        }
    }
}

