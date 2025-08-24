¶ÛShader "Transparent Effects/CheapForcefield2" {
Properties {
 _Scale ("_Scale", Range(0.1,4)) = 0.15
 _Color ("_Color", Color) = (0,1,0,1)
 _Texture ("_Texture", 2D) = "white" {}
 _Properties ("Rim, Strength, Speed, Tile ", Vector) = (1.2,1.5,0.5,5)
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  highp vec3 shlight_3;
  mediump vec2 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_4 = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_9 = tmpvar_1.xyz;
  tmpvar_10 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_11;
  tmpvar_11[0].x = tmpvar_9.x;
  tmpvar_11[0].y = tmpvar_10.x;
  tmpvar_11[0].z = tmpvar_2.x;
  tmpvar_11[1].x = tmpvar_9.y;
  tmpvar_11[1].y = tmpvar_10.y;
  tmpvar_11[1].z = tmpvar_2.y;
  tmpvar_11[2].x = tmpvar_9.z;
  tmpvar_11[2].y = tmpvar_10.z;
  tmpvar_11[2].z = tmpvar_2.z;
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_5 = tmpvar_12;
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = _WorldSpaceCameraPos;
  highp vec4 tmpvar_14;
  tmpvar_14.w = 1.0;
  tmpvar_14.xyz = (tmpvar_8 * (tmpvar_2 * unity_Scale.w));
  mediump vec3 tmpvar_15;
  mediump vec4 normal_16;
  normal_16 = tmpvar_14;
  highp float vC_17;
  mediump vec3 x3_18;
  mediump vec3 x2_19;
  mediump vec3 x1_20;
  highp float tmpvar_21;
  tmpvar_21 = dot (unity_SHAr, normal_16);
  x1_20.x = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = dot (unity_SHAg, normal_16);
  x1_20.y = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAb, normal_16);
  x1_20.z = tmpvar_23;
  mediump vec4 tmpvar_24;
  tmpvar_24 = (normal_16.xyzz * normal_16.yzzx);
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHBr, tmpvar_24);
  x2_19.x = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHBg, tmpvar_24);
  x2_19.y = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBb, tmpvar_24);
  x2_19.z = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = ((normal_16.x * normal_16.x) - (normal_16.y * normal_16.y));
  vC_17 = tmpvar_28;
  highp vec3 tmpvar_29;
  tmpvar_29 = (unity_SHC.xyz * vC_17);
  x3_18 = tmpvar_29;
  tmpvar_15 = ((x1_20 + x2_19) + x3_18);
  shlight_3 = tmpvar_15;
  tmpvar_6 = shlight_3;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_4;
  xlv_TEXCOORD1 = (tmpvar_11 * ((
    (_World2Object * tmpvar_13)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  c_1.xyz = _Color.xyz;
  c_1.w = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out lowp vec3 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  highp vec3 shlight_3;
  mediump vec2 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_4 = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_9 = tmpvar_1.xyz;
  tmpvar_10 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_11;
  tmpvar_11[0].x = tmpvar_9.x;
  tmpvar_11[0].y = tmpvar_10.x;
  tmpvar_11[0].z = tmpvar_2.x;
  tmpvar_11[1].x = tmpvar_9.y;
  tmpvar_11[1].y = tmpvar_10.y;
  tmpvar_11[1].z = tmpvar_2.y;
  tmpvar_11[2].x = tmpvar_9.z;
  tmpvar_11[2].y = tmpvar_10.z;
  tmpvar_11[2].z = tmpvar_2.z;
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_5 = tmpvar_12;
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = _WorldSpaceCameraPos;
  highp vec4 tmpvar_14;
  tmpvar_14.w = 1.0;
  tmpvar_14.xyz = (tmpvar_8 * (tmpvar_2 * unity_Scale.w));
  mediump vec3 tmpvar_15;
  mediump vec4 normal_16;
  normal_16 = tmpvar_14;
  highp float vC_17;
  mediump vec3 x3_18;
  mediump vec3 x2_19;
  mediump vec3 x1_20;
  highp float tmpvar_21;
  tmpvar_21 = dot (unity_SHAr, normal_16);
  x1_20.x = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = dot (unity_SHAg, normal_16);
  x1_20.y = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAb, normal_16);
  x1_20.z = tmpvar_23;
  mediump vec4 tmpvar_24;
  tmpvar_24 = (normal_16.xyzz * normal_16.yzzx);
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHBr, tmpvar_24);
  x2_19.x = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHBg, tmpvar_24);
  x2_19.y = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBb, tmpvar_24);
  x2_19.z = tmpvar_27;
  mediump float tmpvar_28;
  tmpvar_28 = ((normal_16.x * normal_16.x) - (normal_16.y * normal_16.y));
  vC_17 = tmpvar_28;
  highp vec3 tmpvar_29;
  tmpvar_29 = (unity_SHC.xyz * vC_17);
  x3_18 = tmpvar_29;
  tmpvar_15 = ((x1_20 + x2_19) + x3_18);
  shlight_3 = tmpvar_15;
  tmpvar_6 = shlight_3;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_4;
  xlv_TEXCOORD1 = (tmpvar_11 * ((
    (_World2Object * tmpvar_13)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  c_1.xyz = _Color.xyz;
  c_1.w = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_4;
  highp vec3 tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_5 = tmpvar_1.xyz;
  tmpvar_6 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_5.x;
  tmpvar_7[0].y = tmpvar_6.x;
  tmpvar_7[0].z = tmpvar_2.x;
  tmpvar_7[1].x = tmpvar_5.y;
  tmpvar_7[1].y = tmpvar_6.y;
  tmpvar_7[1].z = tmpvar_2.y;
  tmpvar_7[2].x = tmpvar_5.z;
  tmpvar_7[2].y = tmpvar_6.z;
  tmpvar_7[2].z = tmpvar_2.z;
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_7 * ((
    (_World2Object * tmpvar_8)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  c_1.xyz = _Color.xyz;
  c_1.w = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out highp vec2 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_4;
  highp vec3 tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_5 = tmpvar_1.xyz;
  tmpvar_6 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_5.x;
  tmpvar_7[0].y = tmpvar_6.x;
  tmpvar_7[0].z = tmpvar_2.x;
  tmpvar_7[1].x = tmpvar_5.y;
  tmpvar_7[1].y = tmpvar_6.y;
  tmpvar_7[1].z = tmpvar_2.y;
  tmpvar_7[2].x = tmpvar_5.z;
  tmpvar_7[2].y = tmpvar_6.z;
  tmpvar_7[2].z = tmpvar_2.z;
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_7 * ((
    (_World2Object * tmpvar_8)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  c_1.xyz = _Color.xyz;
  c_1.w = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_4;
  highp vec3 tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_5 = tmpvar_1.xyz;
  tmpvar_6 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_5.x;
  tmpvar_7[0].y = tmpvar_6.x;
  tmpvar_7[0].z = tmpvar_2.x;
  tmpvar_7[1].x = tmpvar_5.y;
  tmpvar_7[1].y = tmpvar_6.y;
  tmpvar_7[1].z = tmpvar_2.y;
  tmpvar_7[2].x = tmpvar_5.z;
  tmpvar_7[2].y = tmpvar_6.z;
  tmpvar_7[2].z = tmpvar_2.z;
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_7 * ((
    (_World2Object * tmpvar_8)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  c_1.xyz = _Color.xyz;
  c_1.w = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out highp vec2 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_4;
  highp vec3 tmpvar_5;
  highp vec3 tmpvar_6;
  tmpvar_5 = tmpvar_1.xyz;
  tmpvar_6 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_7;
  tmpvar_7[0].x = tmpvar_5.x;
  tmpvar_7[0].y = tmpvar_6.x;
  tmpvar_7[0].z = tmpvar_2.x;
  tmpvar_7[1].x = tmpvar_5.y;
  tmpvar_7[1].y = tmpvar_6.y;
  tmpvar_7[1].z = tmpvar_2.y;
  tmpvar_7[2].x = tmpvar_5.z;
  tmpvar_7[2].y = tmpvar_6.z;
  tmpvar_7[2].z = tmpvar_2.z;
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_7 * ((
    (_World2Object * tmpvar_8)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  c_1.xyz = _Color.xyz;
  c_1.w = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  highp vec3 shlight_3;
  mediump vec2 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_4 = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_2 * unity_Scale.w));
  highp vec3 tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_10 = tmpvar_1.xyz;
  tmpvar_11 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_12;
  tmpvar_12[0].x = tmpvar_10.x;
  tmpvar_12[0].y = tmpvar_11.x;
  tmpvar_12[0].z = tmpvar_2.x;
  tmpvar_12[1].x = tmpvar_10.y;
  tmpvar_12[1].y = tmpvar_11.y;
  tmpvar_12[1].z = tmpvar_2.y;
  tmpvar_12[2].x = tmpvar_10.z;
  tmpvar_12[2].y = tmpvar_11.z;
  tmpvar_12[2].z = tmpvar_2.z;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_5 = tmpvar_13;
  highp vec4 tmpvar_14;
  tmpvar_14.w = 1.0;
  tmpvar_14.xyz = _WorldSpaceCameraPos;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_9;
  mediump vec3 tmpvar_16;
  mediump vec4 normal_17;
  normal_17 = tmpvar_15;
  highp float vC_18;
  mediump vec3 x3_19;
  mediump vec3 x2_20;
  mediump vec3 x1_21;
  highp float tmpvar_22;
  tmpvar_22 = dot (unity_SHAr, normal_17);
  x1_21.x = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAg, normal_17);
  x1_21.y = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAb, normal_17);
  x1_21.z = tmpvar_24;
  mediump vec4 tmpvar_25;
  tmpvar_25 = (normal_17.xyzz * normal_17.yzzx);
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHBr, tmpvar_25);
  x2_20.x = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBg, tmpvar_25);
  x2_20.y = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBb, tmpvar_25);
  x2_20.z = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = ((normal_17.x * normal_17.x) - (normal_17.y * normal_17.y));
  vC_18 = tmpvar_29;
  highp vec3 tmpvar_30;
  tmpvar_30 = (unity_SHC.xyz * vC_18);
  x3_19 = tmpvar_30;
  tmpvar_16 = ((x1_21 + x2_20) + x3_19);
  shlight_3 = tmpvar_16;
  tmpvar_6 = shlight_3;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_32;
  tmpvar_32 = (unity_4LightPosX0 - tmpvar_31.x);
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosY0 - tmpvar_31.y);
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosZ0 - tmpvar_31.z);
  highp vec4 tmpvar_35;
  tmpvar_35 = (((tmpvar_32 * tmpvar_32) + (tmpvar_33 * tmpvar_33)) + (tmpvar_34 * tmpvar_34));
  highp vec4 tmpvar_36;
  tmpvar_36 = (max (vec4(0.0, 0.0, 0.0, 0.0), (
    (((tmpvar_32 * tmpvar_9.x) + (tmpvar_33 * tmpvar_9.y)) + (tmpvar_34 * tmpvar_9.z))
   * 
    inversesqrt(tmpvar_35)
  )) * (1.0/((1.0 + 
    (tmpvar_35 * unity_4LightAtten0)
  ))));
  highp vec3 tmpvar_37;
  tmpvar_37 = (tmpvar_6 + ((
    ((unity_LightColor[0].xyz * tmpvar_36.x) + (unity_LightColor[1].xyz * tmpvar_36.y))
   + 
    (unity_LightColor[2].xyz * tmpvar_36.z)
  ) + (unity_LightColor[3].xyz * tmpvar_36.w)));
  tmpvar_6 = tmpvar_37;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_4;
  xlv_TEXCOORD1 = (tmpvar_12 * ((
    (_World2Object * tmpvar_14)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  c_1.xyz = _Color.xyz;
  c_1.w = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out lowp vec3 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  highp vec3 shlight_3;
  mediump vec2 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_4 = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_2 * unity_Scale.w));
  highp vec3 tmpvar_10;
  highp vec3 tmpvar_11;
  tmpvar_10 = tmpvar_1.xyz;
  tmpvar_11 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_12;
  tmpvar_12[0].x = tmpvar_10.x;
  tmpvar_12[0].y = tmpvar_11.x;
  tmpvar_12[0].z = tmpvar_2.x;
  tmpvar_12[1].x = tmpvar_10.y;
  tmpvar_12[1].y = tmpvar_11.y;
  tmpvar_12[1].z = tmpvar_2.y;
  tmpvar_12[2].x = tmpvar_10.z;
  tmpvar_12[2].y = tmpvar_11.z;
  tmpvar_12[2].z = tmpvar_2.z;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_5 = tmpvar_13;
  highp vec4 tmpvar_14;
  tmpvar_14.w = 1.0;
  tmpvar_14.xyz = _WorldSpaceCameraPos;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_9;
  mediump vec3 tmpvar_16;
  mediump vec4 normal_17;
  normal_17 = tmpvar_15;
  highp float vC_18;
  mediump vec3 x3_19;
  mediump vec3 x2_20;
  mediump vec3 x1_21;
  highp float tmpvar_22;
  tmpvar_22 = dot (unity_SHAr, normal_17);
  x1_21.x = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAg, normal_17);
  x1_21.y = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAb, normal_17);
  x1_21.z = tmpvar_24;
  mediump vec4 tmpvar_25;
  tmpvar_25 = (normal_17.xyzz * normal_17.yzzx);
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHBr, tmpvar_25);
  x2_20.x = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBg, tmpvar_25);
  x2_20.y = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBb, tmpvar_25);
  x2_20.z = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = ((normal_17.x * normal_17.x) - (normal_17.y * normal_17.y));
  vC_18 = tmpvar_29;
  highp vec3 tmpvar_30;
  tmpvar_30 = (unity_SHC.xyz * vC_18);
  x3_19 = tmpvar_30;
  tmpvar_16 = ((x1_21 + x2_20) + x3_19);
  shlight_3 = tmpvar_16;
  tmpvar_6 = shlight_3;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_32;
  tmpvar_32 = (unity_4LightPosX0 - tmpvar_31.x);
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosY0 - tmpvar_31.y);
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosZ0 - tmpvar_31.z);
  highp vec4 tmpvar_35;
  tmpvar_35 = (((tmpvar_32 * tmpvar_32) + (tmpvar_33 * tmpvar_33)) + (tmpvar_34 * tmpvar_34));
  highp vec4 tmpvar_36;
  tmpvar_36 = (max (vec4(0.0, 0.0, 0.0, 0.0), (
    (((tmpvar_32 * tmpvar_9.x) + (tmpvar_33 * tmpvar_9.y)) + (tmpvar_34 * tmpvar_9.z))
   * 
    inversesqrt(tmpvar_35)
  )) * (1.0/((1.0 + 
    (tmpvar_35 * unity_4LightAtten0)
  ))));
  highp vec3 tmpvar_37;
  tmpvar_37 = (tmpvar_6 + ((
    ((unity_LightColor[0].xyz * tmpvar_36.x) + (unity_LightColor[1].xyz * tmpvar_36.y))
   + 
    (unity_LightColor[2].xyz * tmpvar_36.z)
  ) + (unity_LightColor[3].xyz * tmpvar_36.w)));
  tmpvar_6 = tmpvar_37;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_4;
  xlv_TEXCOORD1 = (tmpvar_12 * ((
    (_World2Object * tmpvar_14)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  c_1.xyz = _Color.xyz;
  c_1.w = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  _glesFragData[0] = c_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES3"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  AlphaTest Greater 0
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * ((
    (_World2Object * _WorldSpaceLightPos0)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out mediump vec3 xlv_TEXCOORD2;
out highp vec3 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * ((
    (_World2Object * _WorldSpaceLightPos0)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out mediump vec3 xlv_TEXCOORD2;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec4 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * ((
    (_World2Object * _WorldSpaceLightPos0)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out mediump vec3 xlv_TEXCOORD2;
out highp vec4 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * ((
    (_World2Object * _WorldSpaceLightPos0)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * ((
    (_World2Object * _WorldSpaceLightPos0)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out mediump vec3 xlv_TEXCOORD2;
out highp vec3 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * ((
    (_World2Object * _WorldSpaceLightPos0)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _Texture_ST;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesTANGENT;
uniform highp vec3 _WorldSpaceCameraPos;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp vec4 _Texture_ST;
out mediump vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
out mediump vec3 xlv_TEXCOORD2;
out highp vec2 xlv_TEXCOORD3;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = ((_glesMultiTexCoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
  tmpvar_3 = tmpvar_5;
  highp vec3 tmpvar_6;
  highp vec3 tmpvar_7;
  tmpvar_6 = tmpvar_1.xyz;
  tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp mat3 tmpvar_8;
  tmpvar_8[0].x = tmpvar_6.x;
  tmpvar_8[0].y = tmpvar_7.x;
  tmpvar_8[0].z = tmpvar_2.x;
  tmpvar_8[1].x = tmpvar_6.y;
  tmpvar_8[1].y = tmpvar_7.y;
  tmpvar_8[1].z = tmpvar_2.y;
  tmpvar_8[2].x = tmpvar_6.z;
  tmpvar_8[2].y = tmpvar_7.z;
  tmpvar_8[2].z = tmpvar_2.z;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (_World2Object * _WorldSpaceLightPos0).xyz);
  tmpvar_4 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = (tmpvar_8 * ((
    (_World2Object * tmpvar_10)
  .xyz * unity_Scale.w) - _glesVertex.xyz));
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _Time;
uniform lowp vec4 _Color;
uniform sampler2D _Texture;
uniform lowp vec4 _Properties;
in mediump vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 c_1;
  mediump vec3 tmpvar_2;
  tmpvar_2 = xlv_TEXCOORD1;
  mediump vec2 texCoords_3;
  lowp float tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = normalize(tmpvar_2).z;
  tmpvar_4 = (1.0 - tmpvar_5);
  highp vec2 tmpvar_6;
  tmpvar_6.x = (xlv_TEXCOORD0.x + (_Time.x * _Properties.z));
  tmpvar_6.y = xlv_TEXCOORD0.y;
  highp vec2 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * _Properties.ww);
  texCoords_3 = tmpvar_7;
  lowp float tmpvar_8;
  tmpvar_8 = (((texture (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
  lowp vec4 c_9;
  c_9.xyz = vec3(0.0, 0.0, 0.0);
  highp float tmpvar_10;
  tmpvar_10 = tmpvar_8;
  c_9.w = tmpvar_10;
  c_1.xyz = c_9.xyz;
  c_1.w = tmpvar_8;
  _glesFragData[0] = c_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES3"
}
}
 }
}
Fallback "Diffuse"
}