¥Shader "CPX_Custom/Mobile/Unlit Color GLSL" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("Main Color", Color) = (1,1,1,1)
}
SubShader { 
 Pass {
Program "vp" {
SubProgram "gles " {
"!!GLES
#ifndef SHADER_API_GLES
    #define SHADER_API_GLES 1
#endif
#ifndef SHADER_API_MOBILE
    #define SHADER_API_MOBILE 1
#endif
#line 6
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif

#line 6
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif

         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _Color;
         uniform lowp vec4 _MainTex_ST;
         varying lowp vec2 uv;
                           

#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform highp mat4 glstate_matrix_mvp;
#define gl_Vertex _glesVertex
attribute vec4 _glesVertex;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
attribute vec4 _glesMultiTexCoord0;

         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         
#endif
#ifdef FRAGMENT

         void main(){
            vec4 color = texture2D(_MainTex, uv);
            gl_FragColor = (color + color) * _Color;
         }
         
#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es
#ifndef SHADER_API_GLES3
    #define SHADER_API_GLES3 1
#endif
#ifndef SHADER_API_MOBILE
    #define SHADER_API_MOBILE 1
#endif
// version 300 es
#line 6
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif

#line 6
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif

         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _Color;
         uniform lowp vec4 _MainTex_ST;
         varying lowp vec2 uv;
                           

#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform highp mat4 glstate_matrix_mvp;
#define gl_Vertex _glesVertex
in vec4 _glesVertex;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;

         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         
#endif
#ifdef FRAGMENT
#define gl_FragColor _glesFragData[0]
#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];

         void main(){
            vec4 color = texture(_MainTex, uv);
            gl_FragColor = (color + color) * _Color;
         }
         
#endif"
}
}
 }
}
Fallback "Unlit/Texture"
}