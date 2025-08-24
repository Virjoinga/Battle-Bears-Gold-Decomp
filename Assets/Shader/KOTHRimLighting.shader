œ Shader "Rim Light" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _RimColor ("Rim Color", Color) = (1,1,1,1)
 _Alpha ("Alpha", Range(0,1)) = 0.5
 _SpeedX ("Speed X", Float) = 0
 _SpeedY ("Speed Y", Float) = 0
 _FlashPeriod ("Flash Period", Range(1,10)) = 1
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _RimColor;
uniform highp float _SpeedX;
uniform highp float _SpeedY;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_COLOR;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = _WorldSpaceCameraPos;
  highp float tmpvar_2;
  tmpvar_2 = clamp (((
    (1.0 - dot (normalize(_glesNormal), normalize((
      ((_World2Object * tmpvar_1).xyz * unity_Scale.w)
     - _glesVertex.xyz))))
   - 0.3) / 0.7), 0.0, 1.0);
  highp vec4 tmpvar_3;
  tmpvar_3.x = (_glesMultiTexCoord0.x + (_Time.y * _SpeedX));
  tmpvar_3.y = (_glesMultiTexCoord0.y + (_Time.y * _SpeedY));
  tmpvar_3.zw = _glesMultiTexCoord0.zw;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((tmpvar_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR = (vec3((tmpvar_2 * (tmpvar_2 * 
    (3.0 - (2.0 * tmpvar_2))
  ))) * _RimColor.xyz);
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform sampler2D _MainTex;
uniform highp vec4 _Color;
uniform highp float _Alpha;
uniform highp float _FlashPeriod;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_COLOR;
void main ()
{
  highp vec4 texcol_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  texcol_1 = tmpvar_2;
  texcol_1.xyz = ((texcol_1 * (
    (_Color + cos((_Time.y * _FlashPeriod)))
   + 1.0)).xyz + xlv_COLOR);
  texcol_1.w = _Alpha;
  gl_FragData[0] = texcol_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _RimColor;
uniform highp float _SpeedX;
uniform highp float _SpeedY;
out highp vec2 xlv_TEXCOORD0;
out highp vec3 xlv_COLOR;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = _WorldSpaceCameraPos;
  highp float tmpvar_2;
  tmpvar_2 = clamp (((
    (1.0 - dot (normalize(_glesNormal), normalize((
      ((_World2Object * tmpvar_1).xyz * unity_Scale.w)
     - _glesVertex.xyz))))
   - 0.3) / 0.7), 0.0, 1.0);
  highp vec4 tmpvar_3;
  tmpvar_3.x = (_glesMultiTexCoord0.x + (_Time.y * _SpeedX));
  tmpvar_3.y = (_glesMultiTexCoord0.y + (_Time.y * _SpeedY));
  tmpvar_3.zw = _glesMultiTexCoord0.zw;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((tmpvar_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_COLOR = (vec3((tmpvar_2 * (tmpvar_2 * 
    (3.0 - (2.0 * tmpvar_2))
  ))) * _RimColor.xyz);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform sampler2D _MainTex;
uniform highp vec4 _Color;
uniform highp float _Alpha;
uniform highp float _FlashPeriod;
in highp vec2 xlv_TEXCOORD0;
in highp vec3 xlv_COLOR;
void main ()
{
  highp vec4 texcol_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_MainTex, xlv_TEXCOORD0);
  texcol_1 = tmpvar_2;
  texcol_1.xyz = ((texcol_1 * (
    (_Color + cos((_Time.y * _FlashPeriod)))
   + 1.0)).xyz + xlv_COLOR);
  texcol_1.w = _Alpha;
  _glesFragData[0] = texcol_1;
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