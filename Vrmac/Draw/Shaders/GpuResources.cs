using Diligent.Graphics;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Vrmac.Draw.Main;

namespace Vrmac.Draw.Shaders
{
	sealed class GpuResources: IDisposable
	{
		public readonly Context context;

		readonly DynamicBuffer m_vertexBuffer;
		public IBuffer vertexBuffer => m_vertexBuffer.buffer;

		readonly DynamicBuffer m_indexBuffer;
		public IBuffer indexBuffer => m_indexBuffer.buffer;

		IBuffer m_fewDrawCalls;
		IBuffer m_staticCBuffer;

		public PaletteTexture paletteTexture { get; }

		static int kb( int count )
		{
			return count << 10;
		}
		public GpuResources( Context context, PaletteTexture palette )
		{
			this.context = context;
			m_vertexBuffer = new DynamicBuffer( context, BindFlags.VertexBuffer, kb( 256 ) );
			m_indexBuffer = new DynamicBuffer( context, BindFlags.IndexBuffer, kb( 32 ) );
			createStaticCBuffer( context.swapChainSize, context.dpiScalingFactor );
			context.swapChainResized.add( this, onSwapChainResized );
			paletteTexture = palette;
		}

		[StructLayout( LayoutKind.Sequential )]
		struct StaticConstantsBuffer
		{
			public Vector4 pixelSizeAndDpiScaling;
		}

		public Vector4 pixelSizeAndDpiScaling { get; private set; }

		void createStaticCBuffer( CSize size, double dpiScaling )
		{
			var data = new StaticConstantsBuffer();
			data.pixelSizeAndDpiScaling.X = 2.0f / size.cx;
			data.pixelSizeAndDpiScaling.Y = 2.0f / size.cy;
			data.pixelSizeAndDpiScaling.Z = (float)dpiScaling;
			data.pixelSizeAndDpiScaling.W = (float)( 1.0 / dpiScaling );

			pixelSizeAndDpiScaling = data.pixelSizeAndDpiScaling;

			BufferDesc desc = new BufferDesc( false )
			{
				uiSizeInBytes = 16,
				BindFlags = BindFlags.UniformBuffer,
				Usage = Usage.Static,
			};

			ComUtils.clear( ref m_staticCBuffer );
			using( var device = context.renderContext.device )
				m_staticCBuffer = device.CreateBuffer( desc, ref data, "StaticConstantsBuffer" );
		}

		void onSwapChainResized( CSize newSize, double dpiScaling )
		{
			createStaticCBuffer( newSize, dpiScaling );
			ComUtils.clear( ref m_staticCBufferForText );
		}

		public void Dispose()
		{
			ComUtils.clear( ref m_staticCBuffer );
			ComUtils.clear( ref m_staticCBufferForText );
			ComUtils.clear( ref m_fewDrawCalls );
			m_indexBuffer?.Dispose();
			m_vertexBuffer?.Dispose();
		}

		public IBuffer staticCBuffer => m_staticCBuffer;

		IBuffer getFewDrawCalls()
		{
			if( null != m_fewDrawCalls )
				return m_fewDrawCalls;
			using( var dev = context.renderContext.device )
				m_fewDrawCalls = dev.CreateDynamicUniformBuffer( FewDrawCallsState.smallCount * FewDrawCallsState.drawCallSize );
			return m_fewDrawCalls;
		}

		public IBuffer fewDrawCalls => getFewDrawCalls();

		DynamicBuffer moarDrawCalls()
		{
			if( null != m_moarDrawCalls )
				return m_moarDrawCalls;

			// Pi4 doesn't support buffer reads from vertex shaders: https://www.raspberrypi.org/forums/viewtopic.php?f=68&t=271863&p=1648066#p1648066
			// It does supports texelFetch, but that one doesn't have the precision: https://www.raspberrypi.org/forums/viewtopic.php?f=68&t=266652 and we need all these juicy 32 bits for transformation matrices.
			// For this reason, on Linux that buffer is uniform.
			// Not required on Windows, D3D can keep gigabytes in these buffers and read them everywhere.

			BindFlags bindFlags = RuntimeEnvironment.runningWindows ? BindFlags.ShaderResource : BindFlags.UniformBuffer;
			// Set up initial capacity for 170 draw calls. Will grow larger as needed, it's dynamic.
			int initialCapacity = 512 * 16;
			m_moarDrawCalls = new DynamicBuffer( context, bindFlags, initialCapacity );
			return m_moarDrawCalls;
		}

		DynamicBuffer m_moarDrawCalls;
		public DynamicBuffer moreDrawCalls => moarDrawCalls();

		public DynamicBuffer getVertexBuffer() => m_vertexBuffer;
		public DynamicBuffer getIndexBuffer() => m_indexBuffer;

		public void initTextCBuffer()
		{
			DrawDevice dev = (DrawDevice)context.drawDevice;
			Action actClear = () =>
			{
				GpuResources res = this;
				ComUtils.clear( ref res.m_staticCBufferForText );
			};
			dev.fontTextures.subscriveResized( this, actClear );
		}

		[StructLayout( LayoutKind.Sequential )]
		struct ConstantsBufferText
		{
			public Vector4 pixelSizeAndDpiScaling;
			public Vector4 textureAtlasSize;
		}
		IBuffer m_staticCBufferForText;

		public IBuffer staticCBufferForText => getTextCBuffer();

		IBuffer getTextCBuffer()
		{
			if( null != m_staticCBufferForText )
				return m_staticCBufferForText;

			ConstantsBufferText data = new ConstantsBufferText();
			data.pixelSizeAndDpiScaling = pixelSizeAndDpiScaling;

			DrawDevice dev = (DrawDevice)context.drawDevice;
			CSize atlasSize = dev.fontTextures.grayscale.layerSize;
			data.textureAtlasSize.X = atlasSize.cx;
			data.textureAtlasSize.Y = atlasSize.cy;

			atlasSize = dev.fontTextures.cleartype.layerSize;
			data.textureAtlasSize.Z = atlasSize.cx;
			data.textureAtlasSize.W = atlasSize.cy;

			IBuffer ib;
			using( var device = context.renderContext.device )
				ib = device.CreateImmutableUniformBuffer( ref data, "Text cbuffer" );
			m_staticCBufferForText = ib;
			return ib;
		}

		public ITextureView grayscaleFontAtlas()
		{
			DrawDevice dev = (DrawDevice)context.drawDevice;
			return dev.fontTextures.grayscale.nativeTexture;
		}

		public ITextureView clearTypeFontAtlas()
		{
			DrawDevice dev = (DrawDevice)context.drawDevice;
			return dev.fontTextures.cleartype.nativeTexture;
		}
	}
}