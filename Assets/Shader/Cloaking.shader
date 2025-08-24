Shader "FX/Cloaking" {
Properties {
 _Color ("Color", Color) = (1,1,1,1)
 _ReflectTex ("Reflection Texture", CUBE) = "dummy.jpg" { TexGen CubeReflect }
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Color [_Color]
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  Offset -1, -1
  SetTexture [_ReflectTex] { combine texture * primary }
 }
}
}