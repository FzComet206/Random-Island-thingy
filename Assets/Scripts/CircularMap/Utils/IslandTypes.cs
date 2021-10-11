using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IslandTypes
{
    [System.Serializable]
    public struct IslandOptions
    {

        [Range(500, 2000)] public int islandRadius;
        [Range(1, 20)] public float islandScale;
        [Range(1, 100)] public float heightScale;

        [Header("Radius")] 
        public int ring2Radius;
        public int ring1Radius;
        public int ring0Radius;

        [Header("Radius Splitting")] 
        public int ring2RadiusFractions;
        public int ring1RadiusFractions;
        public int ring0RadiusFractions;

        [Header("Degree Splitting")] 
        public int ring0DegreeFractions;

        [Header("Ring 2 noise settings (Outer Boundary)")]
        public float ring2NoiseScale;
        public float ring2NoiseAmplitude;
    }

    public struct CircularMapdata
    {
        public Vector3[][] heightMap;
        public CircularMapdata(Vector3[][] heightMap)
        {
            this.heightMap = heightMap;
        }
    }
}
