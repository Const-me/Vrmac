using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using MultiTable = System.Runtime.CompilerServices.ConditionalWeakTable<Vrmac.Draw.iPathGeometry, System.Collections.Generic.Dictionary<int, Vrmac.Draw.Tessellate.Meshes>>;
using Table = System.Runtime.CompilerServices.ConditionalWeakTable<Vrmac.Draw.iPathGeometry, Vrmac.Draw.Tessellate.Meshes>;

namespace Vrmac.Draw.Tessellate
{
	sealed partial class Tesselator
	{
		readonly Table zeroInstances = new Table();
		readonly Table oneInstances = new Table();
		readonly MultiTable multiTable = new MultiTable();

		iTessellatedMeshes updateOldJob( Meshes meshes, ref Options options )
		{
			Debug.Assert( !options.separateStrokeMesh );

			lock( queues.syncRoot )
			{
				Options prevOptions = meshes.options;
				if( prevOptions.isGoodEnough( ref options ) && meshes.hasPolylines )
					return meshes;
				meshes.options = options;
				queues.enqueue( meshes );
				if( threadsRunning >= countThreads )
					return meshes;
				threadsRunning++;
			}
			ThreadPool.QueueUserWorkItem( postJobCallback );
			return meshes;
		}

		iTessellatedMeshes createNewJob( Meshes meshes, ref Options options )
		{
			Debug.Assert( !options.separateStrokeMesh );

			meshes.options = options;

			lock( queues.syncRoot )
			{
				queues.enqueue( meshes );
				if( threadsRunning >= countThreads )
					return meshes;
				threadsRunning++;
			}

			ThreadPool.QueueUserWorkItem( postJobCallback );
			return meshes;
		}

		iTessellatedMeshes postJob( iPathGeometry path, int instance, ref Matrix3x2 tform, eBuildFilledMesh filled, float pixel, sStrokeInfo? stroke )
		{
			if( null == path )
				throw new ArgumentNullException();

			// We use MD4 to detect changes in re-tessellated polylines. To make that more efficient for small changes of scaling, rounding the precision and pixel values.
			float precision = pixel.floorBitwise();
			Options options = new Options( ref tform, precision, pixel, filled, stroke, false );

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
					return updateOldJob( meshes, ref options );

				meshes = new Meshes( path, factory );
				dict.Add( instance, meshes );
				return createNewJob( meshes, ref options );
			}

			if( table.TryGetValue( path, out meshes ) )
				return updateOldJob( meshes, ref options );

			meshes = new Meshes( path, factory );
			table.Add( path, meshes );
			return createNewJob( meshes, ref options );
		}

		void releaseMeshes()
		{
			foreach( var v in allCachedMeshes() )
				v.dispose();

			zeroInstances.Clear();
			oneInstances.Clear();
			multiTable.Clear();
		}
	}
}