using System;

namespace Vrmac.Draw.Tessellate
{
	// Thanks to these MD4 hashes, we don’t need polyline paths for every object rendered, we only need one polyline path per thread.
	// This saves megabytes of RAM, also the data is likely to stay in CPU caches i.e. the processing is faster.
	static class TempPolylines
	{
		[ThreadStatic]
		static iPolylinePath tempPolyline;

		public static iPolylinePath getTemp( iVrmacDraw factory )
		{
			iPolylinePath tmp = tempPolyline;
			if( null != tmp )
				return tmp;
			tmp = factory.createPolylinePath();
			tempPolyline = tmp;
			return tmp;
		}
	}
}