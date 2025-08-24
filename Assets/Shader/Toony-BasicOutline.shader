ÇShader "Toon/Basic Outline" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
 _OutlineColor ("Outline Color", Color) = (0,0,0,1)
 _Outline ("Outline width", Range(0.1,2)) = 0.15
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { TexGen CubeNormal }
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 UsePass "Toon/Basic/BASE"
 Pass {
  Name "OUTLINE"
  Tags { "LIGHTMODE"="Always" "RenderType"="Opaque" }
  Cull Front
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 glstate_matrix_projection;
uniform highp float _Outline;
uniform highp vec4 _OutlineColor;
varying highp vec4 xlv_COLOR;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_1.zw = tmpvar_2.zw;
  highp mat3 tmpvar_3;
  tmpvar_3[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_3[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_3[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  highp mat2 tmpvar_4;
  tmpvar_4[0] = glstate_matrix_projection[0].xy;
  tmpvar_4[1] = glstate_matrix_projection[1].xy;
  tmpvar_1.xy = (tmpvar_2.xy + ((
    (clamp ((tmpvar_4 * (tmpvar_3 * 
      normalize(_glesNormal)
    ).xy), vec2(-1.0, -1.0), vec2(1.0, 1.0)) * tmpvar_2.z)
   * _Outline) / tmpvar_2.z));
  gl_Position = tmpvar_1;
  xlv_COLOR = _OutlineColor;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_COLOR;
void main ()
{
  mediump vec4 tmpvar_1;
  tmpvar_1 = xlv_COLOR;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 glstate_matrix_projection;
uniform highp float _Outline;
uniform highp vec4 _OutlineColor;
out highp vec4 xlv_COLOR;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_1.zw = tmpvar_2.zw;
  highp mat3 tmpvar_3;
  tmpvar_3[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_3[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_3[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  highp mat2 tmpvar_4;
  tmpvar_4[0] = glstate_matrix_projection[0].xy;
  tmpvar_4[1] = glstate_matrix_projection[1].xy;
  tmpvar_1.xy = (tmpvar_2.xy + ((
    (clamp ((tmpvar_4 * (tmpvar_3 * 
      normalize(_glesNormal)
    ).xy), vec2(-1.0, -1.0), vec2(1.0, 1.0)) * tmpvar_2.z)
   * _Outline) / tmpvar_2.z));
  gl_Position = tmpvar_1;
  xlv_COLOR = _OutlineColor;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
in highp vec4 xlv_COLOR;
void main ()
{
  mediump vec4 tmpvar_1;
  tmpvar_1 = xlv_COLOR;
  _glesFragData[0] = tmpvar_1;
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
SubShader { 
 Tags { "RenderType"="Opaque" }
 UsePass "Toon/Basic/BASE"
 Pass {
  Name "OUTLINE"
  Tags { "LIGHTMODE"="Always" "RenderType"="Opaque" }
  Cull Front
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
Program "vp" {
}
  SetTexture [_MainTex] { combine primary }
 }
}
Fallback "Toon/Basic"
}