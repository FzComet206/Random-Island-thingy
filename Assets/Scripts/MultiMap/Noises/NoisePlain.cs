using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisePlain : MonoBehaviour
{
    public static float[,] EncodePlain(
        float[,] noiseMap,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundLeft,
        int yBoundRight)
    {
        AnimationCurve c = options.plainOptions.noiseHeightCurve;
        return noiseMap;
    }
}
