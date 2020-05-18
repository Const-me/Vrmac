using Diligent.Graphics;

namespace Vrmac.Draw.Tessellate
{
	struct Options
	{
		public readonly Matrix transform;
		public readonly float precision, pixel;
		public readonly eBuildFilledMesh fill;
		public readonly sStrokeInfo stroke;

		public Options( ref Matrix transform, float precision, float pixel, eBuildFilledMesh fill, sStrokeInfo? stroke )
		{
			this.transform = transform;
			this.precision = precision;
			this.pixel = pixel;
			this.fill = fill;
			this.stroke = stroke ?? default;
		}

		const float translationThreshold = 1.0f / 16.0f;

		static bool isSmallEnoughChange( Matrix a, Matrix b )
		{
			Vector2 diffTrans = a.translation - b.translation;
			if( diffTrans.absolute().maxCoordinate > translationThreshold )
				return false;
			Vector4 r1 = a.rotationMatrix - b.rotationMatrix;
			if( r1.absolute().maxCoordinate > 0.015625f )
				return false;
			return true;
		}

		public bool isGoodEnough( ref Options that )
		{
			if( that.precision != precision )
				return false;
			if( that.fill != fill )
				return false;
			if( that.stroke != stroke )
				return false;
			return isSmallEnoughChange( transform, that.transform );
		}

		public bool equal( ref Options that )
		{
			if( that.precision != precision )
				return false;
			if( that.fill != fill )
				return false;
			if( that.stroke != stroke )
				return false;
			if( that.transform != transform )
				return false;
			return true;
		}
	}
}