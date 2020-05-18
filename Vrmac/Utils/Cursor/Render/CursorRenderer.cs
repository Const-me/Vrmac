using Diligent.Graphics;
using System;
using System.Reflection;

namespace Vrmac.Utils.Cursor.Render
{
	/// <summary>Implements rendering of mouse cursors</summary>
	sealed class CursorRenderer
	{
		public readonly Context context;
		readonly IBuffer vertexBuffer;

		iCursorRender currentCursor = null;
		sCursorPosition cursorPosition;

		StaticCursor staticCursor;
		AnimatedCursor animatedCursor;
		MonoCursor monochromeCursor;

		static readonly string resourceFolder = typeof( CursorRenderer ).Namespace;

		/// <summary>Construct the object and create the required GPU resources</summary>
		public CursorRenderer( Context context, IRenderDevice device )
		{
			this.context = context;
			vertexBuffer = createVertexBuffer( device );

			iShaderFactory shaderFactory = device.GetShaderFactory();
			iStorageFolder assets = StorageFolder.embeddedResources( Assembly.GetExecutingAssembly(), resourceFolder );
			IShader vs = shaderFactory.compileHlslFile( assets, "CursorVS.hlsl", ShaderType.Vertex );
			using( iPipelineStateFactory stateFactory = device.CreatePipelineStateFactory() )
			{
				staticCursor = new StaticCursor( context, device, stateFactory, shaderFactory, assets, vs );
				animatedCursor = new AnimatedCursor( context, device, stateFactory, shaderFactory, assets );
				monochromeCursor = new MonoCursor( context, device, stateFactory, shaderFactory, assets, vs );
			}

			cursorPosition.setWindowSize( context.swapChainSize );

			// Subscribe to the resized event
			context.swapChainResized.add( this, onSwapChainResized );
		}

		static IBuffer createVertexBuffer( IRenderDevice device )
		{
			Vector2[] vertices = new Vector2[ 4 ]
			{
				new Vector2( 1, 0 ),
				new Vector2( 0, 0 ),
				new Vector2( 1, 1 ),
				new Vector2( 0, 1 ),
			};
			BufferDesc VertBuffDesc = new BufferDesc( false )
			{
				Usage = Usage.Static,
				BindFlags = BindFlags.VertexBuffer,
			};
			return device.CreateBuffer( VertBuffDesc, vertices, "Cursor vertex buffer" );
		}

		CursorTexture m_texture;
		CPoint? m_position;

		void onSwapChainResized( CSize newSize, double dpiScaling )
		{
			cursorPosition.setWindowSize( newSize );
		}

		/// <summary>Get or set the cursor</summary>
		public CursorTexture cursor
		{
			get
			{
				return m_texture;
			}
			set
			{
				m_texture = value;
				if( null == value )
				{
					currentCursor = null;
					return;
				}

				cursorPosition.setTexture( value );

				switch( value )
				{
					case AnimatedCursorTexture ani:
						animatedCursor.updateTexture( ani );
						currentCursor = animatedCursor;
						break;
					case StaticCursorTexture sct:
						staticCursor.updateTexture( sct );
						currentCursor = staticCursor;
						break;
					case MonochromeCursorTexture mct:
						monochromeCursor.updateTexture( mct );
						currentCursor = monochromeCursor;
						break;
					default:
						throw new NotImplementedException();
				}

				if( m_position.HasValue )
				{
					Vector4 v4 = cursorPosition.updatePosition( m_position.Value );
					currentCursor.updatePosition( ref v4 );
				}
			}
		}

		/// <summary>Cursor position. Set this to null to hide the pointer.</summary>
		public CPoint? position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
				if( value.HasValue )
				{
					Vector4 v4 = cursorPosition.updatePosition( value.Value );
					currentCursor?.updatePosition( ref v4 );
				}
			}
		}

		/// <summary>True if the cursor is visible.</summary>
		public bool visible => null != m_texture && m_position.HasValue;

		/// <summary>Render the cursor, if it's visible.</summary>
		public void render()
		{
			if( m_position.HasValue )
				currentCursor?.render( context.context, vertexBuffer );
		}
	}
}