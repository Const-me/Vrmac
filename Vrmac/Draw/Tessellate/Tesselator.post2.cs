using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using Table = System.Runtime.CompilerServices.ConditionalWeakTable<Vrmac.Draw.iPathGeometry, Vrmac.Draw.Tessellate.Meshes>;

namespace Vrmac.Draw.Tessellate
{
	sealed partial class Tesselator
	{
		(iTessellatedMeshes, iTessellatedMeshes) updateOldJobSeparate( Meshes meshes, ref Options options )
		{
			Debug.Assert( options.separateStrokeMesh );

			lock( queues.syncRoot )
			{
				Options prevOptions = meshes.options;
				if( prevOptions.isGoodEnough( ref options ) && meshes.hasPolylines )
					return (meshes, meshes.extraStroke);
				meshes.options = options;
				meshes.ensureExtraStroke( factory );
				queues.enqueue( meshes );
				if( threadsRunning >= countThreads )
					return (meshes, meshes.extraStroke);
				threadsRunning++;
			}
			ThreadPool.QueueUserWorkItem( postJobCallback );
			return (meshes, meshes.extraStroke);
		}

		(iTessellatedMeshes, iTessellatedMeshes) createNewJobSeparate( Meshes meshes, ref Options options )
		{
			Debug.Assert( options.separateStrokeMesh );

			meshes.options = options;
			meshes.ensureExtraStroke( factory );

			lock( queues.syncRoot )
			{
				queues.enqueue( meshes );
				if( threadsRunning >= countThreads )
					return (meshes, meshes.extraStroke);
				threadsRunning++;
			}

			ThreadPool.QueueUserWorkItem( postJobCallback );
			return (meshes, meshes.extraStroke);
		}

		(iTessellatedMeshes, iTessellatedMeshes) postJobSeparate( iPathGeometry path, int instance, ref Matrix3x2 tform, eBuildFilledMesh filled, float pixel, sStrokeInfo stroke )
		{
			if( null == path )
				throw new ArgumentNullException();

			float precision = pixel.floorBitwise();
			Options options = new Options( ref tform, precision, pixel, filled, stroke, true );

			Meshes meshes;
			Table table;
			if( 0 == instance )
				table = zeroInstances;
			else if( 1 == instance )
				table = oneInstances;
			else
			{
				var dict = multiTable.GetOrCreateValue( path );

				if( dict.TryGetValue( instance, out meshes ) )
					return updateOldJobSeparate( meshes, ref options );

				meshes = new Meshes( path, factory );
				dict.Add( instance, meshes );
				return createNewJobSeparate( meshes, ref options );
			}

			if( table.TryGetValue( path, out meshes ) )
				return updateOldJobSeparate( meshes, ref options );

			meshes = new Meshes( path, factory );
			table.Add( path, meshes );
			return createNewJobSeparate( meshes, ref options );
		}
	}
}