#version 310 es
// Diligent Engine can't do these video-related EGL extensions, too exotic.
// Instead of supporting them there which would be a lot of work, implemented a shortcut.
// When the GLSL shader source starts with #version magic string, Diligent's preprocessor doesn't do it's thing and uses the GLSL without any extra transformations.
#extension GL_OES_EGL_image_external : require
#extension GL_OES_EGL_image_external_essl3 : require

// These textures are backed by V4L2 buffers, they are imported to GLES using DMA buffer import/export OS kernel feature.
uniform lowp samplerExternalOES videoTexture;

layout( location = 0 ) in highp vec2 texCoord;
layout( location = 0 ) out lowp vec4 outputColor;

void main()
{
	highp vec2 clipped = clamp( texCoord, vec2( $( UV_MIN ) ), vec2( $( UV_MAX ) ) );
	if( clipped == texCoord )
	{
		// This pixel is inside of the valid video rectangle, sample from the video texture
		lowp vec3 color = texture( videoTexture, texCoord ).rgb;
		outputColor = vec4( color, 1.0 );
	}
	else
	{
		// This pixel is on the outer border of the video frame. Paint it with the specified border color.
		outputColor = vec4( $( BORDER_COLOR ) );
	}
}