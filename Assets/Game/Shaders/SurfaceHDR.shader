Shader "Game/SurfaceHDR"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ORM ("ORM", 2D) = "bump" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_Emission("Emission", 2D) = "black" {}
		[HDR] _EmissionColor("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _ORM;
		sampler2D _BumpMap;
		sampler2D _Emission;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_ORM;
			float2 uv_BumpMap;
			float2 uv_Emission;
        };

        fixed4 _Color;
        fixed4 _EmissionColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

			o.Occlusion = (tex2D (_ORM, IN.uv_ORM)).r;
			o.Smoothness = 1 - (tex2D (_ORM, IN.uv_ORM)).g;
            o.Metallic = (tex2D (_ORM, IN.uv_ORM)).b;
            
			o.Emission = (tex2D (_Emission, IN.uv_Emission)) * _EmissionColor;

			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
