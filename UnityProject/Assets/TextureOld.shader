Shader "Lab09/texture - old" {
	Properties {
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Diffuse texture", 2D) = "white"{}
		_Attenuation("fall off", Range(0,5)) = 0
	}
	SubShader {
			Tags{"LightMode" = "ForwardBase" }
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			//user -color
			uniform float4 _Color;
			
			//user -texture
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			//user -lambert
			uniform float _Attenuation;

			//unity variables
			uniform float3 _LightColor0;

			struct input{
				float4 vertexPos : POSITION;
				float4 textureCoord : TEXCOORD0;
				float3 vertexNormal : NORMAL;	
			};

			struct v2f{
				float4 pixelPos : SV_POSITION;
				float4 tex : TEXCOORD0;
				float4 color : COLOR;
			};

			v2f vert(input i){
				v2f toReturn;

				//texture
				//get pixel position
				toReturn.pixelPos = mul(UNITY_MATRIX_MVP, i.vertexPos);
				//get texture coords
				toReturn.tex = i.textureCoord;	
				
				//lighting
				float3 lightDirection;				

				//get ambient light color
				float3 ambientLight = UNITY_LIGHTMODEL_AMBIENT.rgb;

				//get ight direction
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				
				//get normal
				float3 tempNorm = i.vertexNormal;

				//convert to objSpace
				float4 objNorm = mul(float4(tempNorm, 1.0), _World2Object);

				//normalize
				float3 normalizedNormal = normalize(objNorm).xyz;

				//dot product of light color normals and light direction aswell as falloff
				float3 diffuseReflection = _Attenuation * _LightColor0.xyz * max(0.0, dot(normalizedNormal, lightDirection));

				//ambient light added
				float3 final = diffuseReflection + ambientLight;

				//float4(xyz, w) float4(x,y,z,w)
				toReturn.color = float4(final,1.0);
				//toReturn.color = objNorm;
								
				return toReturn;
			}

			float4 frag(v2f i) : COLOR {
				float4 tex = tex2D(_MainTex, _MainTex_ST.xy * i.tex.xy + _MainTex_ST.zw);

				return i.color * _Color * tex;
		
			}

			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
