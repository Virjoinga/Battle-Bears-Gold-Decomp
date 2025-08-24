ƒShader "CPX_Custom/Mobile/Unlit Lightmapped CG" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 unity_LightmapST;
varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xy = _glesMultiTexCoord0.xy;
  tmpvar_1.zw = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D unity_Lightmap;
uniform highp vec4 _MainTex_ST;
varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  highp vec4 lightMap_1;
  highp vec4 color_2;
  lowp vec4 tmpvar_3;
  highp vec2 P_4;
  P_4 = ((xlv_TEXCOORD0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3 = texture2D (_MainTex, P_4);
  color_2 = tmpvar_3;
  lowp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD0.zw).xyz);
  lightMap_1 = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.xyz = (color_2.xyz * lightMap_1.xyz);
  tmpvar_6.w = color_2.w;
  gl_FragData[0] = tmpvar_6;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 unity_LightmapST;
out highp vec4 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xy = _glesMultiTexCoord0.xy;
  tmpvar_1.zw = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D unity_Lightmap;
uniform highp vec4 _MainTex_ST;
in highp vec4 xlv_TEXCOORD0;
void main ()
{
  highp vec4 lightMap_1;
  highp vec4 color_2;
  lowp vec4 tmpvar_3;
  highp vec2 P_4;
  P_4 = ((xlv_TEXCOORD0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_3 = texture (_MainTex, P_4);
  color_2 = tmpvar_3;
  lowp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (2.0 * texture (unity_Lightmap, xlv_TEXCOORD0.zw).xyz);
  lightMap_1 = tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.xyz = (color_2.xyz * lightMap_1.xyz);
  tmpvar_6.w = color_2.w;
  _glesFragData[0] = tmpvar_6;
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
}