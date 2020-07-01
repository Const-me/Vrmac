struct VideoVertex
{
	float2 texCoord: TEXCOORD0;
	float4 position : SV_Position;
};

struct VSInput
{
	float2 position : ATTRIB0;
	float2 texCoord  : ATTRIB1;
};

// Trivial passthrough VS, just expands 2D positions into 4D
VideoVertex main( VSInput vsi )
{
	VideoVertex result;
	result.texCoord = vsi.texCoord;
	result.position = float4( vsi.position, 0.5, 1 );
	return result;
}