using System;
using Vrmac.Draw.Main;
using Vrmac.Draw.Text;

namespace Vrmac.Draw.Shaders
{
	[Flags]
	enum eRenderPassFlags: byte
	{
		None = 0,
		Opaque = 1,
		Transparent = 2
	}

	struct Order
	{
		/// <summary>Resets to 0 with every flush. When non-negative, index of the mesh. When negative, index of the built-in rectangle or some other draw command-specific thing.</summary>
		public readonly int sn;
		/// <summary>Only resets to 0 when iDrawDevice.begin is called, i.e. it's persistent across flushes.</summary>
		public readonly int z;

		public Order( int sn, int z )
		{
			this.sn = sn;
			this.z = z;
		}
	}

	struct sDrawCall
	{
		public Matrix transform;
		public readonly int foreground, background;
		public readonly float mixFactor;
		public readonly DrawCallType drawCall;
		public readonly Order order;
		public readonly float vaa;

		internal sDrawCall( DrawCallType dc, ref Matrix trans, float vaa, int foreground, int background, float mixFactor, Order order )
		{
			drawCall = dc;
			transform = trans;
			this.vaa = vaa;
			this.foreground = foreground;
			this.background = background;
			this.mixFactor = mixFactor;
			this.order = order;
		}

		const int transparentIndex = (int)eNamedColor.Transparent;

		public static sDrawCall solidColorFill( Order order, ref Matrix trans, int color )
		{
			eVaaKind vaa = BuiltMeshesCache.filledMeshesVaa ? eVaaKind.Filled : eVaaKind.None;
			DrawCallType dc = new DrawCallType( eMesh.Filled, vaa );
			return new sDrawCall( dc, ref trans, 0, color, transparentIndex, MiscUtils.one, order );
		}

		public static sDrawCall builtinShapeNoVaa( Order order, ref Matrix trans, int color )
		{
			DrawCallType dc = new DrawCallType( eMesh.Filled, eVaaKind.None );
			return new sDrawCall( dc, ref trans, 0, color, transparentIndex, MiscUtils.one, order );
		}

		public static sDrawCall sprite( Order order, ref Matrix trans, int color )
		{
			DrawCallType dc = new DrawCallType( eBrush.Sprite, eMesh.SpriteRectangle, eVaaKind.None );
			return new sDrawCall( dc, ref trans, 0, color, 0, 0, order );
		}

		public static sDrawCall drawText( Order order, ref Matrix trans, int color, int backgroundColor, bool opaqueBackground, float physicalPixelSize, eTextRendering textRendering )
		{
			eMesh mesh = ( textRendering == eTextRendering.GrayscaleTransformed ) ? eMesh.TransformedText : eMesh.GlyphRun;
			eBrush brush = opaqueBackground ? eBrush.OpaqueColor : eBrush.SolidColor;

			eClearTypeKind clearTypeKind;
			if( textRendering == eTextRendering.ClearTypeHorizontal )
				clearTypeKind = eClearTypeKind.Straight;
			else
				clearTypeKind = eClearTypeKind.None;
			DrawCallType dc = new DrawCallType( brush, mesh, clearTypeKind );

			return new sDrawCall( dc, ref trans, physicalPixelSize, color, backgroundColor, MiscUtils.one, order );
		}

		public static sDrawCall solidColorStroke( Order order, ref Matrix trans, ref StrokeRenderParams srp )
		{
			DrawCallType dc = new DrawCallType( eMesh.Stroked, srp.vaa );
			return new sDrawCall( dc, ref trans, srp.vaaScaling,
				srp.strokeColor, srp.fillColor, srp.lineColorFade,
				order );
		}

		static sDrawCall stroke( eVaaKind vaaKind, ref Matrix trans, float vaaMul, int col, Order order )
		{
			DrawCallType dc = new DrawCallType( eMesh.Stroked, vaaKind );
			return new sDrawCall( dc, ref trans, vaaMul, col, transparentIndex, MiscUtils.one, order );
		}

		public static sDrawCall scaledStroke( Order order, ref Matrix trans, StrokeRenderParams srp, float newPixelSize )
		{
			/* int typeFlags = 0;
			if( srp.isThinLine )
				typeFlags |= 1;
			if( srp.width <= newPixelSize )
				typeFlags |= 2;

			switch( typeFlags )
			{
				case 0: // Was fat and still fat. Nothing to do here.
					{
						Vector4 color = srp.m_color;
						return stroke( eVaaKind.StrokedFat, ref trans, srp.vaaScaling, ref color, order );
					}

				case 1:
					// Was thin, now fat
					{
						Vector4 color = srp.m_color;
						float vaaScaling = srp.meshWidth / srp.width;
						return stroke( eVaaKind.StrokedFat, ref trans, vaaScaling, ref color, order );
					}
				case 2:
					// Was fat, now thin
					{
						Vector4 color = srp.m_color * ( srp.width / newPixelSize );
						float vaaScaling = 2 * srp.meshWidth / srp.width;
						return stroke( eVaaKind.StrokedThin, ref trans, vaaScaling, ref color, order );
					}
				case 3:
					{
						// Was thin and still thin
						Vector4 color = srp.m_color * ( srp.width / newPixelSize );
						float vaaScaling = MathF.Max( srp.pixel / newPixelSize, 1 );
						return stroke( eVaaKind.StrokedThin, ref trans, vaaScaling, ref color, order );
					}
			}
			throw new ApplicationException(); */
			throw new NotImplementedException();
		}
	}
}