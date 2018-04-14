﻿Shader "Custom/AlcoholSurface" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.4
        _LightIntensity("Light Intensity", Range(0.0,3.0)) = 1.3
        _Cube ("Cubemap", CUBE) = "" {}
        _CubeBlur ("Cubemap Blur", Range(0,6)) = 2.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Transparent-4"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

		LOD 100
        
        
		CGINCLUDE
        #include "UnityCG.cginc"

        struct Input {
            float2 uv_MainTex;
            float3 worldRefl;
            float3 viewDir;
            float4 screenPos;
        };

        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);

        }
        

        sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        float _LightIntensity;
        samplerCUBE _Cube;
        float _CubeBlur;

        ENDCG

        //Only draw over the top of the back part of the pillar
        Stencil{
            ref 2
            Comp Equal
            Pass Keep
            Fail DecrSat
        }
        Cull Back

        CGPROGRAM

        #pragma vertex vert
        #pragma surface surf Standard fullforwardshadows 
        #pragma target 4.0
        #include "UnityCG.cginc"

		void surf (Input IN, inout SurfaceOutputStandard o) {
            //sample the albedo texture
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            //add some light reflected from the cubemap
            o.Albedo = texCUBElod (_Cube, float4(IN.worldRefl, _CubeBlur)).rgb ;
            //add some light transmitted through the water from the cubemap
            o.Albedo += texCUBElod (_Cube, float4(-IN.viewDir, _CubeBlur)).rgb * _Color * _LightColor0.rgb*_LightIntensity; //great
            //tint the water 
			o.Albedo *= c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG

   

	}
	FallBack "Diffuse"
}
