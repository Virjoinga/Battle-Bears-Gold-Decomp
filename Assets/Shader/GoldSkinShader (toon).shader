òShader "Special/Skins/Gold Reflective (toony)" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
 _ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { TexGen CubeNormal }
 _ReflectColorTex ("Reflect Color", 2D) = "white" {}
 _ReflectMap ("Reflection Map", CUBE) = "dummy.jpg" { TexGen CubeReflect }
 _Color ("Base Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "BASE"
  Tags { "RenderType"="Opaque" }
  Cull Off
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp vec4 _MainTex_ST;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.w = 0.0;
  tmpvar_1.xyz = normalize(_glesNormal);
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = (glstate_matrix_modelview0 * tmpvar_1).xyz;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp samplerCube _ToonShade;
uniform sampler2D _ReflectColorTex;
uniform lowp samplerCube _ReflectMap;
uniform highp vec4 _Color;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 reflectMask_1;
  highp vec4 reflection_2;
  highp vec4 cube_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp vec4 tmpvar_5;
  tmpvar_5 = (_Color * tmpvar_4);
  lowp vec4 tmpvar_6;
  tmpvar_6 = textureCube (_ToonShade, xlv_TEXCOORD1);
  cube_3 = tmpvar_6;
  lowp vec4 tmpvar_7;
  tmpvar_7 = textureCube (_ReflectMap, xlv_TEXCOORD1);
  reflection_2 = tmpvar_7;
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_ReflectColorTex, xlv_TEXCOORD0);
  reflectMask_1 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.xyz = (((2.0 * cube_3.xyz) * tmpvar_5.xyz) + (reflection_2.xyz * reflectMask_1.xyz));
  tmpvar_9.w = tmpvar_5.w;
  gl_FragData[0] = tmpvar_9;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp vec4 _MainTex_ST;
out highp vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.w = 0.0;
  tmpvar_1.xyz = normalize(_glesNormal);
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = (glstate_matrix_modelview0 * tmpvar_1).xyz;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp samplerCube _ToonShade;
uniform sampler2D _ReflectColorTex;
uniform lowp samplerCube _ReflectMap;
uniform highp vec4 _Color;
in highp vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 reflectMask_1;
  highp vec4 reflection_2;
  highp vec4 cube_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture (_MainTex, xlv_TEXCOORD0);
  highp vec4 tmpvar_5;
  tmpvar_5 = (_Color * tmpvar_4);
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture (_ToonShade, xlv_TEXCOORD1);
  cube_3 = tmpvar_6;
  lowp vec4 tmpvar_7;
  tmpvar_7 = texture (_ReflectMap, xlv_TEXCOORD1);
  reflection_2 = tmpvar_7;
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_ReflectColorTex, xlv_TEXCOORD0);
  reflectMask_1 = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.xyz = (((2.0 * cube_3.xyz) * tmpvar_5.xyz) + (reflection_2.xyz * reflectMask_1.xyz));
  tmpvar_9.w = tmpvar_5.w;
  _glesFragData[0] = tmpvar_9;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
SubProgram "gles3 " {
"!!GLES3"
}
}
 }
}
Fallback "Special/Skins/Gold Reflective"
}