using System;
using System.Threading;
using UnityEngine;

public class MultiMapGenerator : MonoBehaviour
{
    [Header("Main Map Input Parameters")] [SerializeField]
    public Types.MainMapOptions mainMapOptions;

    public void DrawMesh()
    {
        ActualMapDisplay display = FindObjectOfType<ActualMapDisplay>();
        display.DrawMeshMap(
            MultiMeshGenerator.GenerateTerrainMesh(GenerateMapData().heightMap, mainMapOptions)
        );
    }

    Types.MapData GenerateMapData()
    {
        return new Types.MapData(
            NoiseMaster.GenerateHeightMap(mainMapOptions)
        );
    }

}
