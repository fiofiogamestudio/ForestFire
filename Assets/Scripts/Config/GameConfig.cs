using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfig
{
    public static bool USE_STREAM = true;
    public static string HEIGHTMAP_PATH = "Resources/MapTemp/HeightMap";
    public static string RIVERMAP_PATH = "Resources/MapTemp/RiverMap";
    public static string EXTRACTED_RIVERMAP_PATH = "Resources/MapTemp/ExtractedRiverMap";

    public static string VISUALIZED_RIVERMAP_PATH = "Resources/MapTemp/VisualizedRiverMap"; // Visualize
    public static string VISUALED_PATHMAP = "Resources/MapTemp/VisualizedPathMap"; // Visualize

    public static string MODIFIED_HEIGHTMAP_PATH = "Resources/MapTemp/ModifiedHeightMap";

    // Premap
    public static string NORMALMAP_PATH = "Resources/MapTemp/NormalMap";
    public static string SLOPEMAP_PATH = "Resources/MapTemp/SlopeMap";

    public static int MAP_WIDTH = 1024;

    public static float TERRAIN_HEIGHT_SCALE = 1.0f;
    public static float RIVER_HEIGHT_SCALE = 205;


    // Load Map
    public static string MAP_TO_LOAD = "Test";
}
