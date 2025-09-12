using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh };

    [Header("Mesh Generator")]
    [SerializeField]
    DrawMode drawMode;

    bool genFlag = false;

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

    [NonSerialized]
    public List<GameObject> objects;

    private ushort objectsCount = 1;

    private float maxObjects = 50;

    private Vector3 genCursor;
    private LayerMask ground;
    private RaycastHit hit;
    private MaterialPropertyBlock propertyBlock;

    private GameObject thisObj;
    private Color thisColor;
    private float objectHeight = 0;

    private float timer = 150;

    [SerializeField]
    private GameObject suddenDeath;

    System.Random objectRnd;

    //[SerializeField]
    //TerrainType[] regions;
    private static Vector3[] objOffset = 
        { 
        new Vector3(0, 1f, 0), //rock
        new Vector3(0, 0, 0), //wall
        new Vector3(0, 0.5f, 0), //tomato
        new Vector3(0, 2f, 0), //flan
        new Vector3(0, 1f, 0), //duck
        new Vector3(0, 2f, 0), //chair
        new Vector3(0, 1f, 0), //spear
        new Vector3(0, 0.5f, 0), //meteor
        new Vector3(0, 0, 0), //tree
        new Vector3(0, 3f, 0), //guitar
        new Vector3(0, 0, 0), //pillar
        new Vector3(0, 0, 0), //grenade
        new Vector3(0, 0, 0), //microwave
        new Vector3(0, 1f, 0), //skull
    };

    public void GenerateMap()
    {
        if (genFlag)
            return;

        genFlag = true;

        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);
        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {


                noiseMap[x, y] = noiseMap[x, y] - falloffMap[x, y];


                //float currentheight = noiseMap[x, y];
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

        genFlag = false;
    }

    public void GenerateObjects()
    {

        GameObject grassHolder = new GameObject("WorldGrass");
        grassHolder.transform.position = Vector3.zero;
        grassHolder.transform.rotation = Quaternion.identity;
        Quaternion spawnRotation;
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
                        if (Physics.Raycast(new Vector3(genCursor.x + Random.Range(-5, 5), 50f, genCursor.z + Random.Range(-3, 3)), Vector3.down, out hit, Mathf.Infinity, ~ground))
                        {
                            thisObj = Instantiate(grassPrefab, hit.point - Vector3.up * 0.3f, Quaternion.identity);
                            //thisObj.transform.eulerAngles = Vector3.up * Random.Range(0, 360);
                            spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                            thisObj.transform.rotation = spawnRotation;
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

        for (int i = 0; i < maxObjects; i++)
        {
            SpanwObject();
        }

    }

    private void SpanwObject()
    {
        genCursor = new Vector3(objectRnd.Next((int)-generatorLimit, (int)generatorLimit), 50f, objectRnd.Next((int)-generatorLimit, (int)generatorLimit));

        if (Physics.Raycast(genCursor, Vector3.down, out hit, Mathf.Infinity))
        {
            int rand, randObj;
            rand = objectRnd.Next(0, 2);
            randObj = 0;
            objectHeight = 50f - hit.distance;
            switch (true)
            {
                case true when (objectHeight < 2f):
                    //grass
                    if (rand != 0)
                    {
                        thisObj = Instantiate(prefabs[0], hit.point + objOffset[0], Quaternion.identity);
                        ColorUtility.TryParseHtmlString("#503D35", out thisColor);
                        propertyBlock.SetColor("_BaseColor", thisColor);
                        thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                    }
                    else
                    {
                        randObj = objectRnd.Next(1, 4);
                        thisObj = Instantiate(prefabs[randObj], hit.point + objOffset[randObj], Quaternion.identity);
                        //propertyBlock.SetColor("_BaseColor", Color.gray);
                        //thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                    }

                    break;
                case true when (objectHeight < 6f):
                    // sand

                    if (rand != 0)
                    {
                        thisObj = Instantiate(prefabs[0], hit.point + objOffset[0], Quaternion.identity);
                        ColorUtility.TryParseHtmlString("#716749", out thisColor);
                        propertyBlock.SetColor("_BaseColor", thisColor);
                        thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                    }
                    else
                    {
                        randObj = objectRnd.Next(4, 8);
                        thisObj = Instantiate(prefabs[randObj], hit.point + objOffset[randObj], (randObj == 6 ? Quaternion.Euler(-90f, 0, 0) : Quaternion.identity));
                        //ColorUtility.TryParseHtmlString("#b3a0c0", out thisColor);
                        //propertyBlock.SetColor("_BaseColor", thisColor);
                        //thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                    }
                    break;
                case true when (objectHeight < 22f):
                    //forest
                    if (rand != 0)
                    {
                        thisObj = Instantiate(prefabs[0], hit.point + objOffset[0], Quaternion.identity);
                        ColorUtility.TryParseHtmlString("#39483c", out thisColor);
                        propertyBlock.SetColor("_BaseColor", thisColor);
                        thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                    }
                    else
                    {
                        randObj = objectRnd.Next(8, 11);
                        thisObj = Instantiate(prefabs[randObj], hit.point + objOffset[randObj], (randObj == 9 ? Quaternion.Euler(90,0,0) : Quaternion.identity));
                        //ColorUtility.TryParseHtmlString("#44774d", out thisColor);
                        //propertyBlock.SetColor("_BaseColor", thisColor);
                        //thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                    }
                    break;
                default:
                    if (rand != 0)
                    {
                        thisObj = Instantiate(prefabs[0], hit.point + objOffset[0], Quaternion.identity);
                        propertyBlock.SetColor("_BaseColor", Color.gray);
                        thisObj.transform.GetChild(1).GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
                    }
                    else
                    {
                        randObj = objectRnd.Next(11, 14);
                        thisObj = Instantiate(prefabs[randObj], hit.point + objOffset[randObj], Quaternion.identity);
                        
                    }
                    break;

            }
            thisObj.GetComponent<ThrowableObject>().objectID = objectsCount;
            objectsCount++;

            objects.Add(thisObj);
            Debug.Log($"Objects count:{objects.Count}");
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

        seed = (GameObject.FindObjectOfType<LoadedDataStorage>() != null ? GameObject.FindObjectOfType<LoadedDataStorage>().seed : 0);
        Random.InitState(seed);
        objectRnd = new System.Random(seed);

        objects = new List<GameObject>();

        genCursor = Vector3.zero;
        ground = LayerMask.NameToLayer("Ground");
        propertyBlock = new MaterialPropertyBlock();
        thisColor = Color.white;

        if (!isTitle)
        {
            GenerateMap();
            GenerateObjects();
        }

        if (isTitle)
        {
            GenerateMap();
        }

    }

    private void Update()
    {
        if (!isTitle)
        {
            /*Debug.DrawLine(Vector3.zero, Vector3.left * generatorLimit, Color.red);
            if (timer > 30)
            {
                timer -= Time.deltaTime;

                if (timer < 90 && suddenDeath.activeSelf == false) suddenDeath.SetActive(true);

                if (suddenDeath.activeSelf == true)
                {
                    suddenDeath.transform.localScale -= Vector3.one * 100 * Time.deltaTime;
                    generatorLimit -= Time.deltaTime * 1f;
                    maxObjects -= Time.deltaTime * 1f;

                    //delete object outside of the zone
                    GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");

                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (objects[i].GetComponent<ThrowableObject>())
                        {
                            if (Vector3.Distance(Vector3.zero, new Vector3(objects[i].transform.position.x, 0f, objects[i].transform.position.z)) >= generatorLimit && objects[i].GetComponent<ThrowableObject>().canGrab)
                            {
                                objects[i].GetComponent<ThrowableObject>().RemoveObject();
                                break;
                            }
                        }
                    }
                }

            }
            */
            for (int i = 0; i < objects.Count; i++)
                if (objects[i] == null)
                    objects.Remove(objects[i]);
    


            if (objects.Count < maxObjects)
                    {
                        SpanwObject();
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


