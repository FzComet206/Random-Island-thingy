using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseValley : MonoBehaviour
{
    public static float[,] EncodeValley(
        float [,] finalMap,
        float[,] noiseRef,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundTop,
        int yBoundBottom)
    {
        AnimationCurve c = options.valleyOptions.noiseHeightCurve;
        return finalMap;
    }
}
