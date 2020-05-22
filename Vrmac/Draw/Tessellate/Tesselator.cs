using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Vrmac.Draw.Shaders;

namespace Vrmac.Draw.Tessellate
{
	sealed partial class Tesselator: iTesselator
	{
		readonly iVrmacDraw factory;
		readonly Queues queues = new Queues();
		readonly Context context;

		Rect clippingRectangle;
		public Rect? customClippingRect = null;

		public Tesselator( Context context, iVrmacDraw factory )
		{
			postJobCallback = threadMain;
			this.factory = factory;

			clippingRectangle = computeDefaultClipRectangle( context.swapChainSize );
			context.swapChainResized.add( this, resized );

			this.context = context;
		}

		void iTesselator.begin()
		{
		}

		void IDisposable.Dispose()
		{
			lock( queues.syncRoot )
			{
				queues.dropPending();
				while( threadsRunning > 0 )
					Monitor.Wait( queues.syncRoot );
				queues.dropReady();
			}
			releaseMeshes();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static sDrawCallInfo filledCallInfo( eBuildFilledMesh fillOptions )
		{
			switch( fillOptions & ( eBuildFilledMesh.VAA | eBuildFilledMesh.BrushHasTransparency ) )
			{
				case eBuildFilledMesh.None:
					// Opaque brush without VAA
					return new sDrawCallInfo( eRenderPassFlags.Opaque, 1 );

				case eBuildFilledMesh.VAA:
					// VAA emits both opaque and transparent triangles in that mode. It even partitions vertices in the buffer for better GPU cache utilization.
					return new sDrawCallInfo( eRenderPassFlags.Opaque | eRenderPassFlags.Transparent, 2 );

				case eBuildFilledMesh.BrushHasTransparency:
				case eBuildFilledMesh.BrushHasTransparency | eBuildFilledMesh.VAA:
					// When the brush has transparency, VAA or not, the complete mesh will go to the transparent index buffer
					return new sDrawCallInfo( eRenderPassFlags.Transparent, 1 );
			}
			throw new ApplicationException();
		}

		sPendingDrawCall iTesselator.fill( iPathGeometry path, ref Matrix3x2 tform, float pixel, eBuildFilledMesh fillOptions, int instance )
		{
			var job = postJob( path, instance, ref tform, fillOptions, pixel, null );

			return new sPendingDrawCall( job, filledCallInfo( fillOptions ) );
		}

		sPendingDrawCall iTesselator.stroke( iPathGeometry path, ref Matrix3x2 tform, float pixel, sStrokeInfo stroke, int instance )
		{
			var job = postJob( path, instance, ref tform, eBuildFilledMesh.None, pixel, stroke );

			return new sPendingDrawCall( job, eRenderPassFlags.Transparent, 1 );
		}

		sPendingDrawCall iTesselator.fillAndStroke( iPathGeometry path, ref Matrix3x2 tform, float pixel, eBuildFilledMesh fillOptions, sStrokeInfo stroke, int instance )
		{
			var job = postJob( path, instance, ref tform, fillOptions, pixel, stroke );

			if( !fillOptions.HasFlag( eBuildFilledMesh.BrushHasTransparency ) )
				return new sPendingDrawCall( job, eRenderPassFlags.Opaque | eRenderPassFlags.Transparent, 2 );
			return new sPendingDrawCall( job, eRenderPassFlags.Transparent, 1 );
		}

		(sPendingDrawCall, sPendingDrawCall) iTesselator.fillAndStrokeSeparate( iPathGeometry path, ref Matrix3x2 tform, float pixel, eBuildFilledMesh fillOptions, sStrokeInfo stroke, int instance )
		{
			var jobs = postJobSeparate( path, instance, ref tform, fillOptions, pixel, stroke );

			var pendingFill = new sPendingDrawCall( jobs.Item1, filledCallInfo( fillOptions ) );
			var pendingStroke = new sPendingDrawCall( jobs.Item2, eRenderPassFlags.Transparent, 1 );
			return (pendingFill, pendingStroke);
		}

		void waitForAllJobs()
		{
			if( threadsRunning <= 0 )
				return;

			var rect = getClippingRect();
			iPolylinePath tempPoly = null;

			// Stealing work from background threads back to main
			while( true )
			{
				var job = queues.dequeue();
				if( !job )
					break;

				Monitor.Exit( queues.syncRoot );

				if( null == tempPoly )
					tempPoly = TempPolylines.getTemp( factory );
				bool rdy = job.tessellate( ref rect, tempPoly );

				Monitor.Enter( queues.syncRoot );

				queues.completed( ref job, rdy );
			}

			while( threadsRunning > 0 )
				Monitor.Wait( queues.syncRoot );
		}

		void iTesselator.syncThreads()
		{
			lock( queues.syncRoot )
			{
				waitForAllJobs();
				queues.present();
			}
		}
	}
}