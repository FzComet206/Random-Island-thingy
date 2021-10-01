using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseValley : MonoBehaviour
{
    public static float[,] EncodeValley(
        float[,] noiseMap,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundLeft,
        int yBoundRight)
    {
        AnimationCurve c = options.valleyOptions.noiseHeightCurve;
        return noiseMap;
    }
}
