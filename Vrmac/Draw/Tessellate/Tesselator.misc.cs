using Diligent.Graphics;
using System;
using System.Collections.Generic;

namespace Vrmac.Draw.Tessellate
{
	sealed partial class Tesselator
	{
		static Rect computeDefaultClipRectangle( CSize size )
		{
			float marginPixels = Math.Min( size.cx / 8, size.cy / 8 );
			Vector2 sizeFloats = size.asFloat;
			Vector2 marginClipSpaceUnits = ( 2.0f * sizeFloats ) / sizeFloats;
			return new Rect( -Vector2.One - marginClipSpaceUnits, Vector2.One + marginClipSpaceUnits );
		}

		void resized( CSize newSize, double dpiScaling )
		{
			lock( queues.syncRoot )
				clippingRectangle = computeDefaultClipRectangle( newSize );
		}

		IEnumerable<Meshes> allCachedMeshes()
		{
			foreach( var v in zeroInstances.values() )
				yield return v;
			foreach( var v in oneInstances.values() )
				yield return v;
			foreach( var dict in multiTable.values() )
				foreach( var mv in dict.Values )
					yield return mv;
		}

		Rect? iTesselator.customClippingRect
		{
			get
			{
				lock( queues.syncRoot )
					return customClippingRect;
			}
			set
			{
				lock( queues.syncRoot )
				{
					customClippingRect = value;
					foreach( var v in allCachedMeshes() )
						v.flushCached();
				}
			}
		}

		Rect getClippingRect()
		{
			return customClippingRect ?? clippingRectangle;
		}
	}
}