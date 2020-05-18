using ComLight;
using System;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>GraphicsPipelineDesc C++ structure has too many native pointers in the fields. Need a COM-friendly class to allocate them in native memory.</summary>
	[ComInterface( "b43917f7-6f97-4266-8cf0-5825baf3681c", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface iPipelineStateFactory: IDisposable
	{
		/// <summary>Clear everything set, this way you can reuse the object. Helps with the performance if you're creating many of them.</summary>
		void clear();
		/// <summary>Copy all set native pointers into the native C++ structure</summary>
		void apply( ref PipelineStateDesc desc );

		/// <summary>Set the name of the pipeline state</summary>
		void setName( [MarshalAs( UnmanagedType.LPUTF8Str )] string name );

		/// <summary>Append ShaderResourceVariableDesc to PipelineResourceLayoutDesc.Variables vector</summary>
		/// <param name="stages">Shader stages this resources variable applies to. More than one shader stage can be specified.</param>
		/// <param name="name">Shader variable name</param>
		/// <param name="variableType">Shader variable type. <see cref="ShaderResourceVariableType"/> for a list of allowed types</param>
		void layoutVariable( ShaderType stages, ShaderResourceVariableType variableType = ShaderResourceVariableType.Static, [MarshalAs( UnmanagedType.LPUTF8Str )] string name = null );

		/// <summary>Append StaticSamplerDesc to PipelineResourceLayoutDesc.StaticSamplers vector</summary>
		/// <param name="stages">Shader stages that this static sampler applies to. More than one shader stage can be specified.</param>
		/// <param name="desc">The name of the sampler itself or the name of the texture variable that this static sampler is assigned to if combined texture samplers are used.</param>
		/// <param name="samplerOrTextureName"></param>
		void layoutStaticSampler( ShaderType stages, [In] ref SamplerDesc desc, [MarshalAs( UnmanagedType.LPUTF8Str )] string samplerOrTextureName = null );

		// GraphicsPipelineDesc data

		/// <summary>Set the vertex shader to use in the pipeline state that will be created</summary>
		void graphicsVertexShader( IShader shader );
		/// <summary>Set the pixel shader to use in the pipeline state that will be created</summary>
		void graphicsPixelShader( IShader shader );
		/// <summary>Set the domain shader to use in the pipeline state that will be created</summary>
		void graphicsDomainShader( IShader shader );
		/// <summary>Set the hull shader to use in the pipeline state that will be created</summary>
		void graphicsHullShader( IShader shader );
		/// <summary>Set the geometry shader to use in the pipeline state that will be created</summary>
		void graphicsGeometryShader( IShader shader );

		/// <summary>Append an element to input layout vector.</summary>
		void graphicsLayoutElement( [MarshalAs( UnmanagedType.LPUTF8Str )] string HLSLSemantic, [In] ref LayoutElement layoutElement );

		/// <summary>Set the compute shader to use in the pipeline state that will be created</summary>
		void computeShader( IShader shader );
	}

	/// <summary>Couple extension methods for <see cref="iPipelineStateFactory" /> interface.</summary>
	public static class PipelineStateFactoryExt
	{
		/// <summary>Append an element to input layout vector, the semantic name will be "ATTRIB".</summary>
		/// <remarks>Default value ("ATTRIB") allows HLSL shaders to be converted to GLSL and used in OpenGL backend as well as compiled to SPIRV and used in Vulkan backend.
		/// Any value other than default will only work in Direct3D11 and Direct3D12 backends.</remarks>
		public static void graphicsLayoutElement( this iPipelineStateFactory psf, LayoutElement elt )
		{
			psf.graphicsLayoutElement( null, ref elt );
		}
	}
}