using Diligent.Graphics;
using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Vrmac;
using Vrmac.Animation;

namespace RenderSamples
{
	class Tutorial02_Cube: SampleBase, iDeltaTimeUpdate
	{
		IPipelineState pipelineState;
		IShaderResourceBinding resourceBinding;
		IBuffer cubeVertexBuffer, cubeIndexBuffer, vsConstants;
		Matrix4x4 worldViewProjMatrix;

		void createPipelineState( IRenderDevice device )
		{
			PipelineStateDesc PSODesc = new PipelineStateDesc( false );
			PSODesc.setBufferFormats( context );

			// Primitive topology defines what kind of primitives will be rendered by this pipeline state
			PSODesc.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleList;
			// Cull back faces
			PSODesc.GraphicsPipeline.RasterizerDesc.CullMode = CullMode.Back;
			// Enable depth testing
			PSODesc.GraphicsPipeline.DepthStencilDesc.DepthEnable = true;

			iShaderFactory shaderFactory = device.GetShaderFactory();
			// We won't be using the device object after this, `using` is to release the COM interface once finished
			using( iPipelineStateFactory stateFactory = device.CreatePipelineStateFactory() )
			{
				stateFactory.setName( "Cube PSO" );

				// Compile the two shaders
				ShaderSourceInfo sourceInfo = new ShaderSourceInfo( ShaderType.Vertex, ShaderSourceLanguage.Hlsl );
				sourceInfo.combinedTextureSamplers = true;  // This appears to be the requirement of OpenGL backend.

				// In this tutorial, we will load shaders from resources embedded into this .NET DLL.
				iStorageFolder resources = StorageFolder.embeddedResources( Assembly.GetExecutingAssembly(), "RenderSamples/02-Cube" );
				var vs = shaderFactory.compileFromFile( resources, "cube.vsh", sourceInfo, "Cube VS" );
				stateFactory.graphicsVertexShader( vs );

				// Create dynamic uniform buffer that will store our transformation matrix. Dynamic buffers can be frequently updated by the CPU.
				BufferDesc CBDesc = new BufferDesc( false );
				CBDesc.uiSizeInBytes = Marshal.SizeOf<Matrix4x4>();
				CBDesc.Usage = Usage.Dynamic;
				CBDesc.BindFlags = BindFlags.UniformBuffer;
				CBDesc.CPUAccessFlags = CpuAccessFlags.Write;
				vsConstants = device.CreateBuffer( CBDesc, "VS constants CB" );

				// Create a pixel shader
				sourceInfo.shaderType = ShaderType.Pixel;

				var ps = shaderFactory.compileFromFile( resources, "cube.psh", sourceInfo, "Cube PS" );
				stateFactory.graphicsPixelShader( ps );

				// Define vertex shader input layout

				// Attribute 0 - vertex position
				LayoutElement elt = new LayoutElement( false )
				{
					InputIndex = 0,
					BufferSlot = 0,
					NumComponents = 3,
					ValueType = GpuValueType.Float32,
					IsNormalized = false
				};
				stateFactory.graphicsLayoutElement( elt );
				// Attribute 1 - vertex color
				elt.InputIndex = 1;
				elt.NumComponents = 4;
				stateFactory.graphicsLayoutElement( elt );

				// Define variable type that will be used by default
				PSODesc.ResourceLayout.DefaultVariableType = ShaderResourceVariableType.Static;

				stateFactory.apply( ref PSODesc );
				pipelineState = device.CreatePipelineState( ref PSODesc );
			}

			// Since we did not explicitly specify the type for 'Constants' variable, default
			// type (SHADER_RESOURCE_VARIABLE_TYPE_STATIC) will be used. Static variables never
			// change and are bound directly through the pipeline state object.
			pipelineState.GetStaticVariableByName( ShaderType.Vertex, "Constants" ).Set( vsConstants );

			// Create a shader resource binding object and bind all static resources in it
			resourceBinding = pipelineState.CreateShaderResourceBinding( true );
		}

		[StructLayout( LayoutKind.Sequential, Pack = 4 )]
		struct Vertex
		{
			public Vector3 pos;
			public Vector4 color;
		}

		static Vector3 v3( float x, float y, float z )
		{
			return new Vector3( x, y, z );
		}
		static Vector4 v4( float x, float y, float z, float w )
		{
			return new Vector4( x, y, z, w );
		}
		static Vertex vert( Vector3 p, Vector4 c )
		{
			return new Vertex()
			{
				pos = p,
				color = c
			};
		}

		void createVertexBuffer( IRenderDevice device )
		{
			// Cube vertices

			//      (-1,+1,+1)________________(+1,+1,+1)
			//               /|              /|
			//              / |             / |
			//             /  |            /  |
			//            /   |           /   |
			//(-1,-1,+1) /____|__________/(+1,-1,+1)
			//           |    |__________|____|
			//           |   /(-1,+1,-1) |    /(+1,+1,-1)
			//           |  /            |   /
			//           | /             |  /
			//           |/              | /
			//           /_______________|/
			//        (-1,-1,-1)       (+1,-1,-1)
			//

			// clang-format off
			Vertex[] CubeVerts = new Vertex[ 8 ]
			{
				vert( v3(-1,-1,-1),  v4(1,0,0,1) ),
				vert( v3(-1,+1,-1),  v4(0,1,0,1) ),
				vert( v3( +1,+1,-1), v4( 0,0,1,1) ),
				vert( v3(+1,-1,-1),  v4(1,1,1,1) ),

				vert( v3(-1,-1,+1),  v4(1,1,0,1) ),
				vert( v3(-1,+1,+1),  v4(0,1,1,1) ),
				vert( v3(+1,+1,+1),  v4(1,0,1,1) ),
				vert( v3(+1,-1,+1),  v4(0.2f,0.2f,0.2f,1) ),
			};
			BufferDesc VertBuffDesc = new BufferDesc( false )
			{
				Usage = Usage.Static,
				BindFlags = BindFlags.VertexBuffer,
			};
			cubeVertexBuffer = device.CreateBuffer( VertBuffDesc, CubeVerts, "Cube vertex buffer" );
		}

		void createIndexBuffer( IRenderDevice device )
		{
			// clang-format off
			uint[] Indices = new uint[]
			{
				2,0,1, 2,3,0,
				4,6,5, 4,7,6,
				0,7,4, 0,3,7,
				1,0,4, 1,4,5,
				1,5,2, 5,6,2,
				3,6,7, 3,2,6
			};
			// clang-format on

			BufferDesc IndBuffDesc = new BufferDesc( false )
			{
				Usage = Usage.Static,
				BindFlags = BindFlags.IndexBuffer
			};
			cubeIndexBuffer = device.CreateBuffer( IndBuffDesc, Indices, "Cube index buffer" );
		}

		protected override void createResources( IRenderDevice device )
		{
			createPipelineState( device );
			createVertexBuffer( device );
			createIndexBuffer( device );
			context.animation.startDelta( this );
		}

		static readonly Vector4 clearColor = Color.parse( "black" );

		void renderScene( IDeviceContext ic, ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			ic.SetRenderTarget( swapChainRgb, swapChainDepthStencil );

			// Clear the back buffer
			ic.ClearRenderTarget( swapChainRgb, clearColor );
			ic.ClearDepthStencil( swapChainDepthStencil, ClearDepthStencilFlags.DepthFlag, 1.0f, 0 );

			// Map the buffer and write current world-view-projection matrix
			Matrix4x4 transposed = worldViewProjMatrix.transposed();
			ic.writeBuffer( vsConstants, ref transposed );

			// Bind vertex and index buffers
			ic.SetVertexBuffer( 0, cubeVertexBuffer, 0, ResourceStateTransitionMode.Transition, SetVertexBuffersFlags.Reset );
			ic.SetIndexBuffer( cubeIndexBuffer, 0 );

			// Set the pipeline state
			ic.SetPipelineState( pipelineState );
			// Commit shader resources. RESOURCE_STATE_TRANSITION_MODE_TRANSITION mode makes sure that resources are transitioned to required states.
			ic.CommitShaderResources( resourceBinding );

			DrawIndexedAttribs draw = new DrawIndexedAttribs( false )
			{
				IndexType = GpuValueType.Uint32,
				NumIndices = 36,
				Flags = DrawFlags.VerifyAll
			};
			ic.DrawIndexed( ref draw );
		}

		static readonly TimeSpan cycleDuration = TimeSpan.FromSeconds( 3 );
		static readonly float velocity = (float)( MathF.PI * 2.0f / cycleDuration.TotalSeconds );

		Angle angle;

		void iDeltaTimeUpdate.tick( float elapsedSeconds )
		{
			angle.rotate( velocity, elapsedSeconds );

			// Set cube world view matrix
			Matrix4x4 CubeWorldView = Matrix4x4.CreateRotationY( angle )
				* Matrix4x4.CreateRotationX( MathF.PI * -0.1f )
				* Matrix4x4.CreateTranslation( 0, 0, 5 );

			float NearPlane = 0.1f;
			float FarPlane = 100;
			// Projection matrix differs between DX and OpenGL
			Matrix4x4 Proj = DiligentMatrices.createPerspectiveFieldOfView( 0.25f * MathF.PI, context.aspectRatio, NearPlane, FarPlane, isOpenGlDevice );
			worldViewProjMatrix = CubeWorldView * Proj;
		}

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			printFps();
			renderScene( context.context, swapChainRgb, swapChainDepthStencil );
		}
	}
}