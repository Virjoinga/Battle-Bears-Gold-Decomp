ÍShader "CPX_Custom/Mobile/Unlit Color Lightmapped CG" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
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

uniform highp vec4 _Color;
uniform sampler2D unity_Lightmap;
varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  highp vec4 lightMap_1;
  lowp vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD0.zw).xyz);
  lightMap_1 = tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3.xyz = (_Color.xyz * lightMap_1.xyz);
  tmpvar_3.w = _Color.w;
  gl_FragData[0] = tmpvar_3;
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
uniform highp vec4 _Color;
uniform sampler2D unity_Lightmap;
in highp vec4 xlv_TEXCOORD0;
void main ()
{
  highp vec4 lightMap_1;
  lowp vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = (2.0 * texture (unity_Lightmap, xlv_TEXCOORD0.zw).xyz);
  lightMap_1 = tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3.xyz = (_Color.xyz * lightMap_1.xyz);
  tmpvar_3.w = _Color.w;
  _glesFragData[0] = tmpvar_3;
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