Shader "Custom/InvisibleMask"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1) 
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader{
        // draw after all opaque objects (queue = 3001):
        Tags { "Queue" = "Transparent+1" "RenderPipeline" = "HDRenderPipeline" "RenderType" = "HDUnlitShader"  }
        
      
        // Pass
        //{
        //ZWrite Off

        //    CGPROGRAM
        //    #pragma vertex vert
        //    #pragma fragment frag
        //    // make fog work
        //    #pragma multi_compile_fog

        //    #include "UnityCG.cginc"

        //    struct appdata
        //    {
        //        float4 vertex : POSITION;
        //        float2 uv : TEXCOORD0;
        //    };

        //    struct v2f
        //    {
        //        float2 uv : TEXCOORD0;
        //        UNITY_FOG_COORDS(1)
        //        float4 vertex : SV_POSITION;
        //    };

        //    sampler2D _MainTex;
        //    float4 _MainTex_ST;
        //    float4 _Color;

        //    v2f vert(appdata v)
        //    {
        //        v2f o;
        //        o.vertex = UnityObjectToClipPos(v.vertex);
        //        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        //        UNITY_TRANSFER_FOG(o,o.vertex);
        //        return o;
        //    }

        //    fixed4 frag(v2f i) : SV_Target
        //    {
        //        // sample the texture
        //        fixed4 col = tex2D(_MainTex, i.uv);
        //    // apply fog
        //    UNITY_APPLY_FOG(i.fogCoord, col);            
        //    return _Color;
        //    }
        //    ENDCG
        //}

        Pass {

          Blend OneMinusSrcColor One // keep the image behind it

          HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            struct AttributesDefault
            {
                float3 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VaryingsDefault
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float3 _Range;
            float3 _Offset;
            float4 _Color;

            VaryingsDefault vert(AttributesDefault att)
            {
                VaryingsDefault output;
                UNITY_SETUP_INSTANCE_ID(att);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                //float3 positionRWS = TransformObjectToWorld(att.positionOS.xyz * _Range + _Offset);
                output.positionCS = TransformWorldToHClip(TransformObjectToWorld(att.positionOS.xyz));

                return output;
            }

            void frag(VaryingsDefault varying, out float outLightCount : SV_Target0, out float4 outColorAccumulation : SV_Target1)
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varying);
                outLightCount = 0.5f;
                outColorAccumulation = _Color;
            }

            ENDHLSL
        }

    }
}
