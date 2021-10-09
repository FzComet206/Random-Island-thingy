using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGen
{
    public static Vector3[,] GenerateHeightMap(IslandTypes.IslandOptions op)
    {
        Noise simplex = new Noise();
        
        int outerRadius = op.outerRadius;
        int innerRadius = op.innerRadius;
        
        int outerRadiusFrac = op.outerRadiusFractions;
        int innerRadiusFrac = op.innerRadiusFractions;
        
        int outerDegreeFrac = op.outerDegreeFractions;
        int innerDegreeFrac = op.innerDegreeFractions;
        
        Vector3[,] baseMap = new Vector3[360, 100];
 
        
        // inner ring
        float r2Value = 0;
        float a2Value;

        float r2step = innerRadius / (float) innerRadiusFrac;
        float a2step = 360f / innerDegreeFrac;

        for (int r = 0; r < 100; r++)
        {
            a2Value = 0;
            
            for (int a = 0; a < 360; a++)
            {
                // i fking forgot this is radian mode fml
                float x = r * Mathf.Cos(a * Mathf.Deg2Rad);
                float y = r * Mathf.Sin(a * Mathf.Deg2Rad);
                float height = ((simplex.Evaluate(x / 10, y / 10) + 1) / 2) * 2;

                baseMap[a, r] = new Vector3(x, 0, y);
                
                if (a < 20 && r > 30 && r < 80)
                {
                    baseMap[a, r].y = height;
                }

                a2Value += a2step;
            }

            r2Value += r2step;
        }
        
        return baseMap;
    }
}
