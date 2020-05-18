#include "drawUtils.hlsli"
#if TEXTURE_ATLAS
Texture2DArray textureAtlas;
SamplerState   textureAtlas_sampler;
#endif
#if TEXT_RENDERING
Texture2DArray<float> grayscaleFontAtlas;
SamplerState grayscaleFontAtlas_sampler;
Texture2DArray<float4> cleartypeFontAtlas;
#endif

INLINE float interpolate( float x1, float x2, float y1, float y2, float x )
{
	return ( x * y1 - x * y2 + x1 * y2 - x2 * y1 ) / ( x1 - x2 );	// solve((x-x1)/(x2-x1)=(y-y1)/(y2-y1), y);
}

INLINE float derivativeLength( float func )
{
	return length( float2( ddx( func ), ddy( func ) ) );
}

INLINE float derivativeAbs( float func )
{
	return abs( ddx( func ) ) + abs( ddy( func ) );
}

float4 main( in sVertexOutput inputVertex ) : SV_Target
{
	uint id = inputVertex.id;
	float4 color = inputVertex.color;

#if TEXTURE_ATLAS
	if( brushFromId( id ) == 0x2 )	// eBrush.Sprite
	{
		color *= textureAtlas.Sample( textureAtlas_sampler, inputVertex.textureCoordinates );
		// float4 dbg = float4( inputVertex.uv.z, inputVertex.uv.x, inputVertex.uv.y, 1.0 );
		// color = lerp( color, dbg, 0.75 );
	}
#endif

#if TEXT_RENDERING
	uint meshType = meshTypeFromId( id );
	if( meshType == 0x500 )	// eMesh.GlyphRun
	{
		// Not using samplers, instead loading exact pixel colors by their integer coordinates.
		// FreeType does AA already, not as great as VAA, but still.
		float2 uv = inputVertex.textureCoordinates.xy;
		int4 readLocation;
		readLocation.z = int( inputVertex.textureCoordinates.z );
		readLocation.w = 0;

		uint clearType = vaaFromId( id );
		if( 0x0 == clearType )
		{
			// No ClearType, just grayscale AA for pixel-snapped text
			readLocation.xy = int2( uv * fontAtlasSize.xy );
			float glyphValue = grayscaleFontAtlas.Load( readLocation );
			return lerp( inputVertex.backgroundColor, color, glyphValue );
		}
		else
		{
			// ClearType text. Note we use different texture + size multiplier
			readLocation.xy = int2( uv * fontAtlasSize.zw );
			float4 glyphValueRgba = cleartypeFontAtlas.Load( readLocation );

			if( glyphValueRgba.w == 0.0 )
			{
				// The C++ code which copies glyphs from FreeType bitmaps into the atlas computes alpha as maximum of 3 channels.
				return F40;
			}

			float3 glyphValueRgb;
			if( 0x20000 == clearType )
			{
				// eClearTypeKind.Flipped
				glyphValueRgb = glyphValueRgba.zyx;
			}
			else
			{
				// Gotta be eClearTypeKind.Straight
				glyphValueRgb = glyphValueRgba.xyz;
			}

			// return float4( glyphValueRgb, 1.0 );
			float3 backgroundRgb = float3( 1.0, 1.0, 1.0 ) - glyphValueRgb;

			float3 result = color.xyz * glyphValueRgb + inputVertex.backgroundColor.xyz * backgroundRgb;
			return float4( result, 1.0 );
		}
	}

	if( meshType == 0x600 )	// eMesh.TransformedText
	{
		// For transformed text however, we do want that bilinear sampler. The grayscale atlas inserts padding around glyphs just for that.
		// Ideally, need different set of textures for this case, with a good set of mipmaps, signed distance field in them, and maybe even anisotropic sampler.
		float2 uv = inputVertex.textureCoordinates.xy;
		float glyphValue = grayscaleFontAtlas.Sample( grayscaleFontAtlas_sampler, inputVertex.textureCoordinates );
		return lerp( inputVertex.backgroundColor, color, glyphValue );
	}
#endif

#if OPAQUE_PASS
	return color;
#else
	uint vaaType = vaaFromId( id );
	if( 0x0 == vaaType )
		return color;

#if TEXTURE_ATLAS || TEXT_RENDERING
	float uv = inputVertex.textureCoordinates.x;
#else
	float uv = inputVertex.vaa;
#endif

	float aa = 0.0;
	if( 0x10000 == vaaType )
	{
		// eVaaKind.Filled = 1
		// VAA works differently for filled meshes.
		float edgeDistance = uv;
		float thresholdWidth = derivativeAbs( edgeDistance );
		// GPUs don't like branches with conditions changing within same triangle. The expression below does the right thing, e.g. if thresholdWidth is 0, the saturate will clip +INF to 1.0.
		aa = saturate( edgeDistance / thresholdWidth );

		// Debug code below: visualize VAA output, and transparent pass edge triangles
		// return lerp( float4( 0, 0, 1, 1 ), float4( 0, 1, 0, 1 ), aa );
	}
	else if( 0x20000 == vaaType )
	{
		// eVaaKind.StrokedFat = 2
		// return dbgRenderUV( inputVertex.uv );
		float normLength = abs( uv );
		float edgeDistanceFunc = 1.0 - normLength;
		float VaaFactor = getDefaultVaaFactor();
		float thresholdWidth = VaaFactor * derivativeLength( edgeDistanceFunc );
		aa = saturate( ( edgeDistanceFunc / thresholdWidth ) + 0.5 );
	}
	else if( 0x30000 == vaaType )
	{
		// eVaaKind.StrokedThin = 3
		float normLength = abs( uv );
		aa = 1.0 - saturate( normLength );
	}
	return color * aa;
#endif
}