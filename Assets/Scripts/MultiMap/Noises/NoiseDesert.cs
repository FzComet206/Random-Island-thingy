using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDesert : MonoBehaviour
{
    public static float[,] EncodeDesert(
        float [,] finalMap,
        float[,] noiseRef,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundTop,
        int yBoundBottom)
    {
        AnimationCurve c = options.desertOptions.noiseHeightCurve;
        return finalMap;
    }
}
