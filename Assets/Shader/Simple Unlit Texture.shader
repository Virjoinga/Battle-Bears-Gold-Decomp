Shader "CPX_Custom/Mobile/Simple Unlit Texture" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
 _Color ("Base Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Color [_Color]
  SetTexture [_MainTex] { combine texture * primary double }
 }
}
Fallback "Diffuse"
}