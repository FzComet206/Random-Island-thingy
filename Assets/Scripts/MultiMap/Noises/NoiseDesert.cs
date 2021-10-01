using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDesert : MonoBehaviour
{
    public static float[,] EncodeDesert(
        float[,] noiseMap,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundLeft,
        int yBoundRight)
    {
        AnimationCurve c = options.desertOptions.noiseHeightCurve;
        return noiseMap;
    }
}
