Shader "Special/Skins/Gold Reflective (toony outline)" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
 _ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { TexGen CubeNormal }
 _ReflectColorTex ("Reflect Color", 2D) = "white" {}
 _ReflectMap ("Reflection Map", CUBE) = "dummy.jpg" { TexGen CubeReflect }
 _Color ("Base Color", Color) = (1,1,1,1)
 _OutlineColor ("Outline Color", Color) = (0,0,0,1)
 _Outline ("Outline width", Range(0.1,2)) = 0.15
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 UsePass "Special/Skins/Gold Reflective (toony)/BASE"
 UsePass "Toon/Basic Outline/OUTLINE"
}
Fallback "Special/Skins/Gold Reflective (toony)"
}