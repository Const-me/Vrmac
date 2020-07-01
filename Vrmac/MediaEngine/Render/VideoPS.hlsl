Texture2D<float4> videoTexture;
SamplerState videoTexture_sampler;

float4 main( float2 texCoord: TEXCOORD0 ): SV_Target
{
	float2 clipped = min( max( texCoord, float2( UV_MIN ) ), float2( UV_MAX ) );
	if( all( texCoord == clipped ) )
	{
		// This pixel is inside of the valid video rectangle, sample from the video texture
		return videoTexture.Sample( videoTexture_sampler, texCoord );
	}
	else
	{
		// This pixel is on the outer border of the video frame. Paint it with the specified border color.
		return float4( BORDER_COLOR );
	}
}