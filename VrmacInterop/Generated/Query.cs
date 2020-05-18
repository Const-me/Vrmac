// This source file was automatically generated from "Query.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>Query type.</summary>
	public enum QueryType
	{
		/// <summary>Query type is undefined.</summary>
		Undefined = 0,
		/// <summary>
		/// <para>Gets the number of samples that passed the depth and stencil tests in between IDeviceContext::BeginQuery</para>
		/// <para>and IDeviceContext::EndQuery. IQuery::GetData fills a Diligent::QueryDataOcclusion struct.</para>
		/// </summary>
		Occlusion = 1,
		/// <summary>
		/// <para>Acts like QUERY_TYPE_OCCLUSION except that it returns simply a binary true/false result: false indicates that no samples</para>
		/// <para>passed depth and stencil testing, true indicates that at least one sample passed depth and stencil testing.</para>
		/// <para>IQuery::GetData fills a Diligent::QueryDataBinaryOcclusion struct.</para>
		/// </summary>
		BinaryOcclusion = 2,
		/// <summary>
		/// <para>Gets the GPU timestamp corresponding to IDeviceContext::EndQuery call. Fot this query</para>
		/// <para>type IDeviceContext::BeginQuery is disabled. IQuery::GetData fills a Diligent::QueryDataTimestamp struct.</para>
		/// </summary>
		Timestamp = 3,
		/// <summary>
		/// <para>Gets pipeline statistics, such as the number of pixel shader invocations in between IDeviceContext::BeginQuery</para>
		/// <para>and IDeviceContext::EndQuery. IQuery::GetData will fills a Diligent::QueryDataPipelineStatistics struct.</para>
		/// </summary>
		PipelineStatistics = 4,
		/// <summary>The number of query types in the enum</summary>
		NumTypes = 5
	}

	/// <summary>
	/// <para>Occlusion query data.</para>
	/// <para>This structure is filled by IQuery::GetData() for Diligent::QUERY_TYPE_OCCLUSION query type.</para>
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct QueryDataOcclusion
	{
		/// <summary>Query type - must be Diligent::QUERY_TYPE_OCCLUSION</summary>
		public QueryType Type;

		/// <summary>
		/// <para>The number of samples that passed the depth and stencil tests in between</para>
		/// <para>IDeviceContext::BeginQuery and IDeviceContext::EndQuery.</para>
		/// </summary>
		public ulong NumSamples;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public QueryDataOcclusion( bool unused )
		{
			Type = QueryType.Occlusion;
			NumSamples = 0;
		}
	}

	/// <summary>
	/// <para>Binary occlusion query data.</para>
	/// <para>This structure is filled by IQuery::GetData() for Diligent::QUERY_TYPE_BINARY_OCCLUSION query type.</para>
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct QueryDataBinaryOcclusion
	{
		/// <summary>Query type - must be Diligent::QUERY_TYPE_BINARY_OCCLUSION</summary>
		public QueryType Type;

		byte m_AnySamplePassed;
		/// <summary>
		/// <para>Indicates if at least one sample passed depth and stencil testing in between</para>
		/// <para>IDeviceContext::BeginQuery and IDeviceContext::EndQuery.</para>
		/// </summary>
		public bool AnySamplePassed
		{
			get => ( 0 != m_AnySamplePassed );
			set => m_AnySamplePassed = MiscUtils.byteFromBool( value );
		}

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public QueryDataBinaryOcclusion( bool unused )
		{
			Type = QueryType.BinaryOcclusion;
			m_AnySamplePassed = 0;
		}
	}

	/// <summary>
	/// <para>Timestamp query data.</para>
	/// <para>This structure is filled by IQuery::GetData() for Diligent::QUERY_TYPE_TIMESTAMP query type.</para>
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct QueryDataTimestamp
	{
		/// <summary>Query type - must be Diligent::QUERY_TYPE_TIMESTAMP</summary>
		public QueryType Type;

		/// <summary>The value of a high-frequency counter.</summary>
		public ulong Counter;

		/// <summary>
		/// <para>The counter frequency, in Hz (ticks/second). If there was an error</para>
		/// <para>while getting the timestamp, this value will be 0.</para>
		/// </summary>
		public ulong Frequency;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public QueryDataTimestamp( bool unused )
		{
			Type = QueryType.Timestamp;
			Counter = 0;
			Frequency = 0;
		}
	}

	/// <summary>
	/// <para>Pipeline statistics query data.</para>
	/// <para>This structure is filled by IQuery::GetData() for Diligent::QUERY_TYPE_PIPELINE_STATISTICS query type.</para>
	/// </summary>
	/// <remarks>In OpenGL backend the only field that will be populated is ClippingInvocations.</remarks>
	[StructLayout( LayoutKind.Sequential )]
	public struct QueryDataPipelineStatistics
	{
		/// <summary>Query type - must be Diligent::QUERY_TYPE_PIPELINE_STATISTICS</summary>
		public QueryType Type;

		/// <summary>Number of vertices processed by the input assembler stage.</summary>
		public ulong InputVertices;

		/// <summary>Number of primitives processed by the input assembler stage.</summary>
		public ulong InputPrimitives;

		/// <summary>Number of primitives output by a geometry shader.</summary>
		public ulong GSPrimitives;

		/// <summary>Number of primitives that were sent to the clipping stage.</summary>
		public ulong ClippingInvocations;

		/// <summary>
		/// <para>Number of primitives that were output by the clipping stage and were rendered.</para>
		/// <para>This may be larger or smaller than ClippingInvocations because after a primitive is</para>
		/// <para>clipped sometimes it is either broken up into more than one primitive or completely culled.</para>
		/// </summary>
		public ulong ClippingPrimitives;

		/// <summary>Number of times a vertex shader was invoked.</summary>
		public ulong VSInvocations;

		/// <summary>Number of times a geometry shader was invoked.</summary>
		public ulong GSInvocations;

		/// <summary>Number of times a pixel shader shader was invoked.</summary>
		public ulong PSInvocations;

		/// <summary>Number of times a hull shader shader was invoked.</summary>
		public ulong HSInvocations;

		/// <summary>Number of times a domain shader shader was invoked.</summary>
		public ulong DSInvocations;

		/// <summary>Number of times a compute shader was invoked.</summary>
		public ulong CSInvocations;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public QueryDataPipelineStatistics( bool unused )
		{
			Type = QueryType.PipelineStatistics;
			InputVertices = 0;
			InputPrimitives = 0;
			GSPrimitives = 0;
			ClippingInvocations = 0;
			ClippingPrimitives = 0;
			VSInvocations = 0;
			GSInvocations = 0;
			PSInvocations = 0;
			HSInvocations = 0;
			DSInvocations = 0;
			CSInvocations = 0;
		}
	}

	/// <summary>Query description.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct QueryDesc
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public DeviceObjectAttribs baseStruct;

		/// <summary>Query type, see Diligent::QUERY_TYPE.</summary>
		public QueryType Type;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public QueryDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
			Type = QueryType.Undefined;
		}
	}
}
