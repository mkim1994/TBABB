// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "WaterInBottle/Cg_WaterPillar"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "brown" {}
		_WaterHeight("Water Height", Float) = 1
		[Toggle(SHOW_FIRST_PASS)]
		_ShowFirstPass("Show First Pass", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent"}
		LOD 100
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		CGINCLUDE
		#include "UnityCG.cginc"
		half _WaterHeight;

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			
		};
		
		sampler2D _MainTex;
		float4 _MainTex_ST;
		
		struct v2f
		{
			float4 vertex : SV_POSITION;
			float4 worldPos : TEXCOORD1;
			float2 uv : TEXCOORD0;
		};

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);		
			
			return o;
		}

        #pragma shader_feature SHOW_FIRST_PASS
		ENDCG

		Pass{
		
		    
			Cull Front

			Stencil{
				ref 2
				Comp Greater
				Pass Replace
				ZFail Keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				if(i.worldPos.y >= _WaterHeight){
					discard;
				}
				return half4(1,0,0,0);
			}
			ENDCG			
		}

		Pass
		{
			Cull Back

			Stencil{
				ref 0
				Comp Less
				Pass Replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			

			
			
			fixed4 frag (v2f i) : SV_Target
			{
			    half4 col = tex2D(_MainTex, i.uv);
//			    col.a = .5;
				if(i.worldPos.y >= _WaterHeight){
					discard;
				}

				return col;
			}
			ENDCG
		}
	}
}
