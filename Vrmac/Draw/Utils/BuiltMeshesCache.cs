namespace Vrmac.Draw
{
	// Less than ideal, doesn't update tessellation precision.
	// And if you render a complex mesh twice per frame, with different stroke style or width, it will rebuild the mesh twice per frame.
	// But it's probably good enough for the first version.
	// Also, this class doesn't access GPU. The work it does should be offloaded to CLR thread pool, Pi4 has 4 CPU cores, the graphics engine only uses 1 so far.
	class BuiltMeshesCache
	{
		// This affects both filled and stroked meshes, but only for paths which have curves. This does nothing at all for polygons.
		const float polylinesPrecision = 0.5f;

		// This only affects filled paths. Enabling it drops performance on Pi by a large factor.
		// Unfortunately, VAA of filled shapes needs these higher quality meshes.
		const bool higherQualityFillMeshes = false;

		public const bool filledMeshesVaa = true;

		public const bool strokeMeshesVaa = true;
	}
}