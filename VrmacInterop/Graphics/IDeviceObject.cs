using System;
using ComLight;

namespace Diligent.Graphics
{
	/// <summary>Base interface for all objects created by <see cref="IRenderDevice" />.</summary>
	[ComInterface( "5b4cca0b-5075-4230-9759-f48769ee5502" )]
	public interface IDeviceObject: IDisposable
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		void getIid( out Guid iid );

		/// <summary>Returns unique identifier assigned to an object</summary>
		/// <remarks>
		/// <para>Unique identifiers can be used to reliably check if two objects are identical.
		/// Note that the engine reuses memory reclaimed after an object has been released.
		/// For example, if a texture object is released and then another texture is created, the engine may return the same pointer, so pointer-to-pointer comparisons are not reliable.Unique identifiers, on the other hand, are guaranteed to be, well, unique.</para>
		/// <para>Unique identifiers are object-specifics, so, for instance, buffer identifiers are not comparable to texture identifiers.</para>
		/// <para>Unique identifiers are only meaningful within one session. After an application restarts, all identifiers become invalid.</para>
		/// <para>Valid identifiers are always positive values. Zero and negative values can never be assigned to an object and are always guaranteed to be invalid.</para>
		/// </remarks>
		int GetUniqueID();

		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		/// <remarks>It's used internally to cast objects of varying types bound to GPU pipeline.</remarks>
		IntPtr nativeCast();
	}
}