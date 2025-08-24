ìShader "CPX_Custom/Mobile/Unlit Transparent (Gold)" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("Main Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _Color;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 color_1;
  lowp vec4 tmpvar_2;
  highp vec2 P_3;
  P_3 = ((xlv_TEXCOORD0 * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2 = texture2D (_MainTex, P_3);
  color_1 = tmpvar_2;
  color_1.xyz = (color_1.xyz * _Color.xyz);
  color_1.x = ((0.85 * color_1.y) + 0.15);
  color_1.y = (color_1.y * 0.8);
  gl_FragData[0] = color_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _Color;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 color_1;
  lowp vec4 tmpvar_2;
  highp vec2 P_3;
  P_3 = ((xlv_TEXCOORD0 * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2 = texture (_MainTex, P_3);
  color_1 = tmpvar_2;
  color_1.xyz = (color_1.xyz * _Color.xyz);
  color_1.x = ((0.85 * color_1.y) + 0.15);
  color_1.y = (color_1.y * 0.8);
  _glesFragData[0] = color_1;
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