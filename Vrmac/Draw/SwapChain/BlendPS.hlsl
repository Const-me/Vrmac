Texture2DMS<float4> g_Texture;

float4 main( float4 pos: SV_Position ): SV_TARGET
{
	const int2 readLocation = ( int2 )pos.xy;
	float4 result = 0;
	[unroll]
	for( uint i = 0; i < samplesCount; i++ )
		result += g_Texture.Load( readLocation, i );
	const float scale = 1.0f / samplesCount;
	return result * scale;
}