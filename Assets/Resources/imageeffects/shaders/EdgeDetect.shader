ËShader "BBR/Edge Detect" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Treshold ("Treshold", Float) = 0.2
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_texture0;
uniform highp vec4 _MainTex_TexelSize;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD0_1;
varying highp vec2 xlv_TEXCOORD0_2;
void main ()
{
  mediump vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  highp vec2 inUV_2;
  inUV_2 = tmpvar_1;
  highp vec4 tmpvar_3;
  tmpvar_3.zw = vec2(0.0, 0.0);
  tmpvar_3.xy = inUV_2;
  highp vec4 tmpvar_4;
  tmpvar_4 = (glstate_matrix_texture0 * tmpvar_3);
  highp vec2 tmpvar_5;
  tmpvar_5.x = -(_MainTex_TexelSize.x);
  highp float cse_6;
  cse_6 = -(_MainTex_TexelSize.y);
  tmpvar_5.y = cse_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _MainTex_TexelSize.x;
  tmpvar_7.y = cse_6;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_4.xy;
  xlv_TEXCOORD0_1 = (tmpvar_4.xy + tmpvar_5);
  xlv_TEXCOORD0_2 = (tmpvar_4.xy + tmpvar_7);
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform highp float _Treshold;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD0_1;
varying highp vec2 xlv_TEXCOORD0_2;
void main ()
{
  mediump vec3 p3_1;
  mediump vec3 p2_2;
  mediump vec4 original_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  original_3 = tmpvar_4;
  lowp vec3 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD0_1).xyz;
  p2_2 = tmpvar_5;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, xlv_TEXCOORD0_2).xyz;
  p3_1 = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = (((original_3.xyz * 2.0) - p2_2) - p3_1);
  mediump float tmpvar_8;
  tmpvar_8 = dot (tmpvar_7, tmpvar_7);
  if ((tmpvar_8 >= _Treshold)) {
    original_3.xyz = vec3(0.0, 0.0, 0.0);
  };
  gl_FragData[0] = original_3;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_texture0;
uniform highp vec4 _MainTex_TexelSize;
out highp vec2 xlv_TEXCOORD0;
out highp vec2 xlv_TEXCOORD0_1;
out highp vec2 xlv_TEXCOORD0_2;
void main ()
{
  mediump vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  highp vec2 inUV_2;
  inUV_2 = tmpvar_1;
  highp vec4 tmpvar_3;
  tmpvar_3.zw = vec2(0.0, 0.0);
  tmpvar_3.xy = inUV_2;
  highp vec4 tmpvar_4;
  tmpvar_4 = (glstate_matrix_texture0 * tmpvar_3);
  highp vec2 tmpvar_5;
  tmpvar_5.x = -(_MainTex_TexelSize.x);
  highp float cse_6;
  cse_6 = -(_MainTex_TexelSize.y);
  tmpvar_5.y = cse_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _MainTex_TexelSize.x;
  tmpvar_7.y = cse_6;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_4.xy;
  xlv_TEXCOORD0_1 = (tmpvar_4.xy + tmpvar_5);
  xlv_TEXCOORD0_2 = (tmpvar_4.xy + tmpvar_7);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform highp float _Treshold;
in highp vec2 xlv_TEXCOORD0;
in highp vec2 xlv_TEXCOORD0_1;
in highp vec2 xlv_TEXCOORD0_2;
void main ()
{
  mediump vec3 p3_1;
  mediump vec3 p2_2;
  mediump vec4 original_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture (_MainTex, xlv_TEXCOORD0);
  original_3 = tmpvar_4;
  lowp vec3 tmpvar_5;
  tmpvar_5 = texture (_MainTex, xlv_TEXCOORD0_1).xyz;
  p2_2 = tmpvar_5;
  lowp vec3 tmpvar_6;
  tmpvar_6 = texture (_MainTex, xlv_TEXCOORD0_2).xyz;
  p3_1 = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = (((original_3.xyz * 2.0) - p2_2) - p3_1);
  mediump float tmpvar_8;
  tmpvar_8 = dot (tmpvar_7, tmpvar_7);
  if ((tmpvar_8 >= _Treshold)) {
    original_3.xyz = vec3(0.0, 0.0, 0.0);
  };
  _glesFragData[0] = original_3;
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
Fallback Off
}