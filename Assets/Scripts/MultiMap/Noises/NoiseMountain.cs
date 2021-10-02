using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public static class NoiseMountain  
{
    public static float[,] EncodeMountain(
        ref float[,] finalMap,
        ref float[,] noiseRef,
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
        for (int y = yBoundTop; y < yBoundBottom + interpolateIndexRange; y++)
        {
            for (int x = xBoundLeft; x < xBoundRight + interpolateIndexRange; x++)
            {
                if (y > yTrigger && x > xTrigger)
                {
                    // this is where the four terrain intercepts
                    float r1 = 1 - (y - yTrigger) / (2 * (float) interpolateIndexRange);
                    float r2 = 1 - (x - xTrigger) / (2 * (float) interpolateIndexRange);
                    finalMap[x, y] += c.Evaluate(noiseRef[x, y]) * r1 * r2;

                }
                else if (y > yTrigger)
                {
                    // this is the terrain below mountain
                    float ratio = 1 - (y - yTrigger) / (2 * (float) interpolateIndexRange);
                    finalMap[x, y] += c.Evaluate(noiseRef[x, y]) * ratio;
                } 
                else if (x > xTrigger)
                {
                    // this is the terrain right to mountain
                    float ratio = 1 - (x - xTrigger) / (2 * (float) interpolateIndexRange);
                    finalMap[x, y] += c.Evaluate(noiseRef[x, y]) * ratio;
                }
                else
                {
                    // this is only mountain
                    finalMap[x, y] = c.Evaluate(noiseRef[x, y]);
                }
            }
        }
        
        // extend iteration

        
        return finalMap;
    }
}
