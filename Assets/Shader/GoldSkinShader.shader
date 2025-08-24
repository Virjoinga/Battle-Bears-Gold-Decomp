Shader "Special/Skins/Gold Reflective" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
 _ReflectColorTex ("Reflect Color", 2D) = "white" {}
 _ReflectMap ("Reflection Map", CUBE) = "dummy.jpg" { TexGen CubeReflect }
 _Color ("Base Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  SetTexture [_ReflectMap] { combine texture, texture alpha }
  SetTexture [_ReflectColorTex] { combine texture * previous }
  SetTexture [_MainTex] { combine texture + previous }
 }
}
Fallback "Diffuse"
}