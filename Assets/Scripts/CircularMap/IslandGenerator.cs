using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{

    [Header("Map Mod")] [SerializeField] private IslandTypes.IslandOptions IslandConfig;

    public void DrawMesh()
    {
        ActualMapDisplay display = FindObjectOfType<ActualMapDisplay>();
        display.DrawCircularMeshMap(
            IslandMeshGenerator.GenerateTerrainMesh(GenerateMapData().heightMap, IslandConfig)
        );
    }

    IslandTypes.CircularMapdata GenerateMapData()
    {
        return new IslandTypes.CircularMapdata(
            HeightMapGen.GenerateHeightMap(IslandConfig)
        );
    }
}
