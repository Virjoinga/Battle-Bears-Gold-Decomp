Shader "Special/Skins/Gold Reflective (fancy)" {
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
  Lighting On
  Material {
   Ambient (0.5,0.5,0.5,0.5)
   Diffuse (0.5,0.5,0.5,0.5)
  }
  SetTexture [_MainTex] { ConstantColor (0.5,0.5,0.5,0.5) combine constant + primary }
  SetTexture [_MainTex] { combine texture * previous }
 }
 Pass {
  Tags { "RenderType"="Opaque" }
  Blend One One
  SetTexture [_ReflectMap] { combine texture, texture alpha }
  SetTexture [_ReflectColorTex] { combine texture * previous }
 }
}
Fallback "Diffuse"
}