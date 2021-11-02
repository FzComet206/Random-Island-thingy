using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = System.Random;

public static class HeightMapGen
{
    // first go
    public static Vector3[][] GenerateHeightMap(IslandTypes.IslandOptions op, ref Texture2D textureEncodingOne, ref Texture2D textureEncodingTwo)
    {
        Noise simplex = new Noise();
        
        // islandRadius
        float islandScale = op.islandScale;
        
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
                baseMap[r][a] = new Vector3(x, 0, y) * islandScale;

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
                baseMap[r][a] = new Vector3(x, 0, y) * islandScale;

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
                    baseMap[r][a] = new Vector3(x, 0, y) * islandScale;
                }
                else
                {
                    baseMap[r][a] = new Vector3(x, 0, y) * islandScale;
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
        Dictionary<int, int[]> radiusLookUpOuter = new Dictionary<int, int[]>();
        Dictionary<int, int[]> radiusLookUpInner = new Dictionary<int, int[]>();
        // input angle and spits out radius bounds
        Dictionary<int, int> angleLookUpOuter = new Dictionary<int, int>();
        Dictionary<int, int> angleLookUpMiddle = new Dictionary<int, int>();
        Dictionary<int, int> angleLookUpInner = new Dictionary<int, int>();
        
        // iterate and spits out boundary values stored in hash look ups
        ProcessBoundaryAndHashMap(ref baseMap, op, simplex, prng, octavesOffset, 
            radiusLookUpOuter, 
            radiusLookUpInner,
            angleLookUpOuter, 
            angleLookUpMiddle,
            angleLookUpInner);
        // iterate and use hash look ups to spits out modified heights

        ProcessHeightAndBiome(ref baseMap, n, octavesOffset,
            op,
            radiusLookUpOuter,
            radiusLookUpInner,
            angleLookUpOuter,
            angleLookUpMiddle,
            angleLookUpInner,
            ref textureEncodingOne,
            ref textureEncodingTwo);

        return baseMap;
    }

    private static void ProcessBoundaryAndHashMap(
        ref Vector3[][] baseMap,
        IslandTypes.IslandOptions op,
        Noise simplex,
        System.Random prng,
        Vector2[] octOffset,
        Dictionary<int, int[]> radiusLookUpOuter,
        Dictionary<int, int[]> radiusLookUpInner,
        Dictionary<int, int> angleLookUpOuter,
        Dictionary<int, int> angleLookUpMiddle,
        Dictionary<int, int> angleLookUpInner
        )
    {
        int x1 = prng.Next(-100000, 100000);
        int y1 = prng.Next(-100000, 100000);
        int x2 = prng.Next(-100000, 100000);
        int y2 = prng.Next(-100000, 100000);

        // this generate ring 2 boundary into hashmap
        int r2 = op.ring2BoundaryRadius;
        int a2 = baseMap[r2 - 1].Length;
        for (int i = 0; i < a2; i++)
        {
            EvaluateRadiusBoundaryCurve(baseMap, op, simplex, octOffset, angleLookUpOuter, r2, a2, i, x1, y1, x2, y2);
        }
        
        x1 = prng.Next(-100000, 100000);
        y1 = prng.Next(-100000, 100000);
        x2 = prng.Next(-100000, 100000);
        y2 = prng.Next(-100000, 100000);

        // this generate ring 1 boundary into hashmap
        int r1 = op.ring1BoundaryRadius;
        int a1 = baseMap[r1 - 1].Length;
        
        for (int i = 0; i < a1; i++)
        {
            EvaluateRadiusBoundaryCurve(baseMap, op, simplex, octOffset, angleLookUpMiddle, r1, a1, i, x1, y1, x2, y2);
        }
        
        x1 = prng.Next(-100000, 100000);
        y1 = prng.Next(-100000, 100000);
        x2 = prng.Next(-100000, 100000);
        y2 = prng.Next(-100000, 100000);
        
        // this generate ring 0 boundary into hashmap
        int r0 = op.ring0BoundaryRadius;
        int a0 = baseMap[r0 - 1].Length;
        
        for (int i = 0; i < a0; i++)
        {
            EvaluateRadiusBoundaryCurve(baseMap, op, simplex, octOffset, angleLookUpInner, r0, a0, i, x1, y1, x2, y2);
        }
        
        x1 = prng.Next(-100000, 100000);
        y1 = prng.Next(-100000, 100000);
        x2 = prng.Next(-100000, 100000);
        y2 = prng.Next(-100000, 100000);

        // this generate angle boundary for outer loop into hashmap
        // trace from outer ring until hit inner ring
        // each radius decrease offset the angle, radius -= 1, angle += offset
        // while current radius > angleLookup(current angle)
        // if the current radius is less than ring 3, angle / 2 and angle lookupinner
        // if i + offset < 0, i = i + basemap[r].length + offset
        
        int a3 = op.ring0DegreeFractions * 4;
        int frac1 = Mathf.FloorToInt(a3 / 4f);
        int areaIndex0 = 0;
        for (int i = a3 / 6; i < a3; i += frac1)
        {
            EvaluateDegreeBoundaryCurve(op, simplex, octOffset, radiusLookUpOuter, i, a3, x1, y1, x2, y2, areaIndex0);
            areaIndex0++;
        }
        
        x1 = prng.Next(-100000, 100000);
        y1 = prng.Next(-100000, 100000);
        x2 = prng.Next(-100000, 100000);
        y2 = prng.Next(-100000, 100000);
        
        // this generate angle boundary for inner loop into hashmap
        int a4 = op.ring0DegreeFractions * 2;
        int frac0 = Mathf.FloorToInt(a4 / 3f);
        int areaIndex1 = 0;
        for (int i = a4 / 6; i < a4; i += frac0)
        {
            areaIndex1 = EvaluateInnerDegreeBoundaryCurve(op, simplex, octOffset, radiusLookUpInner, i, a3, x1, y1, x2, y2, areaIndex1);
        }
    }

    // next 100 lines are un-refactored shit
    private static int EvaluateInnerDegreeBoundaryCurve(IslandTypes.IslandOptions op, Noise simplex, Vector2[] octOffset,
        Dictionary<int, int[]> radiusLookUpInner, int i, int a3, int x1,
        int y1, int x2, int y2, int areaIndex1)
    {
        int currentR = op.ring0RadiusFractions + op.ring1RadiusFractions;
        int currentA = i;
        int targetR = 1;

        while (currentR > targetR)
        {
            var (x, y) = EvaluatePositionInWorld(currentR, 360f / a3 * currentA);

            int offset = Mathf.RoundToInt(
                EvaluateHeightInWorld(x + x1, y + y1, simplex, octOffset, op, op.innerBoundaryAngleScale)
                * op.innerBoundaryAngleAmplitude);

            offset += Mathf.RoundToInt(
                EvaluateHeightInWorld(x + x2, y + y2, simplex, octOffset, op, op.innerBoundaryAngleScale * 3)
                * op.innerBoundaryAngleAmplitude);

            currentA = currentA + offset;
            if (currentR < op.ring0RadiusFractions)
            {
                if (radiusLookUpInner.ContainsKey(currentR))
                {
                    radiusLookUpInner[currentR][areaIndex1] = Mathf.FloorToInt(currentA / 2f);
                }
                else
                {
                    radiusLookUpInner[currentR] = new int[4];
                    radiusLookUpInner[currentR][areaIndex1] = Mathf.FloorToInt(currentA / 2f);
                }
            }
            else
            {
                if (radiusLookUpInner.ContainsKey(currentR))
                {
                    radiusLookUpInner[currentR][areaIndex1] = currentA;
                }
                else
                {
                    radiusLookUpInner[currentR] = new int[4];
                    radiusLookUpInner[currentR][areaIndex1] = currentA;
                }
            }
            currentR -= 1;
        }

        areaIndex1++;
        return areaIndex1;
    }

    private static void EvaluateDegreeBoundaryCurve(
        IslandTypes.IslandOptions op, Noise simplex, Vector2[] octOffset,
        Dictionary<int, int[]> radiusLookUp,
        int i, int a3, int x1, int y1,
        int x2, int y2, int areaIndex)
    {
        int currentR = op.ring0RadiusFractions + op.ring1RadiusFractions + op.ring2RadiusFractions;
        int currentA = i;
        int targetR = op.ring0RadiusFractions;

        while (currentR > targetR)
        {
            var (x, y) = EvaluatePositionInWorld(currentR, 360f / a3 * currentA);

            int offset = Mathf.RoundToInt(
                EvaluateHeightInWorld(x + x1, y + y1, simplex, octOffset, op, op.outerBoundaryAngleScale)
                * op.outerBoundaryAngleAmplitude);

            offset += Mathf.RoundToInt(
                EvaluateHeightInWorld(x + x2, y + y2, simplex, octOffset, op, op.outerBoundaryAngleScale * 3)
                * op.outerBoundaryAngleAmplitude);

            currentA = currentA + offset;
            if (currentR < op.ring0RadiusFractions + op.ring1RadiusFractions)
            {
                if (radiusLookUp.ContainsKey(currentR))
                {
                    radiusLookUp[currentR][areaIndex] = Mathf.FloorToInt(currentA / 2f);
                }
                else
                {
                    radiusLookUp[currentR] = new int[4];
                    radiusLookUp[currentR][areaIndex] = Mathf.FloorToInt(currentA / 2f);
                }
            }
            else
            {
                if (radiusLookUp.ContainsKey(currentR))
                {
                    radiusLookUp[currentR][areaIndex] = currentA;
                }
                else
                {
                    radiusLookUp[currentR] = new int[4];
                    radiusLookUp[currentR][areaIndex] = currentA;
                }
            }
            
            currentR -= 1;
        }
    }

    private static void EvaluateRadiusBoundaryCurve(
        Vector3[][] baseMap, IslandTypes.IslandOptions op, Noise simplex, Vector2[] octOffset,
        Dictionary<int, int> angleLookUp, int r, int a, int i, int x1, int y1, int x2, int y2)
    {
        float scale;
        int amplitude;
        
        if (r == op.ring2BoundaryRadius)
        {
            scale = op.ring2NoiseScale;
            amplitude = op.ring2NoiseAmplitude;
        }
        else if (r == op.ring1BoundaryRadius)
        {
            scale = op.ring1NoiseScale;
            amplitude = op.ring1NoiseAmplitude;
        }
        else
        {
            scale = op.ring0NoiseScale;
            amplitude = op.ring0NoiseAmplitude;
        }

        // world position into noise function
        var (x, y) = EvaluatePositionInWorld(r, 360f / a * i);

        // smaller layer
        int offset = Mathf.RoundToInt(
            1 - Mathf.Abs(
                EvaluateHeightInWorld(x + x1, y + y1, simplex, octOffset, op, scale)) * amplitude);

        // larger layer
        offset += Mathf.RoundToInt(
            1 - Mathf.Abs(
                EvaluateHeightInWorld(x + x2, y + y2, simplex, octOffset, op, scale * 3)) * amplitude);

        angleLookUp[i] = r + offset;
    }

    private static IslandTypes.BiomeIndex DetermineBiome(int r, int a,
        Dictionary<int, int[]> radiusLookUpOuter,
        Dictionary<int, int[]> radiusLookUpInner,
        Dictionary<int, int> angleLookUpOuter,
        Dictionary<int, int> angleLookUpMiddle,
        Dictionary<int, int> angleLookUpInner,
        IslandTypes.IslandOptions op
    )
    {
        int one = op.ring0RadiusFractions;
        int two = op.ring0RadiusFractions + op.ring1RadiusFractions;
        int three = op.ring0RadiusFractions + op.ring1RadiusFractions + op.ring2RadiusFractions;

        int _a = a;
        
        if (r < two && r >= one)
        {
            _a = a * 2;
        } else if (r < one)
        {
            _a = a * 4;
        }

        if (r < angleLookUpOuter[_a] && r >= angleLookUpMiddle[_a / 2])
        {
            if (a < radiusLookUpOuter[r][0] || a >= radiusLookUpOuter[r][3])
            {
                return IslandTypes.BiomeIndex.Forest;
            }
            
            if (a < radiusLookUpOuter[r][1] && a >= radiusLookUpOuter[r][0])
            {
                return IslandTypes.BiomeIndex.Beach;
            } 
            
            if (a < radiusLookUpOuter[r][2] && a >= radiusLookUpOuter[r][1])
            {
                return IslandTypes.BiomeIndex.Plain;
            }
            
            if (a < radiusLookUpOuter[r][3] && a >= radiusLookUpOuter[r][2])
            {
                return IslandTypes.BiomeIndex.Rocky;
            }
        }

        if (r < angleLookUpMiddle[_a / 2] && r >= angleLookUpInner[_a / 4])
        {
            if (a < radiusLookUpInner[r][0] || a >= radiusLookUpInner[r][2])
            {
                return IslandTypes.BiomeIndex.Canyon;
            } 
            
            if (a < radiusLookUpInner[r][1] && a >= radiusLookUpInner[r][0])
            {
                return IslandTypes.BiomeIndex.Mystic;
            }
            
            if (a < radiusLookUpInner[r][2] && a >= radiusLookUpInner[r][1])
            {
                return IslandTypes.BiomeIndex.Cliff;
            }
            
        }

        if (r < angleLookUpInner[_a / 4])
        {
            return IslandTypes.BiomeIndex.Volcano;
        }
        
        return IslandTypes.BiomeIndex.Ocean;
    }

    // second go
    private static void ProcessHeightAndBiome(
        ref Vector3[][] baseMap, 
        Noise simplex, 
        Vector2[] octOffset,
        IslandTypes.IslandOptions op,
        Dictionary<int, int[]> radiusLookUpOuter,
        Dictionary<int, int[]> radiusLookUpInner,
        Dictionary<int, int> angleLookUpOuter,
        Dictionary<int, int> angleLookUpMiddle,
        Dictionary<int, int> angleLookUpInner,
        ref Texture2D textureFirstFive,
        ref Texture2D textureLastFour)
    {
        Random rand = new Random();
        IslandTypes.BiomeCurveSettings curves = op.biomes;
        
        // third iteration to decide heights
        int rFrac = baseMap.Length;
        for (int i = 0; i < rFrac; i++)
        {
            int aFrac = baseMap[i].Length;
            
            for (int j = 0; j < aFrac; j++)
            {
                if (i < angleLookUpOuter[j])
                {
                    var (x, y) = EvaluatePositionInWorld(i, 360f / aFrac * (j + 1));
                    
                    // hacks
                    float someInterestingContours = EvaluateHeightInWorld(x + 1000, y + 1000, simplex, octOffset, op, 150) * 70;
                    float heightPlus = Mathf.Abs(i + someInterestingContours - rFrac) / (float) rFrac;
                    heightPlus = op.curve.Evaluate(heightPlus);
                    heightPlus = Mathf.Round(Mathf.Lerp(0, 12, heightPlus));

                    float height = (EvaluateHeightInWorld(x, y, simplex, octOffset, op) + 1) / 2;
                    
                    switch (DetermineBiome(i, j, 
                        radiusLookUpOuter,
                        radiusLookUpInner,
                        angleLookUpOuter,
                        angleLookUpMiddle,
                        angleLookUpInner,
                        op
                        ))
                    {
                        case IslandTypes.BiomeIndex.Ocean:
                            break;
                        case IslandTypes.BiomeIndex.Forest:
                            textureFirstFive.SetPixel(i, j, new Color(1, 0, 0, 0));
                            height = curves.Forest.Evaluate(height);
                            break;
                        case IslandTypes.BiomeIndex.Beach:
                            textureFirstFive.SetPixel(i, j, new Color(0, 1, 0, 0));
                            height = curves.Beach.Evaluate(height);
                            break;
                        case IslandTypes.BiomeIndex.Plain:
                            textureFirstFive.SetPixel(i, j, new Color(0, 0, 1, 0));
                            height = curves.Plain.Evaluate(height);
                            break;
                        case IslandTypes.BiomeIndex.Rocky:
                            textureFirstFive.SetPixel(i, j, new Color(0, 0, 0, 1));
                            height = curves.Rocky.Evaluate(height);
                            break;
                        case IslandTypes.BiomeIndex.Canyon:
                            textureLastFour.SetPixel(i, j, new Color(1, 0, 0, 0));
                            height = curves.Canyon.Evaluate(height);
                            break;
                        case IslandTypes.BiomeIndex.Mystic:
                            textureLastFour.SetPixel(i, j, new Color(0, 1, 0, 0));
                            height = curves.Mystic.Evaluate(height);
                            break;
                        case IslandTypes.BiomeIndex.Cliff:
                            textureLastFour.SetPixel(i, j, new Color(0, 0, 1, 0));
                            height = curves.Cliff.Evaluate(height);
                            break;
                        case IslandTypes.BiomeIndex.Volcano:
                            textureLastFour.SetPixel(i, j, new Color(0, 0, 0, 1));
                            height = curves.Volcano.Evaluate(height);
                            break;
                    }
                    
                    baseMap[i][j].y = (height + heightPlus / 3) * op.heightScale + op.heightScale;
                    
                }
                else
                {
                    textureFirstFive.SetPixel(i, j, new Color(0, 0, 0, 0));
                    baseMap[i][j].y = 5;
                }
            }
        }
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
