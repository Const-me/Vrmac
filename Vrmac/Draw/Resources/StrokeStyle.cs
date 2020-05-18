using System;

namespace Vrmac.Draw
{
	sealed class StrokeStyle: iStrokeStyle
	{
		public sStrokeStyle strokeStyle { get; }

		public StrokeStyle( sStrokeStyle ss)
		{
			strokeStyle = ss;
		}
		void IDisposable.Dispose() { }
	}
}