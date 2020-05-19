using Diligent.Graphics;
using System;
using System.Numerics;
using Vrmac.Draw.Shaders;
using Vrmac.Draw.Text;

namespace Vrmac.Draw.Main
{
	abstract partial class ImmediateContext
	{
		// === Abstract methods ====

		/// <summary>Transform from the coordinate systems of the input geometry into clip space.</summary>
		protected abstract void getCurrentTransform( out Matrix matrix, out float physicalPixelSize );
		protected abstract CPoint transformToPhysicalPixels( Vector2 point, out IntMatrix? intMatrix );
		/// <summary>Scaling component of the top matrix on the stack. Used to try to fix lines when re-playing a list.</summary>
		protected abstract Vector2 getUserScaling();

		protected abstract void updatePalette();
		protected abstract void updateFontTextures();

		/// <summary>Construct the object</summary>
		public ImmediateContext( Context context, iVrmacDraw factory, PaletteTexture palette )
		{
			resources = new GpuResources( context, palette );
			states = new StatesCache( resources );
			depthValues = DepthValues.create( context.swapChainFormats.depth );
			tesselatorThread = new Tessellate.Tesselator( context, factory );
		}

		// Just for the text, really.
		eRenderPassFlags passFlags;

		// ==== API ====
		protected void begin()
		{
			currentZ = 0;
			drawCallsUpperBound = 0;
			calls.clear();
			drawMeshes.clear();
			tesselatorThread.begin();

			passFlags = eRenderPassFlags.None;
		}

		protected void fillGeometry( iPathGeometry path, SolidColorData color, int instance = 0 )
		{
			getCurrentTransform( out Matrix tform, out float pixel );
			if( !path.testApproximateBounds( ref tform ) || color.brushType == eBrushType.Null )
				return;

			eBuildFilledMesh fillOptions = DrawUtilsPrivate.fillFlagsFromColor( color.brushType, false );
			if( fillOptions == eBuildFilledMesh.None )
				return; // Null brush

			sPendingDrawCall pdc = tesselatorThread.fill( path, ref tform, pixel, fillOptions, instance );
			flushIfNeeded( pdc.drawInfo.drawCallsCount );

			Order o = order();
			drawMeshes.meshes.add( ref pdc );
			calls.add( sDrawCall.solidColorFill( o, ref tform, color.paletteIndex ) );
		}

		protected void strokeGeometry( iPathGeometry path, SolidColorData color, float width, ref sStrokeStyle strokeStyle, int instance = 0 )
		{
			getCurrentTransform( out Matrix tform, out float pixel );
			if( !path.testApproximateBounds( ref tform, width ) || color.brushType == eBrushType.Null )
				return;

			StrokeRenderParams srp = StrokeRenderParams.strokedPath( color.paletteIndex, width, pixel );
			sPendingDrawCall pdc = tesselatorThread.stroke( path, ref tform, pixel, new sStrokeInfo( strokeStyle, srp.meshWidth ), instance );
			flushIfNeeded( pdc.drawInfo.drawCallsCount );

			Order o = order();
			drawMeshes.meshes.add( ref pdc );
			calls.add( sDrawCall.solidColorStroke( o, ref tform, ref srp ) );
		}

		protected void fillAndStroke( iPathGeometry path, SolidColorData fillColor, SolidColorData strokeColor, float width, ref sStrokeStyle strokeStyle, int instance = 0 )
		{
			if( fillColor.brushType == eBrushType.Null )
			{
				strokeGeometry( path, strokeColor, width, ref strokeStyle, instance );
				return;
			}
			if( strokeColor.brushType == eBrushType.Null )
			{
				fillGeometry( path, fillColor, instance );
				return;
			}

			getCurrentTransform( out Matrix tform, out float pixel );
			if( !path.testApproximateBounds( ref tform, width ) )
				return;

			eBuildFilledMesh fillOptions = DrawUtilsPrivate.fillFlagsFromColor( fillColor.brushType, true );

			StrokeRenderParams srp = StrokeRenderParams.strokedPath( strokeColor.paletteIndex, width, pixel );
			sPendingDrawCall pdc = tesselatorThread.fillAndStroke( path, ref tform, pixel, fillOptions, new sStrokeInfo( strokeStyle, srp.meshWidth ), instance );
			flushIfNeeded( pdc.drawInfo.drawCallsCount );

			Order o = order();
			drawMeshes.meshes.add( ref pdc );
			calls.add( sDrawCall.solidColorStroke( o, ref tform, ref srp ) );
		}

		protected void drawRectangle( ref Rect rectangle, float width, int colorIndex )
		{
			getCurrentTransform( out Matrix tform, out float pixel );
			var transformedRect = tform.transformRectangle( ref rectangle );
			if( !DrawDevice.clipSpaceRectangle.intersects( ref transformedRect ) )
				return;

			flushIfNeeded( 1 );

			Order o = drawMeshes.addRectangle( ref currentZ, ref rectangle, width );
			calls.add( sDrawCall.builtinShapeNoVaa( o, ref tform, colorIndex ) );
		}

		protected void fillRectangle( ref Rect rectangle, SolidColorData color )
		{
			getCurrentTransform( out Matrix tform, out float pixel );
			var transformedRect = tform.transformRectangle( ref rectangle );
			if( !DrawDevice.clipSpaceRectangle.intersects( ref transformedRect ) || color.brushType == eBrushType.Null )
				return;

			flushIfNeeded( 1 );
			Order o = drawMeshes.addRectangle( ref currentZ, ref rectangle, null );
			calls.add( sDrawCall.builtinShapeNoVaa( o, ref tform, color.paletteIndex ) );
		}

		protected void drawSprite( ref Rect rectangle, ref Sprite uv )
		{
			getCurrentTransform( out Matrix tform, out float pixel );
			var transformedRect = tform.transformRectangle( ref rectangle );
			if( !DrawDevice.clipSpaceRectangle.intersects( ref transformedRect ) )
				return;

			flushIfNeeded( 1 );

			Order o = drawMeshes.addSprite( ref currentZ, ref rectangle, ref uv );
			calls.add( sDrawCall.sprite( o, ref tform, (int)eNamedColor.White ) );
		}

		protected static eTextRendering textRenderingStyle( IntMatrix? intMtx )
		{
			if( !intMtx.HasValue )
				return eTextRendering.GrayscaleTransformed;

			var matrix = intMtx.Value;
			if( matrix == IntMatrix.identity || matrix == IntMatrix.verticalFlip )
				return eTextRendering.ClearTypeHorizontal;

			// TODO: support vertical and flipped modes.
			return eTextRendering.GrayscaleExact;
		}

		protected void drawText( string text, Font font, ref Rect rectangle, SolidColorData foreground, SolidColorData background )
		{
			getCurrentTransform( out Matrix tform, out float pixel );
			var transformedRect = tform.transformRectangle( ref rectangle );
			if( !DrawDevice.clipSpaceRectangle.intersects( ref transformedRect ) || foreground.brushType == eBrushType.Null )
				return;

			flushIfNeeded( 1 );

			IntMatrix? intMtx;
			CPoint startingPoint = transformToPhysicalPixels( rectangle.topLeft, out intMtx );
			CSize size = ( rectangle.size / pixel ).roundToInt().asSize;
			CRect textRect = new CRect( startingPoint, size );
			eTextRendering trs = textRenderingStyle( intMtx );

			Order o = drawMeshes.addText( ref currentZ, text, font, ref textRect, foreground.paletteIndex, trs );

			bool opaqueBackground = background.brushType == eBrushType.Opaque;
			passFlags |= eRenderPassFlags.Transparent;
			calls.add( sDrawCall.drawText( o, ref tform, foreground.paletteIndex,
				background.paletteIndex, opaqueBackground,
				pixel, trs ) );
		}

		protected void drawConsoleText( string text, int width, Font font, Vector2 position, SolidColorData foreground, SolidColorData background )
		{
			getCurrentTransform( out Matrix tform, out float pixel );
			IntMatrix? intMtx;
			CPoint startingPoint = transformToPhysicalPixels( position, out intMtx );
			if( !intMtx.HasValue )
				throw new ApplicationException( "iDrawContext.drawConsoleText doesn't currently support transforms" );

			eTextRendering trs = textRenderingStyle( intMtx );

			CRect rect = new CRect( startingPoint, new CSize() );

			Order o = drawMeshes.addText( ref currentZ, text, font, ref rect, foreground.paletteIndex, trs, width );

			bool opaqueBackground = background.brushType == eBrushType.Opaque;
			passFlags |= eRenderPassFlags.Transparent;

			calls.add( sDrawCall.drawText( o, ref tform, foreground.paletteIndex,
				background.paletteIndex, opaqueBackground,
				pixel, trs ) );
		}

		public void flush()
		{
			tesselatorThread.syncThreads();

			// The commented out code doesn't work.
			// Might be a bug in the native code which results in false positive eClipResult.FillsEntireViewport result.
			// Spent couple hours trying to debug but failed, then disabled the optimization.

			/* int topmostFillingCmd = 0;
			for( int i = calls.length - 1; i >= 0; i-- )
			{
				ref sDrawCall dc = ref calls[ i ];
				if( dc.order.sn < 0 )
					continue;
				ref sPendingDrawCall pdc = ref drawMeshes.meshes[ dc.order.sn ];
				eRenderPassFlags rpf = pdc.mesh.drawInfo.renderPassFlags;
				passFlags |= rpf;

				eClipResult clipResult = pdc.mesh.clipResult;
				if( clipResult != eClipResult.FillsEntireViewport )
					continue;
				if( rpf != eRenderPassFlags.Opaque )
					continue;
				if( !pdc.mesh.sourcePath.flags.HasFlag( ePathFlags.AnyFilledFigures ) )
					continue;

				// Detected an opaque mesh that fills entire clip area. Discarding all draw commands before that.
				ConsoleLogger.logDebug( "Commands [ 0 - {0} ] are occluded, only rendering [ {1} - {2} ]", i - 1, i, calls.length - 1 );
				topmostFillingCmd = i;
				break;
			}
			Span<sDrawCall> drawCallsSpan = calls.read().Slice( topmostFillingCmd ); */

			Span<sDrawCall> drawCallsSpan = calls.read();

			drawMeshes.summarize( ref passFlags, out eShaderMacros drawFeatures );
			if( passFlags == eRenderPassFlags.None )
				return;

			updatePalette();
			if( drawFeatures.HasFlag( eShaderMacros.TextRendering ) )
				updateFontTextures();

			// Vertex/index buffers are concatenated and shared across the two passes.
			// The trade off here is "very rarely save few kb of bandwidth with 16 bit indices" versus "always save map/unmap interop calls and kernel calls"
			// Also, this slightly simplifies command lists implementation.
			layoutBuffers( drawCallsSpan );

			var ic = resources.context.context;

			uploadVertices( ic, drawCallsSpan );
			ic.setVertexBuffer( resources.vertexBuffer );

			GpuValueType indexType = uploadIndices( ic, drawCallsSpan );
			ic.SetIndexBuffer( resources.indexBuffer, 0 );

			bool anyOpaqueMeshes = false;
			if( buffersLayout.drawInfo.opaqueIndices > 0 )
			{
				VrmacStateBase state = states.getState( drawCallsSpan.Length, true, drawFeatures );
				state.uploadDrawCalls( ic, drawCallsSpan, depthValues, ref drawMeshes );
				state.bind( ic );
				DrawIndexedAttribs opaqueCall = buffersLayout.opaqueDrawAttribs( indexType );
				ic.DrawIndexed( ref opaqueCall );
				anyOpaqueMeshes = true;
			}

			if( buffersLayout.drawInfo.transparentIndices > 0 )
			{
				VrmacStateBase state = states.getState( drawCallsSpan.Length, false, drawFeatures );
				// The draw calls buffer is the same. Only uploading here if there were no opaque meshes.
				// The reason we can't do that in advance outside these if-s - different GPU states upload to different locations.
				if( !anyOpaqueMeshes )
					state.uploadDrawCalls( ic, drawCallsSpan, depthValues, ref drawMeshes );

				state.bind( ic );
				DrawIndexedAttribs transparentCall = buffersLayout.transparentDrawAttribs( indexType );
				ic.DrawIndexed( ref transparentCall );
			}

			// Clear everything, but keep the Z
			int z = currentZ;
			begin();
			currentZ = z;
		}
	}
}