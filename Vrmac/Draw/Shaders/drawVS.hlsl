#include "drawUtils.hlsli"

struct VertexWithId
{
	float2 position : ATTRIB0;
	uint id: ATTRIB1;
};

struct sDrawCallData
{
	float4 rotation;
	float4 translationAndVaa;
	uint4 miscIntegers;
};

#if FEW_DRAW_CALLS
#define CBUFFER_LENGTH 64
#elif GL_ES
#define CBUFFER_LENGTH 1365	// 64 kb which is the limit for uniform buffers on Pi4
#else
#define CBUFFER_LENGTH 0
#endif

#if CBUFFER_LENGTH > 0
cbuffer DrawCallsCBuffer
{
	sDrawCallData drawCalls[ CBUFFER_LENGTH ];
};

INLINE sDrawCallData getDrawCall( uint idx )
{
	return drawCalls[ idx ];
}
#else
// Direct3D version uses real buffers, not these miniscule uniform ones.
// Too bad Pi4 doesn't support SSBO for vertex shaders.
Buffer<float4> drawCalls;

inline sDrawCallData getDrawCall( uint idx )
{
	idx *= 3;
	sDrawCallData res;
	// GPUs should be OK broadcasting values. nVidia has broadcasting silicon for reading from local memory:
	// https://docs.nvidia.com/cuda/cuda-c-programming-guide/index.html#shared-memory-5-x__examples-of-irregular-shared-memory-accesses
	// Hopefully, it will be also used here, for broadcasting from global memory.
	res.rotation = drawCalls[ idx ];
	res.translationAndVaa = drawCalls[ idx + 1 ];
	res.miscIntegers = asuint( drawCalls[ idx + 2 ] );
	return res;
}
#endif

// Transform point with 3x2 matrix
INLINE float4 transformPoint( float2 position, float4 rotation, float2 translation, float zValue )
{
	float2 val;
	val.x = dot( position, rotation.xz );
	val.y = dot( position, rotation.yw );
	val += translation;
	return float4( val, zValue, 1 );
}

INLINE int2 unpackFixedPoint( uint val )
{
	return int2 ( int( val & 0xFFFF ), int( val >> 16 ) );
}

// Unpack that rectangle, it's packed into 15 bits / coordinate.
// Texture sizes have relatively small limit, the precision is enough.
// On that Linux, GLES doesn't support conventional buffers, the draw calls are in uniform buffer, they have very small size limit, that's why packing.
INLINE float4 unpackTextureRect( uint leftTop, uint rightBottom )
{
	int4 intVec = int4( unpackFixedPoint( leftTop ), unpackFixedPoint( rightBottom ) );
	float4 floatVec = float4( intVec );
	// Sprite coordinates use 1.15 fixed point representation
	return floatVec * ( 1.0 / 32768.0 );
}

INLINE float3 computeSpriteTexCoords( uint vertexValue, uint4 miscIntegers )
{
	float4 rect = unpackTextureRect( miscIntegers.z, miscIntegers.w );
	int2 uvInt = int2(
		int( vertexValue & 0x1 ),
		int( ( vertexValue >> 1 ) & 0x1 ) );

	float2 uv = float2( uvInt );
	float2 oneMunisUv = float2( 1.0, 1.0 ) - uv;

	float3 result;
	result.xy = rect.xy * oneMunisUv + rect.zw * uv;
	result.z = float( int( miscIntegers.y ) );
	return result;
}

INLINE float2 unpackGlyphPosition( float packedToFloat, float pixelSize )
{
	int2 posInt = unpackFixedPoint( asuint( packedToFloat ) );
	return float2( posInt ) * pixelSize;
}

INLINE float3 unpackGlyphTexCoords( float packedToFloat, uint vertexValue )
{
	int2 uvInt = unpackFixedPoint( asuint( packedToFloat ) );
	float3 result;
	// Sprite coordinates use 1.15 fixed point representation
	result.xy = float2( uvInt ) * ( 1.0 / 32768.0 );
	result.z = float( int( vertexValue & 0x3F ) );
	return result;
}

void main( in VertexWithId vertex, out sVertexOutput result )
{
	uint dcIndex = vertex.id >> 8;
	sDrawCallData dc = getDrawCall( dcIndex );
	uint id = dc.miscIntegers.x;
	result.id = id;

	float2 vertexPosition;
#if TEXT_RENDERING
	uint meshType = meshTypeFromId( id );
	if( meshType == 0x500 || meshType == 0x600 )	// eMesh.GlyphRun or eMesh.TransformedText
	{
		vertexPosition = unpackGlyphPosition( vertex.position.x, dc.translationAndVaa.w );
		result.position = transformPoint( vertexPosition, dc.rotation, dc.translationAndVaa.xy, dc.translationAndVaa.z );
		result.color = readColorFromPalette( dc.miscIntegers.y );
		result.backgroundColor = readColorFromPalette( dc.miscIntegers.z );
		result.textureCoordinates = unpackGlyphTexCoords( vertex.position.y, vertex.id );
		return;
	}
	else
		vertexPosition = vertex.position;
#else
	vertexPosition = vertex.position;
#endif
	
	result.position = transformPoint( vertexPosition, dc.rotation, dc.translationAndVaa.xy, dc.translationAndVaa.z );

#if TEXTURE_ATLAS
	if( brushFromId( id ) == 0x2 )	// eBrush.Sprite
	{
		result.textureCoordinates = computeSpriteTexCoords( vertex.id, dc.miscIntegers );
		result.color = readColorFromPalette( dc.miscIntegers.y >> 0x8 );
#if TEXT_RENDERING
		result.backgroundColor = result.color;
#endif
		return;
	}
#endif

#if TEXTURE_ATLAS || TEXT_RENDERING || !OPAQUE_PASS

	float vaaValue = 0.0;
	uint vaaType = vaaFromId( id );
	if( vaaType == 0x10000 )
	{
		// Filled mesh VAA
		vaaValue = float( int( vertex.id & 0x1 ) );
		result.color = readColorFromPalette( dc.miscIntegers.y );
	}
	else if( vaaType == 0x20000 || vaaType == 0x30000 )	// eVaaKind.StrokedFat or eVaaKind.StrokedThin
	{
		// Stroked mesh with VAA. Can be eDrawCall.SolidColor or eDrawCall.ThinLine
		vaaValue = float( int( vertex.id & 0x3 ) - 1 );
		if( vaaType == 0x20000 )	// eVaaKind.StrokedFat
		{
			vaaValue *= dc.translationAndVaa.w;
			result.color = readColorFromPalette( dc.miscIntegers.y );
		}
		else
		{
			// Gotta be eVaaKind.StrokedThin
			result.color = unpackFp16( dc.miscIntegers.yz );
		}
	}
	else
	{
		// The rest of them use color indices in miscIntegers.y
		result.color = readColorFromPalette( dc.miscIntegers.y );
	}

#if TEXTURE_ATLAS || TEXT_RENDERING
	result.textureCoordinates = float3( vaaValue, 0.0, 0.0 );
#else
	result.vaa = vaaValue;
#endif
#else
	result.color = readColorFromPalette( dc.miscIntegers.y );
#endif	// TEXTURE_ATLAS || TEXT_RENDERING || !OPAQUE_PASS

#if TEXT_RENDERING
	result.backgroundColor = result.color;
#endif
}