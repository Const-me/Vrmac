namespace VrmacVideo.Linux
{
	public struct sImageFormat
	{
		public readonly eBufferType bufferType;
		public readonly int index;
		public readonly string description;
		public readonly ePixelFormat pixelFormat;
		public readonly eImageFormatDescriptionFlags flags;

		internal sImageFormat( ref sImageFormatDescription src )
		{
			bufferType = src.type;
			index = src.index;
			description = src.description;
			pixelFormat = src.pixelFormat;
			flags = src.flags;
		}

		public override string ToString()
		{
			return $"buffer { bufferType }, index { index }: \"{ description  }\", { pixelFormat }, flags { flags }";
		}
	}
}