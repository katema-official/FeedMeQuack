Shader "Custom/TriangleShader"
{
    Properties
    {
        _FirstColor  ("First Color", Color) = (1.0,0.0,0.0,0.0)
        _SecondColor ("Second Color", Color) = (1.0,1.0,1.0,0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"
            //
            // struct appdata
            // {
            //     float4 vertex : POSITION;
            //     float2 uv : TEXCOORD0;
            // };

        struct v2f
          {
              float4 vertex   : SV_POSITION;
              float2 uv        : TEXCOORD0;
          };

            v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0)
          {
            v2f o;
            o.vertex = UnityObjectToClipPos(pos);
            o.uv = uv * 100;
            return o;
          }

            // struct v2f
            // {
            //     float2 uv : TEXCOORD0;
            //     UNITY_FOG_COORDS(1)
            //     float4 vertex : SV_POSITION;
            // };

          fixed4 _FirstColor;
          fixed4 _SecondColor;

          fixed4 frag(v2f IN) : SV_Target
          {
              float2 c = IN.uv;
              fixed4 cout;

              if (c.y>0.0){
                cout = _FirstColor;
              }
              else
              {
                  cout = _SecondColor;
              }
              return cout;

          }
            
            ENDCG
        }
    }
}
