Shader "TNTC/TexturePainter"
{   
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            sampler2D _MainTex;

            sampler2D _Logo1;
            sampler2D _Logo2;
            sampler2D _Logo3;
            
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;

            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };
            

            v2f vert (appdata v)
            {
                v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
				float4 uv = float4(0, 0, 0, 1);
                uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * float2( 2, 2) - float2(1, 1));
				o.vertex = uv; 
                return o;
            }

            float4 SetColor(float x)
            {
                if(_Color2.a != 1 && _Color3.a != 1) return _Color1;
                if(_Color1.a == 1 && _Color2.a == 1&& _Color3.a == 1)
                {
                    if(x < 0.33333)return _Color1;
                    if(x < 0.66666) return _Color2;
                    return _Color3;
                }
                if(_Color1.a != 1)
                {
                    if(x < 0.5)return _Color2;
                    return _Color3;
                }
                if(_Color2.a != 1)
                {
                    if(x < 0.5)return _Color1;
                    return _Color3;
                }
                if(x < 0.5) return _Color1;
                return _Color2;
            }

            float4 SetLogo(float2 uv)
            {
                if(tex2D(_Logo1,uv).a == 1) return tex2D(_Logo1,uv);
                if(tex2D(_Logo2,uv).a == 1) return tex2D(_Logo2,uv);
                if(tex2D(_Logo3,uv).a == 1) return tex2D(_Logo3,uv);
                return tex2D(_MainTex,uv);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = SetColor(i.uv.x);
                col.a = 0;
                return lerp(col,SetLogo(i.uv),SetLogo(i.uv).a);
            }
       
            
            ENDCG
        }
    }
}