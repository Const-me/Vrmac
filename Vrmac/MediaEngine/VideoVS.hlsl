struct VideoVertex
{
	float2 texCoord: TEXCOORD0;
	float4 position : SV_Position;
};

// Full-screen triangle shader which outputs both position and texture coordinates
VideoVertex main( uint index: SV_VertexID )
{
	int id = int( index );
	float2 uv = float2( ( id << 1 ) & 2, id & 2 );
	float2 pos = uv * float2( 2.0, 2.0 ) + float2( -1.0, -1.0 );

	VideoVertex result;
	result.texCoord = uv;
	result.position = float4( pos, 0.5, 1 );
	return result;
}