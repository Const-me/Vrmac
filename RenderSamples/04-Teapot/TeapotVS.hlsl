cbuffer Constants
{
	float4x4 worldViewProj;
	float4x4 worldView;
};

// STL files only have position, but STL loader has an option to compute per-vertex normals.
struct StlVertexWithNormal
{
	float3 position : ATTRIB0;
	float3 normal   : ATTRIB1;
};

#include "Teapot.hlsli"

static const float3 cameraPos = float3( 0.0, 0.0, 0.0 );

void main( in StlVertexWithNormal stl, out sTeapotVertexOutput result )
{
	float4 pos4 = float4( stl.position, 1.0 );
	float4 viewSpace = mul( pos4, worldView );

	result.viewPos = viewSpace.xyz / viewSpace.w;
	result.position = mul( pos4, worldViewProj );
#ifdef HLSL
	// HLSL and GLSL have incompatible syntax for casting stuff.
	float3x3 vw3 = (float3x3)worldView;
#else
	float3x3 vw3 = float3x3( worldView );
#endif
	result.normal = normalize( mul( stl.normal, vw3 ) );
}