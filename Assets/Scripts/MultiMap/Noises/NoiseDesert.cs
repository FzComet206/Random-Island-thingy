using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDesert : MonoBehaviour
{
    public static float[,] EncodeDesert(
        ref float[,] finalMap,
        ref float[,] noiseRef,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundTop,
        int yBoundBottom)
    {
        AnimationCurve c = options.desertOptions.noiseHeightCurve;
        
        int xRange = xBoundRight - xBoundLeft;
        int interpolateIndexRange = Mathf.FloorToInt((xRange) * (options.rangeOfInterceptingLerp / 100f));

        int yTrigger = yBoundTop + interpolateIndexRange;
        int xTrigger = xBoundLeft + interpolateIndexRange;
        
        // bottom-right block
        for (int y = yBoundTop - interpolateIndexRange; y < yBoundBottom; y++)
        {
            for (int x = xBoundLeft - interpolateIndexRange; x < xBoundRight; x++)
            {
                if (y < yTrigger && x < xTrigger)
                {
                    float r1 = 1 - (yTrigger - y) / (2 * (float) interpolateIndexRange);
                    float r2 = 1 - (xTrigger - x) / (2 * (float) interpolateIndexRange);
                    finalMap[x, y] += c.Evaluate(noiseRef[x, y]) * r1 * r2;

                }
                else if (y < yTrigger)
                {
                    // this is the terrain below mountain
                    float ratio = 1 - (yTrigger - y) / (2 * (float) interpolateIndexRange);
                    finalMap[x, y] += c.Evaluate(noiseRef[x, y]) * ratio;
                } 
                else if (x < xTrigger)
                {
                    // this is the terrain right to mountain
                    float ratio = 1 - (xTrigger - x) / (2 * (float) interpolateIndexRange);
                    finalMap[x, y] += c.Evaluate(noiseRef[x, y]) * ratio;
                }
                else
                {
                    // this is only mountain
                    finalMap[x, y] = c.Evaluate(noiseRef[x, y]);
                }
            }
        }
        
        return finalMap;
    }
}
