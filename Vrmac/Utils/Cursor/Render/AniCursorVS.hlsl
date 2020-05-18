cbuffer CursorCB
{
	float4 positionAndSize;
	uint frame;
};

struct CursorVertex
{
	float2 position : ATTRIB0;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float2 texcoord : TEX_COORD;
};

void main( in CursorVertex vertex, out PSInput PSIn )
{
	float x = positionAndSize.x + positionAndSize.z * vertex.position.x;
	// In clip space, Y axis is upwards.
	float y = positionAndSize.y - positionAndSize.w * ( 1.0 - vertex.position.y );
	PSIn.position = float4( x, y, 0.0, 1.0 );
	PSIn.texcoord = vertex.position;
}