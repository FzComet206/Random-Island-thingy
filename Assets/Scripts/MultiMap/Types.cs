using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Types 
{
    [System.Serializable]
    public struct MainMapOptions
    {
        public int seed;
        public float offsetx;
        public float offsety;
        
        [Range(10, 2049)]
        public int mapWidth;
        [Range(10, 2049)]
        public int mapHeight;
        [Range(10, 1000)]
        public float scale;
        [Range(0, 10)]
        public int octaves;
        [Range(0, 0.2f)]
        public float persistance;
        [Range(0, 10f)]
        public float lacunarity;
        [Range(0, 100)]
        public int depth;
        [Range(0, 30)]
        public int heightScale;
        [Range(0, 1)]
        public float negativeClamp;
        [Range(0, 1)]
        public float flattenScale;
        
        public bool useFlatShading;

        public MountainOptions mountainOptions;
        public ValleyOptions valleyOptions;
        public PlainOptions plainOptions;
        public DesertOptions desertOptions;
        
        [Header(" ")]
        [Range(0, 50)] public int rangeOfInterceptingLerp;

    }
    
    [System.Serializable]
    public struct MountainOptions
    {
        public AnimationCurve noiseHeightCurve;
        public AnimationCurve depthPerLayerCurve;
    }

    [System.Serializable]
    public struct ValleyOptions 
    {
        public AnimationCurve noiseHeightCurve;
        public AnimationCurve depthPerLayerCurve;
    }
    
    [System.Serializable]
    public struct PlainOptions 
    {
        public AnimationCurve noiseHeightCurve;
        public AnimationCurve depthPerLayerCurve;
    }
    
    [System.Serializable]
    public struct DesertOptions 
    {
        public AnimationCurve noiseHeightCurve;
        public AnimationCurve depthPerLayerCurve;
    }
}
