Texture2D<float3> nv12;
SamplerState nv12_sampler;

float4 main( float2 uv: TEXCOORD0 ) : SV_Target
{
	float3 color = nv12.Sample( nv12_sampler, uv );
	return float4( color, 1.0 );
	Compiler_error
}