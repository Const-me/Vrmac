using Diligent.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw
{
	/// <summary>We can reasonably expect users won't be painting with millions of unique brushes.
	/// To save bandwidth, keep used unique colors in a floating point texture in VRAM.</summary>
	class PaletteTexture: IDisposable
	{
		public void Dispose()
		{
			ComUtils.clear( ref m_textureView );
			ComUtils.clear( ref texture );
		}

		ITexture texture;
		ITextureView m_textureView;
		public ITextureView textureView => m_textureView;

		readonly Dictionary<ulong, int> colorIndices = new Dictionary<ulong, int>( 0x100 );
		readonly Context context;
		int writtenColorsCount = 0;
		int textureRows = 0;

		static TextureDesc textureDesc( int rows )
		{
			TextureDesc desc = new TextureDesc( false );
			desc.Type = ResourceDimension.Tex2d;
			desc.Size = new CSize( 0x100, rows );
			desc.Format = TextureFormat.Rgba16Float;
			desc.BindFlags = BindFlags.ShaderResource;
			return desc;
		}

		public const int transparentIndex = 0;
		public const int blackIndex = 1;
		public const int whiteIndex = 2;

		public PaletteTexture( Context context )
		{
			this.context = context;
			var desc = textureDesc( 1 );
			using( var dev = context.device )
				texture = dev.CreateTexture( ref desc, "Palette" );
			m_textureView = texture.GetDefaultView( TextureViewType.ShaderResource );
			textureRows = 1;

			// Add just a few hardcoded colors, with names matching eNamedColor enum
			Palette.PredefinedPaletteEntries.initPalette( colorIndices );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int color( ref Vector4 color )
		{
			ulong fp16 = GraphicsUtils.fp16( ref color );
			if( colorIndices.TryGetValue( fp16, out int index ) )
				return index;
			index = colorIndices.Count;
			colorIndices.Add( fp16, index );
			return index;
		}

		static readonly ushort nullThreshold, opaqueThreshold;

		static PaletteTexture()
		{
			Vector4 thresholds = new Vector4( 1.0f / 255.0f, 254.0f / 255.0f, 0, 0 );
			ulong fp16 = GraphicsUtils.fp16( ref thresholds );
			checked
			{
				nullThreshold = (ushort)( fp16 & 0xFFFF );
				opaqueThreshold = (ushort)( fp16 >> 16 );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public SolidColorData colorData( ref Vector4 color )
		{
			ulong fp16 = GraphicsUtils.fp16( ref color );

			eBrushType brushType;
			ushort alpha = (ushort)( fp16 >> 48 );
			if( alpha < nullThreshold )
				brushType = eBrushType.Null;
			else if( alpha < opaqueThreshold )
				brushType = eBrushType.Transparent;
			else
				brushType = eBrushType.Opaque;

			if( colorIndices.TryGetValue( fp16, out int index ) )
				return new SolidColorData( index, brushType );

			index = colorIndices.Count;
			colorIndices.Add( fp16, index );
			return new SolidColorData( index, brushType );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void update()
		{
			if( writtenColorsCount == colorIndices.Count )
				return;
			updateTexture();
		}

		public WeakEvent<Action> textureResized { get; } = new WeakEvent<Action>();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void gatherColorValues( int first, Span<ulong> values )
		{
			foreach( var kvp in colorIndices )
			{
				int index = kvp.Value;
				if( index < first )
					continue;
				index -= first;
				if( index < values.Length )
					values[ index ] = kvp.Key;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void updateRange( int begin, int end )
		{
			int y = begin >> 8;
			Debug.Assert( y == ( end - 1 ) >> 8 );

			int newEntries = end - begin;
			Span<ulong> values = stackalloc ulong[ newEntries ];
			gatherColorValues( begin, values );
			CRect updateBox = new CRect( begin & 0xFF, y, end & 0xFF, y + 1 );
			context.context.updateTexture<ulong>( texture, values, ref updateBox );
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		void updateTexture()
		{
			int newLength = colorIndices.Count;
			if( newLength <= ( textureRows << 8 ) )
			{
				// Still writing the same row
				updateRange( writtenColorsCount, colorIndices.Count );
				writtenColorsCount = colorIndices.Count;
				return;
			}

			// Filled a row, resize the texture
			int capacity = textureRows << 8;
			if( writtenColorsCount != capacity )
			{
				updateRange( writtenColorsCount, capacity );
				writtenColorsCount = capacity;
			}

			int newHeight = ( newLength >> 8 ) + 1;
			ITexture newTexture;
			using( var dev = context.device )
			{
				var desc = textureDesc( newHeight );
				newTexture = dev.CreateTexture( ref desc, "Palette" );
			}

			context.context.copyTexture( newTexture, texture );
			texture.Dispose();
			m_textureView.Dispose();
			texture = newTexture;
			m_textureView = newTexture.GetDefaultView( TextureViewType.ShaderResource );

			while( newLength >= writtenColorsCount + 0x100 )
			{
				int lineEnd = writtenColorsCount + 0x100;
				updateRange( writtenColorsCount, lineEnd );
				writtenColorsCount = lineEnd;
			}

			updateRange( writtenColorsCount, newLength );
			writtenColorsCount = newLength;

			foreach( var sub in textureResized )
				sub();
		}

		readonly Dictionary<int, ulong> colorValues = new Dictionary<int, ulong>();

		/// <summary>Do the reverse lookup, from index to FP16 color.</summary>
		/// <remarks>Only used for thin lines so we need just a couple of colors, if any at all. For performance reasons using much smaller hash map just for the requested indices.</remarks>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ulong colorValue( int index )
		{
			if( colorValues.TryGetValue( index, out var color ) )
				return color;
			return findColor( index );
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		ulong findColor( int index )
		{
			if( index >= colorIndices.Count )
				throw new ArgumentOutOfRangeException();

			foreach( var kvp in colorIndices )
			{
				if( kvp.Value != index )
					continue;
				colorValues.Add( index, kvp.Key );
				return kvp.Key;
			}
			throw new ApplicationException();
		}

		public int colorsCount => colorIndices.Count;
	}
}