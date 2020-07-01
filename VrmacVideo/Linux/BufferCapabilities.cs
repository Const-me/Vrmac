using System.Collections.Generic;

namespace VrmacVideo.Linux
{
	sealed class BufferCapabilities
	{
		static readonly Dictionary<string, BufferCapabilities> cache = new Dictionary<string, BufferCapabilities>();

		public static BufferCapabilities getBufferCaps( string path, VideoDevice device )
		{
			if( cache.TryGetValue( path, out var res ) )
				return res;

			res = new BufferCapabilities( device );
			cache.Add( path, res );
			return res;
		}

		static eBufferCapabilityFlags query( VideoDevice device, eBufferType bt )
		{
			eBufferCapabilityFlags res = eBufferCapabilityFlags.None;

			if( device.queryBufferCaps( bt, eMemory.MemoryMap ) )
				res |= eBufferCapabilityFlags.MemoryMap;
			if( device.queryBufferCaps( bt, eMemory.UserPointer ) )
				res |= eBufferCapabilityFlags.UserPointer;
			if( device.queryBufferCaps( bt, eMemory.DmaSharedBuffer ) )
				res |= eBufferCapabilityFlags.DmaSharedBuffer;

			return res;
		}

		BufferCapabilities( VideoDevice device )
		{
			videoOutput = query( device, eBufferType.VideoOutput );
			videoOutputMPlane = query( device, eBufferType.VideoOutputMPlane );
		}

		public eBufferCapabilityFlags videoOutput { get; }
		public eBufferCapabilityFlags videoOutputMPlane { get; }
	}
}