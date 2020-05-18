#ifdef HLSL
#define INLINE inline
#else
#define INLINE
#endif

struct sVertexOutput
{
	float4 position : SV_Position;
	float4 color: TEXCOORD0;
#if TEXTURE_ATLAS || TEXT_RENDERING
	float3 textureCoordinates : TEXCOORD1;
#if TEXT_RENDERING
	float4 backgroundColor: TEXCOORD2;
#endif

#elif !OPAQUE_PASS
	float vaa: TEXCOORD1;
#endif
	uint id: BLENDINDICES;
};

INLINE uint vaaFromId( uint id )
{
	return ( id & 0xFF0000 );
}
INLINE uint meshTypeFromId( uint id )
{
	return ( id & 0xFF00 );
}
INLINE uint brushFromId( uint id )
{
	return ( id & 0xFF );
}

INLINE float getDefaultVaaFactor()
{
	return 1.4142135623730950488016887242097;	// sqrt( 2 )
}

INLINE float4 greenColor()
{
	return float4( 0.0, 1.0, 0.0, 1.0 );
}
INLINE float4 blueColor()
{
	return float4( 0.0, 0.0, 1.0, 1.0 );
}

#define F20 float2( 0.0, 0.0 )
#define F30 float3( 0.0, 0.0, 0.0 )
#define F40 float4( 0.0, 0.0, 0.0, 0.0 )

#if !OPAQUE_PASS || TEXT_RENDERING
cbuffer StaticConstantsBuffer
{
	// XY = size of pixel in clip space units. Z = DPI scaling multiplier, W = 1.0 / Z
	float4 pixelSizeAndDpiScaling;
#if TEXT_RENDERING
	// XY = size of the grayscale atlas, ZW = size of the ClearType atlas
	float4 fontAtlasSize;
#endif
};
#endif

Texture2D<float4> paletteTexture;

INLINE float4 readColorFromPalette( uint index )
{
	int3 loadLoc;
	// Width is hardcoded 0x100 for that one
	loadLoc.x = int( index & 0xFF );
	loadLoc.y = int( index >> 8 );
	loadLoc.z = 0;
	return paletteTexture.Load( loadLoc );
}

// Unpack 4-lanes FP vector stored as 32-bit integers
INLINE float4 unpackFp16( uint2 packedFp16 )
{
#if PLATFORM_LINUX
	// OpenGL ES Shading Language 3.1, section 8.4 "Floating-Point Pack and Unpack Functions" on page 114
	float2 vect1 = unpackHalf2x16( packedFp16.x );
	float2 vect2 = unpackHalf2x16( packedFp16.y );
	return float4( vect1, vect2 );
#else
	// https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/f16tof32
	const uint2 high = packedFp16 >> 16;
	const uint4 allValues = uint4( packedFp16.x, high.x, packedFp16.y, high.y );
	return f16tof32( allValues );
#endif
}