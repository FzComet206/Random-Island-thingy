using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public static class NoiseMountain  
{
    public static float[,] EncodeMountain(
        float[,] finalMap,
        float[,] noiseRef,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundTop,
        int yBoundBottom)
    {
        AnimationCurve c = options.mountainOptions.noiseHeightCurve;

        int xRange = xBoundRight - xBoundLeft;
        int interpolateIndexRange = Mathf.FloorToInt((xRange) * (options.rangeOfInterceptingLerp / 100f));

        int yTrigger = yBoundBottom - interpolateIndexRange;
        int xTrigger = xBoundRight - interpolateIndexRange;
        
        // top-left block
        for (int y = 0; y < yBoundTop; y++)
        {
            for (int x = 0; x < xBoundLeft; x++)
            {
                if (y > yTrigger && x > xTrigger)
                {
                    // this is where the four terrain intercepts
                }
                else if (y > yTrigger)
                {
                    // this is the terrain below mountain
                } 
                else if (x > xTrigger)
                {
                    // this is the terrain right to mountain
                }
                else
                {
                    // this is only mountain
                }
            }
        }
        
        return finalMap;
    }
}
