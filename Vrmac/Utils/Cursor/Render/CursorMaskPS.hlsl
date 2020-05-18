Texture2D    g_Texture;
SamplerState g_Texture_sampler;

struct PSInput
{
	float4 position : SV_POSITION;
	float2 texcoord : TEX_COORD;
};

struct PSOutput
{
	float4 color : SV_TARGET;
};

void main( in PSInput PSIn, out PSOutput PSOut )
{
	float r = g_Texture.Sample( g_Texture_sampler, PSIn.texcoord ).x;
	if( r < 0.75 )
	{
		PSOut.color = float4( 0.0, 0.0, 0.0, 0.0 );
		discard;
	}
	else
	{
		PSOut.color = float4( 1.0, 1.0, 1.0, 1.0 );
	}
}