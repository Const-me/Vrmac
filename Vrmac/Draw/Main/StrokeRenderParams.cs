using Diligent.Graphics;
using System;

namespace Vrmac.Draw.Main
{
	struct StrokeRenderParams
	{
		internal readonly int strokeColor, fillColor;
		/// <summary>Width specified by user</summary>
		internal readonly float width;
		/// <summary>Physical pixel size in the units of the path.</summary>
		internal readonly float pixel;

		public readonly eVaaKind vaa;

		public bool isThinLine => vaa == eVaaKind.StrokedThin;

		public static readonly float outlineMeshWidth;
		static readonly float lineMeshWidth;

		static StrokeRenderParams()
		{
			outlineMeshWidth = 2.0f * MathF.Sqrt( 2 );
			lineMeshWidth = outlineMeshWidth;
		}

		StrokeRenderParams( int strokeColor, int fillColor, float w, float px, bool vaa )
		{
			this.strokeColor = strokeColor;
			this.fillColor = fillColor;
			width = w;
			pixel = px;

			if( !vaa )
			{
				this.vaa = eVaaKind.None;
				meshWidth = w;
			}
			else if( w <= px )
			{
				this.vaa = eVaaKind.StrokedThin;
				meshWidth = pixel * 2;
			}
			else
			{
				this.vaa = eVaaKind.StrokedFat;
				meshWidth = width + pixel * lineMeshWidth;
			}
		}

		public static StrokeRenderParams strokedPath( int c, float w, float px, int filledColor = (int)eNamedColor.Transparent )
		{
			return new StrokeRenderParams( c, filledColor, w, px, BuiltMeshesCache.strokeMeshesVaa );
		}
		public static StrokeRenderParams filledAndStrokedPath( int c, int fc, float w, float px )
		{
			return new StrokeRenderParams( c, fc, w, px, BuiltMeshesCache.strokeMeshesVaa );
		}

		StrokeRenderParams( int c, float px )
		{
			strokeColor = c;
			fillColor = (int)eNamedColor.Transparent;
			width = px;
			pixel = px;
			vaa = eVaaKind.StrokedThin;
			meshWidth = px * outlineMeshWidth;
		}

		public static StrokeRenderParams outlineOfFilledMesh( int c, float px )
		{
			return new StrokeRenderParams( c, px );
		}

		/// <summary>Width of the generated mesh</summary>
		public readonly float meshWidth;

		float getLineColorFade()
		{
			if( !isThinLine )
				return 1;
			return ( width / pixel );
		}

		public float lineColorFade => getLineColorFade();

		public float vaaScaling =>
			isThinLine ? 1 : meshWidth / width;
	};
}