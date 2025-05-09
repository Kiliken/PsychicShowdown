using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh};

    [SerializeField]
    DrawMode drawMode;

    [SerializeField]
    int mapWidth;
    [SerializeField]
    int mapHeight;
    [SerializeField]
    float noiseScale;
    [SerializeField]
    float meshHeightMultiplier;
    [SerializeField]
    AnimationCurve meshHeightCurve;

    [SerializeField]
    int seed;
    [SerializeField]
    Vector2 offset;

    [SerializeField]
    int octaves;
    [SerializeField]
    [Range(0,1)]
    float persistence;
    [SerializeField]
    float lacunarity;

    [Space(12)]    
    public bool autoUpdate;

    [SerializeField]
    TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale, octaves, persistence, lacunarity, offset);
        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y=0; y < mapHeight; y++)
        {
            for (int x=0; x < mapWidth; x++)
            {
                noiseMap[x, y] = noiseMap[x, y] - falloffMap[x, y];
                float currentheight = noiseMap[x, y];
                for (int i=0; i < regions.Length; i++)
                {
                    if(currentheight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (drawMode == DrawMode.ColorMap)
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        else if (drawMode == DrawMode.Mesh)
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        //else if (drawMode == DrawMode.ColorMap)

    }

    private void OnValidate()
    {
        if (mapWidth < 1)
            mapWidth = 1;

        if (mapHeight < 1)
            mapHeight = 1;

        if (lacunarity < 1)
            lacunarity = 1;

        if (octaves < 0)
            octaves = 0;

    }

    private void Start()
    {
        GenerateMap();
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}


