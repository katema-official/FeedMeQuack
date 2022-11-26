Shader "Custom/SimpleBar"
{
  Properties
  {
     [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
      [Header(Life)]_FirstColor ("First Color", Color) = (0.2,1,0.2,1)
//      _LifePercent ("Life Percent", Float) = 1

      [Header(Damages)]_SecondColor ("Second color", Color) = (1,1,0,1)
//      _DamagesPercent ("Damages Percent", Float) = 0

//      [Header(Shield)]_ShieldColor ("Shield color", Color) = (.2, .2, 1, 0)
//      _ShieldPercent ("Shield Percent", Float) = 0

  }

  SubShader
  {
      Pass
      {
      CGPROGRAM
          #pragma vertex vert
          #pragma fragment frag
          #include "UnityCG.cginc"

          struct v2f
          {
              float4 vertex   : SV_POSITION;
              float2 uv        : TEXCOORD0;
          };

          fixed4 _FirstColor;
          // half _LifePercent;

          fixed4 _SecondColor;
          // half _DamagesPercent;

          // fixed4 _ShieldColor;
          half _Threshold = 0.0;

          v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0)
          {
            v2f o;
            o.vertex = UnityObjectToClipPos(pos);
            o.uv = uv * 100;
            return o;
          }

          // fixed4 frag(v2f IN) : SV_Target
          // {
          //     float2 c = IN.uv;
          //     fixed4 cout;
          //
          //     if (c.y < _LifePercent){
          //       cout = _LifeColor;
          //     }
          //     else if (c.y > _LifePercent && c.y < _DamagesPercent + _LifePercent){
          //       cout = _DamagesColor;
          //     }
          //     else {
          //       cout = _ShieldColor;
          //     }
          //     return cout;
          //
          // }
      
          fixed4 frag(v2f IN) : SV_Target
          {
              float2 c = IN.uv;
              fixed4 cout;

              if ((c.y+c.x) < 100.0){
                cout = _FirstColor;
              } else {
                  cout = _SecondColor;
              }
              return cout;

          }
      ENDCG
      }
  }
}