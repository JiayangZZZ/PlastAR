﻿Shader "Custom/Hologram" {
   
    Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_Outline ("Outline width", Range (0.0, 0.03)) = .005
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _maxHeight ("Height", float) = 1
    }
   
    SubShader {
     Tags {"Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 200
        Pass {
           Blend One One
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 texcoord : TEXCOORD0;

                };
 
                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 color : COLOR;
                    float3 lpos : TEXCOORD2;
                    
                    
                };
 
                uniform float4 _MainTex_ST;
                uniform float4 _RimColor;
                 float _Shininess;
				uniform float _Outline;

                v2f vert (appdata_base v) {
                    v2f o;
                    o.lpos = v.vertex;
                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					//float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
					//float2 offset = TransformViewToProjection(norm.xy);
					//o.pos.xy += offset * o.pos.z * _Outline;
					float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
					float2 offset = TransformViewToProjection(norm.xy);
					o.pos.xy += offset  * _Outline;

                    float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                    float dotProduct =( 1 - dot(v.normal, viewDir))*_Shininess;
                    float rimWidth = 0.7;
                    o.color = smoothstep(1 - rimWidth, 1.0, dotProduct);
                    o.color *= (_RimColor);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    
                    return o;
                }
 
                uniform sampler2D _MainTex;
                uniform float4 _Color;
                uniform sampler2D _Noise;
                uniform float _maxHeight;
 
                float4 frag(v2f i) : COLOR {
                    
                    float offset = ((tex2D(_Noise, float2(_Time[1] / 20, (i.lpos.x / _maxHeight))).r) / 10 + 0.01);

                    float4 texcol = tex2D(_MainTex, i.uv);
                    texcol *= _Color / (i.lpos.y / _maxHeight) * 2  - offset;
                    // texcol *= _Color;
                    texcol.rgb += i.color;
                    // texcol.a = _Color.a;
                    return texcol;
                }
 
            ENDCG
        }
    }
}