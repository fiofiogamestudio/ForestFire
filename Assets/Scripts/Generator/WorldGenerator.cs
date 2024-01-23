using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public enum EWorldScale
    {
        Small, // 512
        Normal // 1024
    }

    public EWorldScale WorldScale = EWorldScale.Normal;

    public bool HasRiver = true;

    [Header("Binded")]
    public HeightMapGenerator heightMapGenerator;
    public RiverGenerator riverGenerator;
    public RiverExtractor riverExtractor;
    public RiverParameterizer riverParameterizer;
    public RiverMeshGenerator riverMeshGenerator;
    public HeightMapModifier heightMapModifier;
    public TerrainRebuilder terrainRebuilder;

    public PremapGenerator premapGenerator;
    public FireSimulator fireSimulator;

    [ContextMenu("Generate World")]
    public void GenerateWorld()
    {
        GameConfig.USE_STREAM = true;

        // World Scale
        switch (WorldScale)
        {
            case EWorldScale.Small:
                GameConfig.MAP_WIDTH = 512;
                // GameConfig.RIVER_HEIGHT_SCALE = 100;
                GameConfig.TERRAIN_HEIGHT_SCALE = 0.5f;
                break;
            case EWorldScale.Normal:
                GameConfig.MAP_WIDTH = 1024;
                // GameConfig.RIVER_HEIGHT_SCALE = 200;
                // GameConfig.TERRAIN_HEIGHT_SCALE = 1.0f;
                break;
        }

        heightMapGenerator.seed = 2023;
        float[] heightmap = heightMapGenerator.GenerateHeightMap();

        riverGenerator.heightStream = heightmap;
        float[] rivermap = riverGenerator.GenerateRiver();
        riverExtractor.riverStream = rivermap;
        float[] extractedmap = riverExtractor.ExtractRiver();
        riverParameterizer.riverStream = extractedmap;
        riverMeshGenerator.heightStream = heightmap;
        riverMeshGenerator.GenerateRivermesh();
        heightMapModifier.heightStream = heightmap;
        heightMapModifier.riverStream = extractedmap;
        float[] modifiedMap = heightMapModifier.ModifyHeightMap();
        terrainRebuilder.heightStream = modifiedMap;
        terrainRebuilder.RefreshTerrain();

        GameConfig.USE_STREAM = false;
        PCGNode.PackNode.PackAndSave(heightmap, GameConfig.MAP_WIDTH, GameConfig.HEIGHTMAP_PATH);
    }


    [ContextMenu("LoadMap")]
    public void LoadMap()
    {
        MapInfo targetInfo = loadTargetMap(GameConfig.MAP_TO_LOAD);

        if (targetInfo != null)
        {
            string mapeName = targetInfo.mapName;
            Texture2D rawMap = loadTargetTexture(mapeName, targetInfo.rawMap);
            Texture2D extractedRiverMap = loadTargetTexture(mapeName, targetInfo.extractedRiverMap);

            GameConfig.USE_STREAM = true;
            float[] heightmap = PCGNode.PackNode.Unpack(rawMap);
            if (targetInfo.hasRiver)
            {
                float[] rivermap = PCGNode.PackNode.Unpack(extractedRiverMap);
                riverParameterizer.riverStream = rivermap;
                riverMeshGenerator.heightStream = heightmap;
                riverMeshGenerator.GenerateRivermesh();
                heightMapModifier.riverStream = rivermap;
            }
            else
            {
                riverMeshGenerator.ClearRiverMesh();
            }
            heightMapModifier.heightStream = heightmap;
            heightMapModifier.EnableRiver = targetInfo.hasRiver;
            float[] modifiedMap = heightMapModifier.ModifyHeightMap();
            terrainRebuilder.heightStream = modifiedMap;
            terrainRebuilder.RefreshTerrain();




            GameConfig.USE_STREAM = false;
            PCGNode.PackNode.PackAndSave(heightmap, GameConfig.MAP_WIDTH, GameConfig.HEIGHTMAP_PATH);
        }
        else
        {
            Debug.LogWarning("no map called " + GameConfig.MAP_TO_LOAD);
        }
    }

    public void RefreshMap()
    {
        MapInfo targetInfo = loadTargetMap(GameConfig.MAP_TO_LOAD);
        if (targetInfo != null)
        {
            string mapeName = targetInfo.mapName;
            Texture2D normalMap = loadTargetTexture(mapeName, targetInfo.normalMap);
            Texture2D slopeMap = loadTargetTexture(mapeName, targetInfo.slopeMap);


        }

    }

    private MapInfo loadTargetMap(string name)
    {
        MapJson jsonObject = DataLoader.LoadJson<MapJson>("Map/Map");
        foreach (var info in jsonObject.maps)
        {
            if (info.mapName == name) return info;
        }
        return null;
    }

    private Texture2D loadTargetTexture(string mapName, string texName)
    {
        string fullPath = "MapTex/" + mapName + "/" + texName;
        Debug.Log(fullPath);
        Texture2D tex = Resources.Load<Texture2D>(fullPath);
        return tex;
    }

    public static WorldGenerator instance;
    private float[] bufferHeightStream;
    void Awake()
    {
        if (instance == null) instance = this;

        // loadbuffer
        MapInfo targetInfo = loadTargetMap(GameConfig.MAP_TO_LOAD);
        if (targetInfo != null)
        {
            string mapeName = targetInfo.mapName;
            Texture2D bufferHeightTex = loadTargetTexture(mapeName, targetInfo.rawMap);
            bufferHeightStream = PCGNode.PackNode.Unpack(bufferHeightTex);
        }
    }

    public float GetPosHeight(int x, int y)
    {
        return bufferHeightStream[x + y * GameConfig.MAP_WIDTH] * GameConfig.RIVER_HEIGHT_SCALE;
    }

    public Vector2 CheckClick()
    {
        // Check if the left mouse button is pressed
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Convert the mouse position to a ray

        //     Plane plane = new Plane(Vector3.up, 0); // Create a plane at y=0 (X-Z plane)

        //     float distanceToPlane;
        //     if (plane.Raycast(ray, out distanceToPlane)) // If the ray intersects the plane
        //     {
        //         Vector3 hitPoint = ray.GetPoint(distanceToPlane); // Get the intersection point

        //         // Return the X and Z coordinates of the intersection point as a Vector2
        //         return new Vector2(hitPoint.x, hitPoint.z);
        //     }
        // }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Convert the mouse position to a ray

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) // If the ray intersects the terrain
            {
                if (hit.collider.gameObject.GetComponent<Terrain>() != null) // Check if the hit object is a Terrain
                {
                    Vector3 hitPoint = hit.point; // Get the intersection point

                    // Return the X and Z coordinates of the intersection point as a Vector2
                    return new Vector2(hitPoint.x, hitPoint.z);
                }
            }
        }

        // If the left mouse button is not pressed, return a zero vector
        return Vector2.zero;
    }

    public Vector2 CheckHold()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Convert the mouse position to a ray

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) // If the ray intersects the terrain
            {
                if (hit.collider.gameObject.GetComponent<Terrain>() != null) // Check if the hit object is a Terrain
                {
                    Vector3 hitPoint = hit.point; // Get the intersection point

                    // Return the X and Z coordinates of the intersection point as a Vector2
                    return new Vector2(hitPoint.x, hitPoint.z);
                }
            }
        }

        // If the left mouse button is not pressed, return a zero vector
        return Vector2.zero;
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameConfig.MAP_TO_LOAD = "Test";
            LoadMap();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            GameConfig.MAP_TO_LOAD = "Test1";
            LoadMap();
        }
    }

}


[System.Serializable]
public class MapInfo
{
    public string mapName;
    public bool hasRiver; // has River

    // river need
    public string rawMap; // raw height map
    public string extractedRiverMap; // extracted river map

    // common
    public string normalMap;
    public string slopeMap;


}

[System.Serializable]
public class MapJson
{
    public MapInfo[] maps;
}
