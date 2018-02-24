// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

    Shader "Custom/Transparent Specular"
    {
        Properties
        {
            _Color("Main Color", Color) = (0.3529412,0.3529412,0.3529412,1)
            _GlossNoYes("_GlossNoYes", Range(0,1) ) = 0
            _Spraying("Base (RGB) Gloss (A)", 2D) = "white" {}
            _Metalization("_Metalization", 2D) = "black" {}
            _Painting("_Painting", 2D) = "black" {}
            _UVOffset("_UVOffset", Range(0,1) ) = 0.5
            _Reflection("_Reflection", Cube) = "black" {}
            _BumpMap("Normalmap", 2D) = "bump" {}
            _Distorsion("_Distorsion", Range(0,0.1) ) = 0
            _Refraction("_Refraction", Range(0,1) ) = 0
            _Shininess("Shininess", Range(0.01,1) ) = 1
            _FresnelPower("_Fresnel Power", Range(0.1,3) ) = 0.45
       
        }
     
       
        SubShader
        {
           
            Tags
            {
                "Queue"="Transparent"
                "IgnoreProjector"="False"
                "RenderType"="Transparent"
            }
     
           
           
            //--------------------------1---------------------- Original-Hintergrund als Textur sichern
            GrabPass { "_GrabTexture1" }
     
     
     
            //--------------------------2---------------------- Bereich des Objektes schwarz färben
            //------------------------------------------------- Grundlage für den nächsten Pass, der additiv arbeitet - sonst würde alles überstrahlt werden
            Blend One Zero
            ZWrite Off
            Lighting Off
           
            CGPROGRAM
            #pragma surface surf Lambert
           
            struct Input
            {
                float4 color : COLOR;
            };
           
            void surf( Input IN, inout SurfaceOutput o )
            {
                o.Albedo = 0;
            }
           
            ENDCG
     
     
     
            //--------------------------3---------------------- Transparenz wird mit GrabTexture #1 simuliert
            //------------------------------------------------- Refraction wird errechnet aus der Oberflächen-Normale (Richtung der Brechung)
            ZWrite Off
            Lighting On
           
            CGPROGRAM
            #pragma surface surf Lambert
           
            struct Input
            {
                float4 screenPos;
                float3 viewDir;
                float3 worldNormal;
            };
           
            sampler2D _GrabTexture1;
            float _Refraction;
           
            void surf( Input IN, inout SurfaceOutput o )
            {
                float3 obj = mul( (float3x3)unity_ObjectToWorld, float3( 0,0,0 ) );
                float3 view = float3( IN.viewDir.x, 0, IN.viewDir.z );
                float3 middle = obj + normalize( IN.viewDir );
                float3 offsetVector = obj + normalize( IN.worldNormal );
                float deltaX = max( offsetVector.x - middle.x, offsetVector.z - middle.z );
                float deltaY = ( offsetVector.y - middle.y );
               
                float2 screenPosition = ( IN.screenPos.xy / IN.screenPos.w ).xy - float2( deltaX, deltaY ) * pow( _Refraction, 3 );// * ( 1 - glassDepth );
                float3 bgColor = tex2D( _GrabTexture1, screenPosition ).rgb;
               
                o.Albedo = 0;
                o.Emission = bgColor;
            }
           
            ENDCG
           
       
           
            //--------------------------4---------------------- Blende Farbe (Spraying), Transparenz (Metalization) und Painting drüber
            //------------------------------------------------- Hier wird in den Z-Buffer geschrieben und sicher gestellt, das undurchsichtige Bereiche auch undurchsichtig und im Vordergrund angezeigt werden
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Lighting On
           
            CGPROGRAM
            #pragma surface surf BlinnPhong
            #pragma target 3.0
           
            struct Input
            {
                float2 uv_Spraying;
                float3 viewDir;
                float2 uv_BumpMap;
                float2 uv_Metalization;
                float3 worldNormal;
                INTERNAL_DATA
            };
           
            float4 _Color;
            sampler2D _Spraying;
            samplerCUBE _Reflection;
            sampler2D _BumpMap;
            float _FresnelPower;
            float _Shininess;
            float _GlossNoYes;
            sampler2D _Metalization;
           
            void surf( Input IN, inout SurfaceOutput o )
            {
                float3 sprayColor = tex2D( _Spraying, IN.uv_Spraying );
                float3 metal = tex2D( _Metalization, IN.uv_Metalization ).rgb;
               
                float3 n = UnpackNormal( tex2D( _BumpMap, IN.uv_BumpMap ) );
               
                o.Normal = float3( 0, 0, 1 ); // wird hart gesetzt für Fresnel-Effekt
               
                float fresnel = ( 1.0 - dot( normalize( IN.viewDir ), float3( 0, 0, 1 ) ) );
                float fresnelPower = max( 0.2, pow( fresnel, _FresnelPower ) );
                float3 reflectionVector = -reflect( IN.viewDir, IN.worldNormal );
                float3 reflectionColor = texCUBE( _Reflection, reflectionVector ).rgb;
     
                o.Emission = lerp( sprayColor, reflectionColor, fresnelPower );
                o.Alpha = max( fresnelPower, max( metal.r, _Color.a ) );
               
    //          o.Specular = _Shininess;
    //          o.Gloss = sprayColor * _GlossNoYes.xxxx;
                o.Specular = 1;
                o.Gloss = 1;
                    }
           
            ENDCG
        }
       
       
    //  Fallback "Transparent"
       
       
    }
