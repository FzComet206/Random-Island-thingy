Shader "Unlit/Default"
{
    Properties
    {
        _Value ("Value", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // this includes unity libraries
            #include "UnityCG.cginc"

            float _Value;
            
            struct appdata
            { // this is per vertex mesh data
                float4 vertex : POSITION; // vertex position
                float3 normals : NORMAL; // normal direction of a vertex
                float4 tangent: TANGENT;
                float4 color : COLOR;
                float2 uv : TEXCOORD0; // uv coordinates (can be used for anything)
            };

            // the data that get passed from vertex to frag
            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Interpolators vert (appdata v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
