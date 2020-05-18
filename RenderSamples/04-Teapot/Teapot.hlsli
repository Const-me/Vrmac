struct sTeapotVertexOutput
{
	// Position relative to the camera
	float3 viewPos : TEXCOORD0;
	// Normal
	float3 normal: TEXCOORD1;
	// Position in clip space
	float4 position : SV_Position;
};