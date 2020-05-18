using System;

namespace Vrmac.Input.Linux
{
	/// <summary>Applies metadata to parse the /proc/bus/input/devices fake file on Linux</summary>
	public class BitFieldsKeyAttribute: Attribute
	{
		/// <summary>Value on the left side of '=' in /proc/bus/input/devices</summary>
		public readonly string key;

		/// <summary>Construct the attribute</summary>
		public BitFieldsKeyAttribute( string key )
		{
			this.key = key;
		}
	}
}