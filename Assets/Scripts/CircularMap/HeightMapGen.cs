using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGen
{
    public static Vector3[][] GenerateHeightMap(IslandTypes.IslandOptions op)
    {
        Noise simplex = new Noise();
        
        // islandRadius
        int radiusMain = op.islandRadius;
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
            radiusFrac0 + radiusFrac1 + radiusFrac2 + radiusFrac2
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
                var (x, y, height) = EvaluateBaseNoise(rValue, aValue, simplex);

                baseMap[r][a] = new Vector3(x, height * 10 * heightScale, y) * islandScale;

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
                var (x, y, height) = EvaluateBaseNoise(rValue, aValue, simplex);

                baseMap[r][a] = new Vector3(x, height * 7 * heightScale, y) * islandScale;

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
                var (x, y, height) = EvaluateBaseNoise(rValue, aValue, simplex);

                if (r > cap - 30)
                {
                    baseMap[r][a] = new Vector3(x, height * 1 * heightScale, y) * islandScale;
                }
                else
                {
                    baseMap[r][a] = new Vector3(x, height * 4 * heightScale, y) * islandScale;
                }

                aValue += a2Step;
            }

            rValue += r2Step;
        }
        
        return baseMap;
    }

    private static (float x, float y, float height) EvaluateBaseNoise(float r0Value, float a0Value, Noise simplex)
    {
        float x = r0Value * Mathf.Cos(a0Value * Mathf.Deg2Rad);
        float y = r0Value * Mathf.Sin(a0Value * Mathf.Deg2Rad);
        float height = ((simplex.Evaluate(x / 60, y / 60) + 1) / 2);
        return (x, y, height);
    }
}
