float4 main( uint index: SV_VertexID ): SV_Position
{
	const float2 uv = float2( uint2( index, index << 1 ) & 2 );
	const float2 pos = lerp( float2( -1, 1 ), float2( 1, -1 ), uv );
	return float4( pos, 0, 1 );
}