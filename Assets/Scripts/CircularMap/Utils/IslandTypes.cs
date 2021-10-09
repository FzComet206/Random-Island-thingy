using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IslandTypes
{
    [System.Serializable]
    public struct IslandOptions
    {
        [Range(360, 3600)]
        public int outerDegreeFractions;

        [Range(1, 2400)]
        public int outerRadiusFractions;
        
        [Range(360, 3600)]
        public int innerDegreeFractions;
        
        [Range(1, 2400)]
        public int innerRadiusFractions;
        
        [Range(10, 1200)]
        public int outerRadius;
        
        [Range(10, 1200)]
        public int innerRadius;
        
    }

}
