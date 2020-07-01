namespace VrmacVideo.Containers.MP4
{
	public abstract class ChunkOffsetTable
	{
		public abstract int count { get; }
		public abstract long this[ int index ] { get; }
	}

	sealed class ChunkOffsetTable32: ChunkOffsetTable
	{
		public readonly uint[] entries;

		internal ChunkOffsetTable32( uint[] entries )
		{
			this.entries = entries;
		}

		public override int count => entries.Length;
		public override long this[ int index ] => entries[ index ];

		public override string ToString() =>
			$"32-bit chunk offset table, { entries.Length } entries";
	}

	sealed class ChunkOffsetTable64: ChunkOffsetTable
	{
		public readonly long[] entries;

		internal ChunkOffsetTable64( long[] entries )
		{
			this.entries = entries;
		}

		public override int count => entries.Length;
		public override long this[ int index ] => entries[ index ];

		public override string ToString() =>
			$"64-bit chunk offset table, { entries.Length } entries";
	}
}