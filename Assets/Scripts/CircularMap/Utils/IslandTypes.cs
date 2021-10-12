using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IslandTypes
{
    [System.Serializable]
    public struct IslandOptions
    {
        [Range(1, 20)] public float islandScale;
        [Range(1, 100)] public float heightScale;
        
        [Header("General Noise Settings")]
        public int seed;
        public Vector2 offset;
        [Range(10, 1000)]
        public float scale;
        [Range(0, 10)]
        public int octaves;
        [Range(0, 0.2f)]
        public float persistance;
        [Range(0, 10f)]
        public float lacunarity;

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
        public int ring2BoundaryRadius;
        public float ring2NoiseScale;
        public int ring2NoiseAmplitude;
    }

    public enum BiomeIndex
    {
        Beach,
        Canyon,
        Forest,
        Plain,
        Rocky,
        Mystic,
        Cliff,
        Volcano
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
