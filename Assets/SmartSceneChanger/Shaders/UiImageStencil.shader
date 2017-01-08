Shader "SSC/UiImageStencil"
{

	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_StencilTex("Stencil Texture", 2D) = "white" {}
		_BackgroundColor("Background Color", Color) = (0.0, 0.0, 0.0, 1.0)
		_AlphaUvScale("Alpha UV Scale", Float) = 1
		_LimitAlphaUvScale("Limit Alpha Uv Scale", Float) = 10
	}

		SubShader
		{
			
			Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}

			Blend SrcAlpha OneMinusSrcAlpha

			Pass
		{

				CGPROGRAM

				#pragma vertex vert  
				#pragma fragment frag
				#include "UnityCG.cginc"

				half  _AlphaUvScale;
				half  _LimitAlphaUvScale;
				half4 _MainTex_ST;
				half4 _StencilTex_ST;
				fixed4 _BackgroundColor;
				uniform sampler2D _MainTex;
				uniform sampler2D _StencilTex;

				struct v2f {
					float4 pos : SV_POSITION;
					fixed2 uv : TEXCOORD0;
					fixed2 uva : TEXCOORD1;
				};

				v2f vert(appdata_full v)
				{

					v2f o;

					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

					o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);

					o.uva.xy = ((o.uv.xy - fixed2(0.5, 0.5)) * _AlphaUvScale) + fixed2(0.5, 0.5);

					return o;

				}

				fixed4 frag(v2f i) : COLOR
				{

					fixed4 ret = tex2D(_MainTex, i.uv.xy);
				    fixed alpha = tex2D(_StencilTex, i.uva.xy).a;

					ret.rgb = _BackgroundColor.rgb;
					ret.a = (1 - alpha) + step(1, (_AlphaUvScale / _LimitAlphaUvScale));

					return ret;

				}
					ENDCG
				}

		}

}
