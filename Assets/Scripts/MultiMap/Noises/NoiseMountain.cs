using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public static class NoiseMountain  
{
    public static float[,] EncodeMountain(
        float[,] noiseMap,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundLeft,
        int yBoundRight)
    {
        AnimationCurve c = options.mountainOptions.noiseHeightCurve;
        return noiseMap;
    }
}
