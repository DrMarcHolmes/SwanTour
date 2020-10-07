Shader "Unlit/Monosphere"
{
	Properties
	{
		_MainTex("Texture", 2D) = "White" {}
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Cull front
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		//#pragma fragmentoption ARB_precision_hint_fastest
		//#pragma glsl

		// make fog work
		//#pragma multi_compile_fog

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float3    normal : TEXCOORD0;
		float4    pos : SV_POSITION;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;

	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.normal = v.normal;
		return o;
	}



	//#define PI 3.141592653589793
#define PI 3.141592653589793

	inline float2 RadialCoords(float3 a_coords)
	{
		float3 a_coords_n = normalize(a_coords);
		float lon = atan2(a_coords_n.z, a_coords_n.x);
		float lat = acos(a_coords_n.y);
		float2 sphereCoords = float2(lon, lat) * (1.0 / PI);
		return float2((1.0 - sphereCoords.x) * 0.5 , (1.0 - sphereCoords.y));
	}

	/*fixed4 frag(v2f IN) : SV_Target
	{


	float2 equiUV = RadialCoords(IN.normal);
	fixed4 col = tex2D(_MainTex, equiUV);
	return col;
	}*/
	fixed4 frag(v2f IN) : SV_Target
	{
		float2 equiUV = RadialCoords(IN.normal);
		return tex2Dlod(_MainTex, float4(equiUV.x,equiUV.y,0,0));

	}
		ENDCG

	}
	}
		FallBack "VertexLit"
}
