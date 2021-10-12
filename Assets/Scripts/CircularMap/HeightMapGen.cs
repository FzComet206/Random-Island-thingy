using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class HeightMapGen
{
    // first go
    public static Vector3[][] GenerateHeightMap(IslandTypes.IslandOptions op)
    {
        Noise simplex = new Noise();
        
        // islandRadius
        float islandScale = op.islandScale;
        float heightScale = op.heightScale;
        
        // ring 0 settings
        int radius0 = op.ring0Radius;
        int radiusFrac0 = op.ring0RadiusFractions;
        int degreeFrac0 = op.ring0DegreeFractions;
        
        // ring 1 settings
        int radius1 = op.ring1Radius;
        int radiusFrac1 = op.ring1RadiusFractions;
        int degreeFrac1 = degreeFrac0 * 2;
        
        // ring 2 settings
        int radius2 = op.ring2Radius;
        int radiusFrac2 = op.ring2RadiusFractions;
        int degreeFrac2 = degreeFrac1 * 2;
        
        float noiseAmplitude2 = op.ring2NoiseAmplitude;
        float noiseScale2 = op.ring2NoiseAmplitude;

        // init array # same as total number of radius fractions for each ring
        Vector3[][] baseMap = new Vector3[
            radiusFrac0 + radiusFrac1 + radiusFrac2
        ][];
        
        // ring 0
        float rValue = 0;
        float aValue;

        float r0Step = radius0 / (float) radiusFrac0;
        float a0Step = 360f / degreeFrac0;
        
        float r1Step = (radius1 - radius0) / (float) radiusFrac1;
        float a1Step = 360f / degreeFrac1;

        float r2Step = (radius2 - radius1) / (float) radiusFrac2;
        float a2Step = 360f / degreeFrac2;
        
        // outermost ring have the same radius and degree frac as ring 2
        
        for (int r = 0; r < radiusFrac0; r++)
        {
            aValue = 0;
            baseMap[r] = new Vector3[degreeFrac0];
            
            for (int a = 0; a < degreeFrac0; a++)
            {
                var (x, y) = EvaluatePositionInWorld(rValue, aValue);
                baseMap[r][a] = new Vector3(x, 6, y) * islandScale;

                aValue += a0Step;
            }

            rValue += r0Step;
        }

        int bot = radiusFrac0;
        int cap = bot + radiusFrac1;
        for (int r = bot; r < cap; r++)
        {
            aValue = 0;
            baseMap[r] = new Vector3[degreeFrac1];
            
            for (int a = 0; a < degreeFrac1; a++)
            {
                var (x, y) = EvaluatePositionInWorld(rValue, aValue);
                baseMap[r][a] = new Vector3(x, 3, y) * islandScale;

                aValue += a1Step;
            }

            rValue += r1Step;
        }


        bot = cap;
        cap = bot + radiusFrac2;
        for (int r = bot; r < cap; r++)
        {
            aValue = 0;
            baseMap[r] = new Vector3[degreeFrac2];
            
            for (int a = 0; a < degreeFrac2; a++)
            {
                var (x, y) = EvaluatePositionInWorld(rValue, aValue);

                if (r > cap - 30)
                {
                    baseMap[r][a] = new Vector3(x, 1, y) * islandScale;
                }
                else
                {
                    baseMap[r][a] = new Vector3(x, 1, y) * islandScale;
                }

                aValue += a2Step;
            }

            rValue += r2Step;
        }

        Noise n = new Noise();
        System.Random prng = new System.Random(op.seed);
        Vector2[] octavesOffset = new Vector2[op.octaves];
        for (int i = 0; i < op.octaves; i++)
        {
            // scrolling and random octaves
            float offsetXOct = prng.Next(-100000, 100000) + op.offset.x;
            float offsetYOct = prng.Next(-100000, 100000) + op.offset.y;
            octavesOffset[i] = new Vector2(offsetXOct, offsetYOct);
        }
        
        // input radius and spits out angle bounds
        Dictionary<int, int[]> radiusLookUp = new Dictionary<int, int[]>();
        // input angle and spits out radius bounds
        Dictionary<int, int[]> angleLookUp = new Dictionary<int, int[]>();
        
        // iterate and spits out boundary values stored in hash look ups
        ProcessBoundaryAndHashMap(ref baseMap, op, simplex, prng, octavesOffset, ref radiusLookUp, ref angleLookUp);
        // iterate and use hash look ups to spits out modified heights
        
        return baseMap;
    }

    private static void ProcessBoundaryAndHashMap(
        ref Vector3[][] baseMap, 
        IslandTypes.IslandOptions op,
        Noise simplex,
        System.Random prng,
        Vector2[] octOffset,
        ref Dictionary<int, int[]> radiusLookUp, 
        ref Dictionary<int, int[]> angleLookUp)
    {
        int x1 = prng.Next(-100000, 100000);
        int y1 = prng.Next(-100000, 100000);
        
        int x2 = prng.Next(-100000, 100000);
        int y2 = prng.Next(-100000, 100000);
        
        // this generate first ring boundary into hashmap
        int r2 = op.ring2BoundaryRadius;
        int a2 = baseMap[r2].Length;
        for (int i = 0; i < a2; i++)
        {
            EvaluateRadiusBoundaryCurve(baseMap, op, simplex, octOffset, angleLookUp, r2, a2, i, x1, y1, x2, y2);
        }
        
        // this generate second ring boundary into hashmap
        int r1 = op.ring1BoundaryRadius;
        int a1 = baseMap[r1].Length;
        for (int i = 0; i < a1; i++)
        {
            EvaluateRadiusBoundaryCurve(baseMap, op, simplex, octOffset, angleLookUp, r1, a1, i, x1, y1, x2, y2);
        }

    }

    private static void EvaluateRadiusBoundaryCurve(
        Vector3[][] baseMap, IslandTypes.IslandOptions op, Noise simplex, Vector2[] octOffset,
        Dictionary<int, int[]> angleLookUp, int r, int a, int i, int x1, int y1, int x2, int y2)
    {
        float scale;
        int amplitude;
        if (r == op.ring2BoundaryRadius)
        {
            scale = op.ring2NoiseScale;
            amplitude = op.ring2NoiseAmplitude;
        }
        else
        {
            scale = op.ring1NoiseScale;
            amplitude = op.ring1NoiseAmplitude;
        }
        
        // world position into noise function
        var (x, y) = EvaluatePositionInWorld(r, 360f / a * i);

        // smaller layer
        int offset = Mathf.FloorToInt(
            1 - Mathf.Abs(
                EvaluateHeightInWorld(x + x1, y + y1, simplex, octOffset, op, scale)) * amplitude);

        // larger layer
        offset += Mathf.FloorToInt(
            1 - Mathf.Abs(
                EvaluateHeightInWorld(x + x2, y + y2, simplex, octOffset, op, scale * 3)) * amplitude);

        if (r + offset > op.ring1RadiusFractions + op.ring0RadiusFractions)
        {
            // this means the angle is a0 * 2 * 2
            if (angleLookUp.ContainsKey(i))
            {
                angleLookUp[i][1] = r + offset;
            }
            else
            {
                angleLookUp[i] = new[] {r + offset, 0};
            }
            
            baseMap[r + offset][i].y = 10;
            
        } else if (r + offset > op.ring0RadiusFractions)
        {
            // this means the angle is a0 * 2
            int frac = Mathf.FloorToInt(i / 2f);
            
            if (angleLookUp.ContainsKey(i))
            {
                angleLookUp[frac][1] = r + offset;
            }
            else
            {
                angleLookUp[frac] = new []{r + offset, 0};
            }
            
            baseMap[r + offset][frac].y = 10;
        }
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    // second go
    private static void ProcessHeightAndBiome(
        ref Vector3[][] baseMap, 
        Noise simplex, 
        ref Dictionary<int, int> radiusLookup, 
        ref Dictionary<int, int> angleLookup)
    {
        // third iteration to decide heights
        int rFrac = baseMap.Length;
        for (int i = 0; i < rFrac; i++)
        {
            int aFrac = baseMap[i].Length;
            
            for (int j = 0; j < aFrac; j++)
            {
                
            }
        }
    }
    
    // helper function for process biome
    private static IslandTypes.BiomeIndex EvaluateBiomeType(
        int r, 
        int a, 
        ref Dictionary<int, int> radiusLookUp,
        ref Dictionary<int, int> angleLookUp)
    {
        return IslandTypes.BiomeIndex.Beach;
    }

    // helper function for converting position
    private static (float x, float y) EvaluatePositionInWorld(float rValue, float aValue)
    {
        float x = rValue * Mathf.Cos(aValue * Mathf.Deg2Rad);
        float y = rValue * Mathf.Sin(aValue * Mathf.Deg2Rad);
        return (x, y);
    }

    // this one spits out the general noise height
    private static float EvaluateHeightInWorld(
        float x, 
        float y, 
        Noise simplex, 
        Vector2[] octavesOffset, 
        IslandTypes.IslandOptions nm,
        float scale = 0)
    {
        var sc = nm.scale;
        if (scale > 1) { sc = scale; }

        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < nm.octaves; i++)
        {
            // apply octaves sample and offsets with scale and freq
            float sampleX = (x + octavesOffset[i].x) / sc * frequency;
            float sampleY = (y + octavesOffset[i].y) / sc * frequency;

            // simplex value of range -1 to 1 sample
            float simplxValue = simplex.Evaluate(sampleX, sampleY);

            noiseHeight += simplxValue * amplitude;

            // update frequence and amplitude with persistance and lacunarity
            amplitude *= nm.persistance;
            frequency *= nm.lacunarity;
        }

        return noiseHeight;
    }

}
