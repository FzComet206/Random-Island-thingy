using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    public Material mat;
    
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
        int r = IslandConfig.ring0RadiusFractions + 
                IslandConfig.ring1RadiusFractions +
                IslandConfig.ring2RadiusFractions;
        int w = IslandConfig.ring0DegreeFractions * 4;

        Texture2D textureOne = new Texture2D(r, w);
        Texture2D textureTwo = new Texture2D(r, w);
        
        IslandTypes.CircularMapdata mp = new IslandTypes.CircularMapdata(
            HeightMapGen.GenerateHeightMap(IslandConfig, ref textureOne, ref textureTwo)
        );

        TextureEncoder.WriteTextureData(textureOne, textureTwo);
        
        return mp;
    }
}
