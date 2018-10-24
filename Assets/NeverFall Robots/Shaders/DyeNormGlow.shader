Shader "RoboCraft/DyeNormGlow"
{
	Properties
	{
		_Color ("Diffuse Color", Color) = (0.5,0.5,0.5,1)
	    _DyeColor ("Dye Color", Color) = (0.15,0.15,0.15,1)
	    _EmissionColor ("Emission Color", Color) = (0,0.24,1,1)
	    _SpecColor ("Specular Color", Color) = (1,1,1,1)
	    _ReflectColor ("Reflection Color", Color) = (0.45,0.45,0.45,1)
	    
	    _Glossiness ("Glossiness", Float) = 0.96
	    
	    _MainTex ("Diffuse Map", 2D) = "white" {}
	    _BumpMap ("Normal Map", 2D) = "bump" {}
	    _UtilityMap ("Utility Map", 2D) = "black" {}
	    
	    _Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
	    
	    _TimeScale ("Time for Animation to complete", Float) = 1
	}
	
	
	SubShader
	{
	    Tags { "RenderType"="Opaque" }
	    LOD 400
	
	CGPROGRAM
	
	#pragma surface surf BlinnPhong
	
	#pragma target 3.0
	
	//input limit (8) exceeded, shader uses 9
	
	#pragma exclude_renderers d3d11_9x
	
	 
	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _UtilityMap;
	
	
	samplerCUBE _Cube;
	
	fixed4 _Color;
	float3 _DyeColor;
	float3 _EmissionColor;
	fixed4 _ReflectColor;
	
	float _Glossiness;
	
	float _TimeScale;
	 
	
	struct Input
	{
	    float2 uv_MainTex;
	    float3 worldRefl;
	
	    INTERNAL_DATA
	};
	
	 
	
	void surf (Input IN, inout SurfaceOutput o)
	{
	
	    fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	    fixed4 utilityMap = tex2D(_UtilityMap, IN.uv_MainTex);
	
	
	    o.Albedo = utilityMap.a * _DyeColor.rgb + (tex.rgb - utilityMap.a) * _Color.rgb;
	
	
		//fixed3 anim = pow(fmod((utilityMap.a + (1 - fmod(_Time.y/_TimeScale,1))),1),2);
	    fixed3 anim = pow(fmod((utilityMap.a + (1 - fmod(_Time.y/_TimeScale,1))),1),2)    +    pow(fmod((utilityMap.a + (fmod(_Time.y/_TimeScale,1))),1),2)/4;
	
	    o.Gloss = _Glossiness*2;//unity_DeltaTime.xx/0.2 * _Glossiness;
	
	    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	
	    
	
	    float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	
	    
	    fixed4 reflcol = texCUBE (_Cube, worldRefl);
	
	    reflcol *= utilityMap.r;// * _ReflectColor.a;//
	    //reflcol *= tex.a;// * _ReflectColor.a;
	
	
	    o.Emission = utilityMap.g * _EmissionColor.rgb * anim * 4 + (tex.rgb - utilityMap.g) * reflcol.a;// _ReflectColor.rgb;//
	    //o.Emission = reflcol.a * _ReflectColor.rgb * 8;
	
	
		o.Specular = _ReflectColor.rgb;// * 2;
		//o.Alpha = reflcol.a * _ReflectColor.rgb;
	}
	
	ENDCG
	
	}
	 
	FallBack "Reflective/Bumped Diffuse"
}