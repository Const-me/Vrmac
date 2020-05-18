void main()
{
	vec2 texcoord = vec2( ivec2( gl_VertexID, gl_VertexID << 1 ) & 2 );
	gl_Position = vec4( mix( vec2( -1, 1 ), vec2( 1, -1 ), texcoord ), 0, 1 );
}