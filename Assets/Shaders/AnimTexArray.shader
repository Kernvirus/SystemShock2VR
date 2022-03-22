Shader "SS2/AnimTexArray"
{
    Properties
    {
        _MainTexArr ("Tex", 2DArray) = "" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_FPS("FPS", Float) = 4
		_FrameCount("Frame Count", Int) = 1
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
		#pragma require 2darray

        struct Input
        {
            float2 uv_MainTexArr;
        };

        half _Glossiness;
        half _Metallic;
		half _FPS;
		int _FrameCount;
		UNITY_DECLARE_TEX2DARRAY(_MainTexArr);

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			int texIndex = fmod(floor(_Time.y * _FPS), _FrameCount);
			fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTexArr, float3(IN.uv_MainTexArr, texIndex));
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
