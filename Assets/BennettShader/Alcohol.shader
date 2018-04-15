Shader "Custom/Alcohol" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.4
        _WaterHeight("Water Height", Float) = 1
        _LightIntensity("Light Intensity", Range(0.0,3.0)) = 1.3
        _Cube ("Cubemap", CUBE) = "" {}
        _CubeBlur ("Cubemap Blur", Range(0,6)) = 2.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Transparent-5"}
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        CGINCLUDE
        #include "UnityCG.cginc"
	      
		sampler2D _MainTex;
        half _WaterHeight;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _LightIntensity;
        samplerCUBE _Cube;
        float _CubeBlur;
        float4x4 _MatrixToSurface;

		struct Input {
            float2 uv_MainTex;
            float3 worldPos : TEXCOORD1;
            float3 worldRefl;
            float3 viewDir;
            float4 screenPos;
            float3 localPos;
        };

		void vert (inout appdata_full v, out Input o) {
		    UNITY_INITIALIZE_OUTPUT(Input,o);
		    o.localPos = v.vertex.xyz;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);

		}
        
        ENDCG

        Cull Front

        Stencil{
            ref 2
            Comp Greater
            Pass Replace
            ZFail Keep
        }

        CGPROGRAM

        #pragma vertex vert
        #pragma surface surf Standard fullforwardshadows 
        #pragma target 4.0
        #include "UnityCG.cginc"

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// ditch anything over the top of the water
			
            // if(IN.worldPos.y >= _WaterHeight){
            //     discard;
            // }
            float3 localPos = mul(_MatrixToSurface, float4(IN.worldPos,1)).xyz;
            if(localPos.y >= 0){
                discard;
            }
              
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
            o.Albedo = texCUBElod (_Cube, float4(IN.worldRefl, _CubeBlur)).rgb ;
            o.Albedo *= c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

            
		}
        ENDCG
        
		Cull Back

        Stencil{
            ref 4
            Comp Always
            Pass Replace
        }
	
	    CGPROGRAM
        
        #pragma vertex vert
        #pragma surface surf Standard fullforwardshadows 
        #pragma multi_compile_fog
        #pragma target 4.0
        #include "UnityCG.cginc"

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // ditch anything over the top of the water
            // if(IN.worldPos.y >= _WaterHeight){
            //     discard;
            // }
            float3 localPos = mul(_MatrixToSurface, float4(IN.worldPos,1)).xyz;
            if(localPos.y >= 0){
                discard;
            }

            //sample main texture
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            //add some light transmitted through the water from the cubemap, two directions for extra texture
            o.Albedo = texCUBElod (_Cube, float4(-IN.viewDir, _CubeBlur)).rgb * _Color * _LightColor0.rgb*_LightIntensity; //great
            o.Albedo += texCUBElod (_Cube, float4(IN.viewDir, _CubeBlur)).rgb * _Color * _LightColor0.rgb*_LightIntensity; //great
            //tint it
            o.Albedo *= c.rgb;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
	    
    }    
}
