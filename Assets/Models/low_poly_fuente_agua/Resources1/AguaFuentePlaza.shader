Shader "TheyAreNotPigeons/AguaFuentePlaza"
{
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct Entrada
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
            };

            struct Salida
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
            };

            Salida vert(Entrada v)
            {
                Salida o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(Salida i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
