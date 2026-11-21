Shader "TheyAreNotPigeons/AguaFuentePaloma"
{
    Properties
    {
        [HideInInspector] _TiempoAgua ("Tiempo del efecto", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _TiempoAgua;

            struct Entrada
            {
                float4 vertex : POSITION;
                float2 tipo : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct Salida
            {
                float4 position : SV_POSITION;
                float2 plano : TEXCOORD0;
                float superficie : TEXCOORD1;
                fixed4 color : COLOR;
            };

            Salida vert(Entrada v)
            {
                Salida o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.plano = v.vertex.xz;
                o.superficie = v.tipo.x;
                o.color = v.color;
                return o;
            }

            float2 PuntoCelda(float2 celda)
            {
                float2 semilla = float2(dot(celda, float2(127.1, 311.7)),
                                       dot(celda, float2(269.5, 183.3)));
                return frac(sin(semilla) * 43758.5453);
            }

            float ReflejosDeAgua(float2 plano)
            {
                float t = _TiempoAgua * 0.7;
                float2 p = plano * 8.0
                    + float2(sin(plano.y * 4.0 + t), sin(plano.x * 5.0 - t)) * 0.25;
                float2 celda = floor(p);
                float2 posCelda = frac(p);
                float menor = 10.0;
                float segundo = 10.0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 vecino = float2(x, y);
                        float2 punto = PuntoCelda(celda + vecino);
                        punto = 0.5 + 0.34 * sin(t + 6.2831853 * punto);
                        float2 d = vecino + punto - posCelda;
                        float distancia = dot(d, d);
                        segundo = min(segundo, max(menor, distancia));
                        menor = min(menor, distancia);
                    }
                }
                return 1.0 - smoothstep(0.028, 0.12, segundo - menor);
            }

            fixed4 frag(Salida i) : SV_Target
            {
                if (i.superficie < 0.5) return i.color;
                float reflejo = ReflejosDeAgua(i.plano);
                fixed3 espuma = fixed3(0.63, 0.96, 0.98);
                return fixed4(lerp(i.color.rgb * 0.82, espuma, reflejo * 0.42), i.color.a);
            }
            ENDCG
        }
    }
}
