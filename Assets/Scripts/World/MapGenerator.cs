using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    int mapWidth;
    [SerializeField]
    int mapHeight;
    [SerializeField]
    float noiseScale;

    [SerializeField]
    int octaves;
    [SerializeField]
    float persistence;
    [SerializeField]
    float lacunarity;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity);

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }
}
