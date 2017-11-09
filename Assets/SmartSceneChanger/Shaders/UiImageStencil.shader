// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SSC/UiImageStencil"
{

	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_StencilTex("Stencil Texture", 2D) = "white" {}
		_BackgroundTex("Background Texture", 2D) = "white" {}
		_BackgroundColor("Background Color", Color) = (1,1,1,1)
		_AlphaUvScale("Alpha UV Scale", Range(0.0, 50.0)) = 1
		_LimitAlphaUvScale("Limit Alpha Uv Scale", Range(0.001, 50.0)) = 10
	}

		SubShader
		{
			
			Tags
			{
				"Queue" = "Overlay"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off ZWrite Off ZTest Always
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

				struct v2f {
					float4 pos : SV_POSITION;
					fixed2 uv : TEXCOORD0;
					fixed2 uva : TEXCOORD1;
				};

				v2f vert(appdata_full v)
				{

					v2f o;

					o.pos = UnityObjectToClipPos(v.vertex);

					o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);

					o.uva.xy = ((o.uv.xy - fixed2(0.5, 0.5)) * _AlphaUvScale) + fixed2(0.5, 0.5);

					return o;

				}

				sampler2D _MainTex;
				sampler2D _StencilTex;
				sampler2D _BackgroundTex;

				fixed4 frag(v2f i) : COLOR
				{

					fixed4 ret = tex2D(_MainTex, i.uv.xy);
					fixed4 bg = tex2D(_BackgroundTex, i.uv.xy);
				    fixed alpha = tex2D(_StencilTex, i.uva.xy).a;

					ret.rgb = bg.rgb * _BackgroundColor.rgb;
					ret.a = (1 - alpha) + step(1, (_AlphaUvScale / _LimitAlphaUvScale));

					return ret;

				}
					ENDCG
				}

		}

}
