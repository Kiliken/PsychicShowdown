using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh};

    [Header("Mesh Generator")]
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
    bool isTitle;

    [Header("Object generator")]
    [SerializeField]
    float generatorLimit;
    [SerializeField]
    GameObject[] prefabs;

    //[SerializeField]
    //TerrainType[] regions;

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
                /*for (int i=0; i < regions.Length; i++)
                {
                    if(currentheight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                } */
            }
        }

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        //else if (drawMode == DrawMode.ColorMap)
        //    display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        else if (drawMode == DrawMode.Mesh)
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve)); // TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight)
        //else if (drawMode == DrawMode.ColorMap)

    }

    public void GenerateObjects()
    {
        Vector3 genCursor = Vector3.zero;
        LayerMask ground = LayerMask.NameToLayer("Ground");
        RaycastHit hit;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        
        GameObject thisObj;
        Color thisColor = Color.white;
        float objectHeight = 0;
        


        //Debug.Log(genCursor);

        for (int i= 0; i< 50f; i++) {
            
            genCursor = new Vector3(Random.Range(-generatorLimit, generatorLimit), 50f, Random.Range(-generatorLimit, generatorLimit));
            
            if (Physics.Raycast(genCursor, Vector3.down, out hit, Mathf.Infinity, ~ground))
            {
                int rand;
                rand = Random.Range(0, 2);
                objectHeight = 50f - hit.distance;
                switch (true)
                {
                    case true when (objectHeight < 2f):
                        //shell or rock
                        if(rand != 0)
                        {
                            thisObj = Instantiate(prefabs[0], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#503D35", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        else
                        {
                            thisObj = Instantiate(prefabs[1], hit.point, Quaternion.identity);
                            propertyBlock.SetColor("_BaseColor", Color.gray);
                            thisObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }

                            break;
                    case true when (objectHeight < 7f):
                        // plants or rock
                        
                        if (rand != 0)
                        {
                            thisObj = Instantiate(prefabs[0], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#716749", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        else
                        {
                            thisObj = Instantiate(prefabs[3], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#b3a0c0", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        break;
                    case true when (objectHeight < 12f):
                        //granade or rock
                        if (rand != 0)
                        {
                            thisObj = Instantiate(prefabs[0], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#39483c", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        else
                        {
                            thisObj = Instantiate(prefabs[2], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#44774d", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        break;
                    default:
                        
                            thisObj = Instantiate(prefabs[0], hit.point, Quaternion.identity);
                            propertyBlock.SetColor("_BaseColor", Color.gray);
                            thisObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        
                        break;

                }
            }
        }

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
        seed = Random.Range(6,4587);

        if (!isTitle)
        {
            GenerateMap();
            GenerateObjects();
        }

    }

    private void Update()
    {
        if (isTitle)
        {
            offset.y += 0.75f * Time.deltaTime;
            offset.x += 0.75f * Time.deltaTime;
            GenerateMap();
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}


