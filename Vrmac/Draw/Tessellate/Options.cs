using System.Numerics;

namespace Vrmac.Draw.Tessellate
{
	struct Options
	{
		public readonly Matrix3x2 transform;
		public readonly float precision, pixel;
		public readonly eBuildFilledMesh fill;
		public readonly sStrokeInfo stroke;
		public readonly bool separateStrokeMesh;

		public Options( ref Matrix3x2 transform, float precision, float pixel, eBuildFilledMesh fill, sStrokeInfo? stroke, bool strokeSeparate )
		{
			this.transform = transform;
			this.precision = precision;
			this.pixel = pixel;
			this.fill = fill;
			this.stroke = stroke ?? default;
			separateStrokeMesh = strokeSeparate;
		}

		const float translationThreshold = 1.0f / 16.0f;

		static bool isSmallEnoughChange( Matrix3x2 a, Matrix3x2 b )
		{
			Vector2 diffTrans = a.Translation - b.Translation;
			if( diffTrans.absolute().maxCoordinate() > translationThreshold )
				return false;
			Vector4 r1 = a.rotationMatrix() - b.rotationMatrix();
			if( r1.absolute().maxCoordinate() > 0.015625f )
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
			if( that.separateStrokeMesh != separateStrokeMesh )
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
			if( that.separateStrokeMesh != separateStrokeMesh )
				return false;
			return true;
		}
	}
}