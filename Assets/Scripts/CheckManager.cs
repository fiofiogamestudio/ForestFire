using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckManager : MonoBehaviour
{
    public Terrain TerrainToCheck;

    public Material TerrainMaterial;

    public Material HeightMaterial;
    public Material NormalMaterial;

    public Material FlowMaterial;

    public Material FireMaterial;

    public Material FuelMaterial;

    public Color CheckColor;

    [ContextMenu("Check Terrain")]
    public void ChecKTerrain()
    {
        TerrainToCheck.materialTemplate = TerrainMaterial;
    }
    [ContextMenu("Check Height")]
    public void CheckHeight()
    {
        TerrainToCheck.materialTemplate = HeightMaterial;
    }

    [ContextMenu("Check Normal")]
    public void CheckNormal()
    {
        TerrainToCheck.materialTemplate = NormalMaterial;
    }

    [ContextMenu("Check Flow")]
    public void CheckSpat()
    {
        TerrainToCheck.materialTemplate = FlowMaterial;
    }

    [ContextMenu("Check Fire")]
    public void CheckFire()
    {
        TerrainToCheck.materialTemplate = FireMaterial;
    }

    [ContextMenu("Check Fuel")]
    public void CheckFuel()
    {
        TerrainToCheck.materialTemplate = FuelMaterial;
    }
}

