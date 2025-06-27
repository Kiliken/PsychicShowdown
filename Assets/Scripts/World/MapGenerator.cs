using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh };

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
    [Range(0, 1)]
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
    [SerializeField]
    GameObject grassPrefab;
    string[] grassColors = { "#FFFFFF", "#FFEBBD", "#C2FFFF", "#DBB5B7" };

    //[SerializeField]
    //TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);
        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
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
        Random.InitState(seed);
        Vector3 genCursor = Vector3.zero;
        LayerMask ground = LayerMask.NameToLayer("Ground");
        RaycastHit hit;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        GameObject thisObj;
        Color thisColor = Color.white;
        float objectHeight = 0;

        GameObject grassHolder = new GameObject("WorldGrass");
        grassHolder.transform.position = Vector3.zero;
        grassHolder.transform.rotation = Quaternion.identity;
        grassHolder.isStatic = true;
        //GRASS
        for (int i = 0; i < 30f; i++)
        {

            genCursor = new Vector3(Random.Range(-generatorLimit, generatorLimit), 50f, Random.Range(-generatorLimit, generatorLimit));

            if (Physics.Raycast(genCursor, Vector3.down, out hit, Mathf.Infinity, ~ground))
            {
                objectHeight = 50f - hit.distance;

                if (objectHeight < 2f || (objectHeight > 8 && objectHeight < 22f))
                {
                    for (int j = 0; j < 20f; j++)
                    {
                        if (Physics.Raycast(new Vector3(genCursor.x + Random.Range(-5, 5), 50f,genCursor.z + Random.Range(-3,3)), Vector3.down, out hit, Mathf.Infinity, ~ground))
                        {
                            thisObj = Instantiate(grassPrefab, hit.point - Vector3.up * 0.3f, Quaternion.identity);
                            thisObj.transform.eulerAngles = Vector3.up * Random.Range(0, 360);
                            thisObj.transform.parent = grassHolder.transform;
                            ColorUtility.TryParseHtmlString(grassColors[Random.Range(0, grassColors.Length)], out thisColor);
                            propertyBlock.SetColor("_MainColor", thisColor);
                            thisObj.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                    }
                    
                }
                else i--;

            }
        }

        //Debug.Log(genCursor);

        for (int i = 0; i < 50f; i++)
        {
            
            genCursor = new Vector3(Random.Range(-generatorLimit, generatorLimit), 50f, Random.Range(-generatorLimit, generatorLimit));

            if (Physics.Raycast(genCursor, Vector3.down, out hit, Mathf.Infinity, ~ground))
            {
                int rand;
                rand = Random.Range(0, 2);
                objectHeight = 50f - hit.distance;
                switch (true)
                {
                    case true when (objectHeight < 2f):
                        //grass
                        if (rand != 0)
                        {
                            thisObj = Instantiate(prefabs[0], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#503D35", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        else
                        {
                            thisObj = Instantiate(prefabs[1], hit.point, Quaternion.identity);
                            propertyBlock.SetColor("_BaseColor", Color.gray);
                            thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }

                        break;
                    case true when (objectHeight < 7f):
                        // sand

                        if (rand != 0)
                        {
                            thisObj = Instantiate(prefabs[0], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#716749", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        else
                        {
                            thisObj = Instantiate(prefabs[3], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#b3a0c0", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        break;
                    case true when (objectHeight < 12f):
                        //forest
                        if (rand != 0)
                        {
                            thisObj = Instantiate(prefabs[0], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#39483c", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        else
                        {
                            thisObj = Instantiate(prefabs[2], hit.point, Quaternion.identity);
                            ColorUtility.TryParseHtmlString("#44774d", out thisColor);
                            propertyBlock.SetColor("_BaseColor", thisColor);
                            thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        break;
                    default:
                        if (rand != 0)
                        {
                            thisObj = Instantiate(prefabs[0], hit.point, Quaternion.identity);
                            propertyBlock.SetColor("_BaseColor", Color.gray);
                            thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                        }
                        else
                        {
                            thisObj = Instantiate(prefabs[4], hit.point, Quaternion.identity);
                        }
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
        seed = Random.Range(6, 4587);

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

            if (offset.y >= 100f)
            {
                offset.y = 0f;
                offset.x = 0f;
            }
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


