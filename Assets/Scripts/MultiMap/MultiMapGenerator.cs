using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMapGenerator : MonoBehaviour
{
    [Header("Main Map Input Parameters")] [SerializeField]
    public Types.MainMapOptions mainMapOptions;

    public void DrawMesh()
    {
        ActualMapDisplay display = FindObjectOfType<ActualMapDisplay>();
        display.DrawMeshMap(
            MultiMeshGenerator.GenerateTerrainMesh(NoiseMaster.GenerateHeightMap(
                mainMapOptions
                ), mainMapOptions)
            );
    }
}
