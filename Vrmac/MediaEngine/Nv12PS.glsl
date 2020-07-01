#version 310 es
// Diligent Engine can't do these video-related EGL extensions, too exotic.
// Instead of supporting them there which would be a lot of work, implemented a shortcut.
// When the GLSL shader source starts with #version magic string, Diligent's preprocessor doesn't do it's thing and uses the GLSL without any extra transformations.
#extension GL_OES_EGL_image_external : require
#extension GL_OES_EGL_image_external_essl3 : require

uniform lowp samplerExternalOES nv12;

layout( location = 0 ) in highp vec2 uv;
layout( location = 0 ) out lowp vec4 outputColor;

// Pass-through pixel shader that loads pixels from external NV12 texture.
// The source textures are backed by V4L2 buffers, they are imported to GLES using DMA buffer import/export OS kernel feature.
void main()
{
	lowp vec3 color = texture( nv12, uv ).rgb;
	outputColor = vec4( color, 1.0 );
}