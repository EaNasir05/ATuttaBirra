Shader "Instanced/GridTestParticleShader_URP"
{
    Properties
    {
        _Color("Color", Color) = (0.25, 0.5, 0.5, 1)
        _DensityRange("Density Range", Range(0,500000)) = 1.0
        _size("Size", Float) = 1.0
    }

    SubShader
    {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Tags{ "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Instancing support
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            // URP includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _Color;
            float _size;

            struct Particle
            {
                float pressure;
                float density;
                float3 currentForce;
                float3 velocity;
                float3 position;
            };

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                StructuredBuffer<Particle> _particlesBuffer;
            #endif

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float3x3 BuildRotationIdentity() {
                // no rotation in original shader
                return float3x3(1,0,0,
                                0,1,0,
                                0,0,1);
            }

            void setup()
            {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                float3 pos = _particlesBuffer[unity_InstanceID].position;
                float size = _size;

                float4x4 m = float4x4(
                    float4(size,0,0,0),
                    float4(0,size,0,0),
                    float4(0,0,size,0),
                    float4(pos,1)
                );

                unity_ObjectToWorld = m;

                // Inversa calcolata a mano
                float inv = 1.0 / size;

                float4x4 invM = float4x4(
                    float4(inv,0,0,0),
                    float4(0,inv,0,0),
                    float4(0,0,inv,0),
                    float4(-pos.x * inv, -pos.y * inv, -pos.z * inv, 1)
                );

                unity_WorldToObject = invM;
            #endif
            }

            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normal = normalize(IN.normalWS);

                // simple lambert
                float3 lightDir = _MainLightPosition.xyz;
                float NdotL = saturate(dot(normal, -lightDir));

                float3 col = _Color.rgb * (0.2 + NdotL);

                return float4(col, 1);
            }

            ENDHLSL
        }
    }
}


