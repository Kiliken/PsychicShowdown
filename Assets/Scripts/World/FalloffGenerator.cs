using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator {

    //static float pathFalloff = 2.0f;
    
    public static float[,] GenerateFalloffMap(int mapWidth, int mapHight)
    {
        float[,] map = new float[mapWidth, mapHight];

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHight; j++)
            {
                float x = i / (float)mapWidth * 2 - 1;
                float y = j / (float)mapHight * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);

                /*if (i < (mapWidth / 2) + pathFalloff && i > (mapWidth / 2) - pathFalloff && Evaluate(value) < 0.25f)
                {
                    float dist = i - (((float)mapWidth) / 2) + 0.5f ;
                    
                    dist = MathF.Abs(dist) / (pathFalloff * 2);
                    dist = (0.5f - dist);
                    //Debug.Log(dist);
                    map[i, j] = dist;
                }*/
            }
        }
        return map;
    }

    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
