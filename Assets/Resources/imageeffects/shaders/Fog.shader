֏Shader "BBR/ImageEffects/Fog" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "black" {}
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
uniform highp mat4 _FrustumCornersWS;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_3;
  tmpvar_3.xyw = tmpvar_1.xyw;
  mediump float index_4;
  highp vec2 tmpvar_5;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  highp float tmpvar_8;
  tmpvar_8 = tmpvar_1.z;
  index_4 = tmpvar_8;
  tmpvar_3.z = 0.1;
  tmpvar_5 = tmpvar_2;
  tmpvar_6 = tmpvar_2;
  int i_9;
  i_9 = int(index_4);
  highp vec4 tmpvar_10;
  mediump vec4 v_11;
  v_11.x = _FrustumCornersWS[0][i_9];
  v_11.y = _FrustumCornersWS[1][i_9];
  v_11.z = _FrustumCornersWS[2][i_9];
  v_11.w = _FrustumCornersWS[3][i_9];
  tmpvar_10 = v_11;
  tmpvar_7.xyz = tmpvar_10.xyz;
  tmpvar_7.w = index_4;
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = tmpvar_6;
  xlv_TEXCOORD2 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp float _GlobalDensity;
uniform highp vec4 _FogColor;
uniform highp vec4 _StartDistance;
uniform highp vec4 _Y;
uniform highp vec4 _CameraWS;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  mediump vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_3;
  z_3 = tmpvar_2.x;
  highp vec4 tmpvar_4;
  tmpvar_4 = ((1.0/((
    (_ZBufferParams.x * z_3)
   + _ZBufferParams.y))) * xlv_TEXCOORD2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_6;
  tmpvar_6 = max (0.0, ((
    (_CameraWS + tmpvar_4)
  .y - _Y.x) * _Y.y));
  highp vec4 tmpvar_7;
  tmpvar_7 = mix (tmpvar_5, _FogColor, vec4(((1.0 - 
    exp((-(_GlobalDensity) * (clamp (
      ((sqrt(dot (tmpvar_4.xyz, tmpvar_4.xyz)) * _StartDistance.x) - 1.0)
    , 0.0, 1.0) * _StartDistance.y)))
  ) * exp(
    -((tmpvar_6 * tmpvar_6))
  ))));
  tmpvar_1 = tmpvar_7;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _FrustumCornersWS;
out highp vec2 xlv_TEXCOORD0;
out highp vec2 xlv_TEXCOORD1;
out highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_3;
  tmpvar_3.xyw = tmpvar_1.xyw;
  mediump float index_4;
  highp vec2 tmpvar_5;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  highp float tmpvar_8;
  tmpvar_8 = tmpvar_1.z;
  index_4 = tmpvar_8;
  tmpvar_3.z = 0.1;
  tmpvar_5 = tmpvar_2;
  tmpvar_6 = tmpvar_2;
  int i_9;
  i_9 = int(index_4);
  highp vec4 tmpvar_10;
  mediump vec4 v_11;
  v_11.x = _FrustumCornersWS[0][i_9];
  v_11.y = _FrustumCornersWS[1][i_9];
  v_11.z = _FrustumCornersWS[2][i_9];
  v_11.w = _FrustumCornersWS[3][i_9];
  tmpvar_10 = v_11;
  tmpvar_7.xyz = tmpvar_10.xyz;
  tmpvar_7.w = index_4;
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = tmpvar_6;
  xlv_TEXCOORD2 = tmpvar_7;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp float _GlobalDensity;
uniform highp vec4 _FogColor;
uniform highp vec4 _StartDistance;
uniform highp vec4 _Y;
uniform highp vec4 _CameraWS;
in highp vec2 xlv_TEXCOORD0;
in highp vec2 xlv_TEXCOORD1;
in highp vec4 xlv_TEXCOORD2;
void main ()
{
  mediump vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_3;
  z_3 = tmpvar_2.x;
  highp vec4 tmpvar_4;
  tmpvar_4 = ((1.0/((
    (_ZBufferParams.x * z_3)
   + _ZBufferParams.y))) * xlv_TEXCOORD2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_6;
  tmpvar_6 = max (0.0, ((
    (_CameraWS + tmpvar_4)
  .y - _Y.x) * _Y.y));
  highp vec4 tmpvar_7;
  tmpvar_7 = mix (tmpvar_5, _FogColor, vec4(((1.0 - 
    exp((-(_GlobalDensity) * (clamp (
      ((sqrt(dot (tmpvar_4.xyz, tmpvar_4.xyz)) * _StartDistance.x) - 1.0)
    , 0.0, 1.0) * _StartDistance.y)))
  ) * exp(
    -((tmpvar_6 * tmpvar_6))
  ))));
  tmpvar_1 = tmpvar_7;
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
uniform highp mat4 _FrustumCornersWS;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_3;
  tmpvar_3.xyw = tmpvar_1.xyw;
  mediump float index_4;
  highp vec2 tmpvar_5;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  highp float tmpvar_8;
  tmpvar_8 = tmpvar_1.z;
  index_4 = tmpvar_8;
  tmpvar_3.z = 0.1;
  tmpvar_5 = tmpvar_2;
  tmpvar_6 = tmpvar_2;
  int i_9;
  i_9 = int(index_4);
  highp vec4 tmpvar_10;
  mediump vec4 v_11;
  v_11.x = _FrustumCornersWS[0][i_9];
  v_11.y = _FrustumCornersWS[1][i_9];
  v_11.z = _FrustumCornersWS[2][i_9];
  v_11.w = _FrustumCornersWS[3][i_9];
  tmpvar_10 = v_11;
  tmpvar_7.xyz = tmpvar_10.xyz;
  tmpvar_7.w = index_4;
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = tmpvar_6;
  xlv_TEXCOORD2 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp vec4 _FogColor;
uniform highp vec4 _Y;
uniform highp vec4 _CameraWS;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  mediump vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_3;
  z_3 = tmpvar_2.x;
  highp float tmpvar_4;
  tmpvar_4 = max (0.0, ((
    (_CameraWS + ((1.0/((
      (_ZBufferParams.x * z_3)
     + _ZBufferParams.y))) * xlv_TEXCOORD2))
  .y - _Y.x) * _Y.y));
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp vec4 tmpvar_6;
  tmpvar_6 = mix (tmpvar_5, _FogColor, vec4(exp(-(
    (tmpvar_4 * tmpvar_4)
  ))));
  tmpvar_1 = tmpvar_6;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _FrustumCornersWS;
out highp vec2 xlv_TEXCOORD0;
out highp vec2 xlv_TEXCOORD1;
out highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_3;
  tmpvar_3.xyw = tmpvar_1.xyw;
  mediump float index_4;
  highp vec2 tmpvar_5;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  highp float tmpvar_8;
  tmpvar_8 = tmpvar_1.z;
  index_4 = tmpvar_8;
  tmpvar_3.z = 0.1;
  tmpvar_5 = tmpvar_2;
  tmpvar_6 = tmpvar_2;
  int i_9;
  i_9 = int(index_4);
  highp vec4 tmpvar_10;
  mediump vec4 v_11;
  v_11.x = _FrustumCornersWS[0][i_9];
  v_11.y = _FrustumCornersWS[1][i_9];
  v_11.z = _FrustumCornersWS[2][i_9];
  v_11.w = _FrustumCornersWS[3][i_9];
  tmpvar_10 = v_11;
  tmpvar_7.xyz = tmpvar_10.xyz;
  tmpvar_7.w = index_4;
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = tmpvar_6;
  xlv_TEXCOORD2 = tmpvar_7;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp vec4 _FogColor;
uniform highp vec4 _Y;
uniform highp vec4 _CameraWS;
in highp vec2 xlv_TEXCOORD0;
in highp vec2 xlv_TEXCOORD1;
in highp vec4 xlv_TEXCOORD2;
void main ()
{
  mediump vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_3;
  z_3 = tmpvar_2.x;
  highp float tmpvar_4;
  tmpvar_4 = max (0.0, ((
    (_CameraWS + ((1.0/((
      (_ZBufferParams.x * z_3)
     + _ZBufferParams.y))) * xlv_TEXCOORD2))
  .y - _Y.x) * _Y.y));
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture (_MainTex, xlv_TEXCOORD0);
  highp vec4 tmpvar_6;
  tmpvar_6 = mix (tmpvar_5, _FogColor, vec4(exp(-(
    (tmpvar_4 * tmpvar_4)
  ))));
  tmpvar_1 = tmpvar_6;
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
uniform highp mat4 _FrustumCornersWS;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_3;
  tmpvar_3.xyw = tmpvar_1.xyw;
  mediump float index_4;
  highp vec2 tmpvar_5;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  highp float tmpvar_8;
  tmpvar_8 = tmpvar_1.z;
  index_4 = tmpvar_8;
  tmpvar_3.z = 0.1;
  tmpvar_5 = tmpvar_2;
  tmpvar_6 = tmpvar_2;
  int i_9;
  i_9 = int(index_4);
  highp vec4 tmpvar_10;
  mediump vec4 v_11;
  v_11.x = _FrustumCornersWS[0][i_9];
  v_11.y = _FrustumCornersWS[1][i_9];
  v_11.z = _FrustumCornersWS[2][i_9];
  v_11.w = _FrustumCornersWS[3][i_9];
  tmpvar_10 = v_11;
  tmpvar_7.xyz = tmpvar_10.xyz;
  tmpvar_7.w = index_4;
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = tmpvar_6;
  xlv_TEXCOORD2 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp float _GlobalDensity;
uniform highp vec4 _FogColor;
uniform highp vec4 _StartDistance;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  mediump vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_3;
  z_3 = tmpvar_2.x;
  highp vec4 tmpvar_4;
  tmpvar_4 = ((1.0/((
    (_ZBufferParams.x * z_3)
   + _ZBufferParams.y))) * xlv_TEXCOORD2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp vec4 tmpvar_6;
  tmpvar_6 = mix (_FogColor, tmpvar_5, vec4(exp((
    -(_GlobalDensity)
   * 
    (clamp (((
      sqrt(dot (tmpvar_4, tmpvar_4))
     * _StartDistance.x) - 1.0), 0.0, 1.0) * _StartDistance.y)
  ))));
  tmpvar_1 = tmpvar_6;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _FrustumCornersWS;
out highp vec2 xlv_TEXCOORD0;
out highp vec2 xlv_TEXCOORD1;
out highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_3;
  tmpvar_3.xyw = tmpvar_1.xyw;
  mediump float index_4;
  highp vec2 tmpvar_5;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  highp float tmpvar_8;
  tmpvar_8 = tmpvar_1.z;
  index_4 = tmpvar_8;
  tmpvar_3.z = 0.1;
  tmpvar_5 = tmpvar_2;
  tmpvar_6 = tmpvar_2;
  int i_9;
  i_9 = int(index_4);
  highp vec4 tmpvar_10;
  mediump vec4 v_11;
  v_11.x = _FrustumCornersWS[0][i_9];
  v_11.y = _FrustumCornersWS[1][i_9];
  v_11.z = _FrustumCornersWS[2][i_9];
  v_11.w = _FrustumCornersWS[3][i_9];
  tmpvar_10 = v_11;
  tmpvar_7.xyz = tmpvar_10.xyz;
  tmpvar_7.w = index_4;
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = tmpvar_6;
  xlv_TEXCOORD2 = tmpvar_7;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp float _GlobalDensity;
uniform highp vec4 _FogColor;
uniform highp vec4 _StartDistance;
in highp vec2 xlv_TEXCOORD0;
in highp vec2 xlv_TEXCOORD1;
in highp vec4 xlv_TEXCOORD2;
void main ()
{
  mediump vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_3;
  z_3 = tmpvar_2.x;
  highp vec4 tmpvar_4;
  tmpvar_4 = ((1.0/((
    (_ZBufferParams.x * z_3)
   + _ZBufferParams.y))) * xlv_TEXCOORD2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture (_MainTex, xlv_TEXCOORD0);
  highp vec4 tmpvar_6;
  tmpvar_6 = mix (_FogColor, tmpvar_5, vec4(exp((
    -(_GlobalDensity)
   * 
    (clamp (((
      sqrt(dot (tmpvar_4, tmpvar_4))
     * _StartDistance.x) - 1.0), 0.0, 1.0) * _StartDistance.y)
  ))));
  tmpvar_1 = tmpvar_6;
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
uniform highp mat4 _FrustumCornersWS;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_3;
  tmpvar_3.xyw = tmpvar_1.xyw;
  mediump float index_4;
  highp vec2 tmpvar_5;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  highp float tmpvar_8;
  tmpvar_8 = tmpvar_1.z;
  index_4 = tmpvar_8;
  tmpvar_3.z = 0.1;
  tmpvar_5 = tmpvar_2;
  tmpvar_6 = tmpvar_2;
  int i_9;
  i_9 = int(index_4);
  highp vec4 tmpvar_10;
  mediump vec4 v_11;
  v_11.x = _FrustumCornersWS[0][i_9];
  v_11.y = _FrustumCornersWS[1][i_9];
  v_11.z = _FrustumCornersWS[2][i_9];
  v_11.w = _FrustumCornersWS[3][i_9];
  tmpvar_10 = v_11;
  tmpvar_7.xyz = tmpvar_10.xyz;
  tmpvar_7.w = index_4;
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = tmpvar_6;
  xlv_TEXCOORD2 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp float _GlobalDensity;
uniform highp vec4 _FogColor;
uniform highp vec4 _StartDistance;
uniform highp vec4 _Y;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  mediump vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_3;
  z_3 = tmpvar_2.x;
  highp vec4 tmpvar_4;
  tmpvar_4 = ((1.0/((
    (_ZBufferParams.x * z_3)
   + _ZBufferParams.y))) * xlv_TEXCOORD2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_6;
  tmpvar_6 = max (0.0, ((tmpvar_4.y - _Y.x) * _Y.y));
  highp vec4 tmpvar_7;
  tmpvar_7 = mix (tmpvar_5, _FogColor, vec4(((1.0 - 
    exp((-(_GlobalDensity) * (clamp (
      ((sqrt(dot (tmpvar_4.xyz, tmpvar_4.xyz)) * _StartDistance.x) - 1.0)
    , 0.0, 1.0) * _StartDistance.y)))
  ) * exp(
    -((tmpvar_6 * tmpvar_6))
  ))));
  tmpvar_1 = tmpvar_7;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _FrustumCornersWS;
out highp vec2 xlv_TEXCOORD0;
out highp vec2 xlv_TEXCOORD1;
out highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesVertex;
  mediump vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_3;
  tmpvar_3.xyw = tmpvar_1.xyw;
  mediump float index_4;
  highp vec2 tmpvar_5;
  highp vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  highp float tmpvar_8;
  tmpvar_8 = tmpvar_1.z;
  index_4 = tmpvar_8;
  tmpvar_3.z = 0.1;
  tmpvar_5 = tmpvar_2;
  tmpvar_6 = tmpvar_2;
  int i_9;
  i_9 = int(index_4);
  highp vec4 tmpvar_10;
  mediump vec4 v_11;
  v_11.x = _FrustumCornersWS[0][i_9];
  v_11.y = _FrustumCornersWS[1][i_9];
  v_11.z = _FrustumCornersWS[2][i_9];
  v_11.w = _FrustumCornersWS[3][i_9];
  tmpvar_10 = v_11;
  tmpvar_7.xyz = tmpvar_10.xyz;
  tmpvar_7.w = index_4;
  gl_Position = (glstate_matrix_mvp * tmpvar_3);
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = tmpvar_6;
  xlv_TEXCOORD2 = tmpvar_7;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform highp float _GlobalDensity;
uniform highp vec4 _FogColor;
uniform highp vec4 _StartDistance;
uniform highp vec4 _Y;
in highp vec2 xlv_TEXCOORD0;
in highp vec2 xlv_TEXCOORD1;
in highp vec4 xlv_TEXCOORD2;
void main ()
{
  mediump vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_CameraDepthTexture, xlv_TEXCOORD1);
  highp float z_3;
  z_3 = tmpvar_2.x;
  highp vec4 tmpvar_4;
  tmpvar_4 = ((1.0/((
    (_ZBufferParams.x * z_3)
   + _ZBufferParams.y))) * xlv_TEXCOORD2);
  lowp vec4 tmpvar_5;
  tmpvar_5 = texture (_MainTex, xlv_TEXCOORD0);
  highp float tmpvar_6;
  tmpvar_6 = max (0.0, ((tmpvar_4.y - _Y.x) * _Y.y));
  highp vec4 tmpvar_7;
  tmpvar_7 = mix (tmpvar_5, _FogColor, vec4(((1.0 - 
    exp((-(_GlobalDensity) * (clamp (
      ((sqrt(dot (tmpvar_4.xyz, tmpvar_4.xyz)) * _StartDistance.x) - 1.0)
    , 0.0, 1.0) * _StartDistance.y)))
  ) * exp(
    -((tmpvar_6 * tmpvar_6))
  ))));
  tmpvar_1 = tmpvar_7;
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
Fallback Off
}