uniform sampler2DMS g_Texture;
out lowp vec4 fragColor;

void main()
{
	ivec2 readLocation = ivec2( gl_FragCoord.xy );

	mediump vec4 result = vec4( 0.0, 0.0, 0.0, 0.0 );
	for( int i = 0; i < samplesCount; i++ )
		result += texelFetch( g_Texture, readLocation, i );

	mediump float scale = 1.0 / float( samplesCount );
	fragColor = result * scale;
}