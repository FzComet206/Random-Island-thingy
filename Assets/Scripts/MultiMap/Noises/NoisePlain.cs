using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoisePlain : MonoBehaviour
{
    public static float[,] EncodePlain(
        float [,] finalMap,
        float[,] noiseRef,
        Types.MainMapOptions options,
        int xBoundLeft,
        int xBoundRight,
        int yBoundTop,
        int yBoundBottom)
    {
        AnimationCurve c = options.plainOptions.noiseHeightCurve;
        return finalMap;
    }
}
