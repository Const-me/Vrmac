using System;
using System.Collections.Generic;
using System.Linq;
using Vrmac;
using VrmacVideo.Linux;

namespace VrmacVideo.Decoders
{
	abstract class SizeSupported
	{
		public eFrameSizeType type { get; protected set; }

		public abstract CSize maxSize { get; }

		public static SizeSupported query( VideoDevice device, ePixelFormat pixelFormat )
		{
			sFrameSizeEnum fse = device.frameSizeFirst( pixelFormat );
			switch( fse.type )
			{
				case eFrameSizeType.Discrete:
					break;
				case eFrameSizeType.Continuous:
				case eFrameSizeType.Stepwise:
					return new ContinuousSizes( ref fse );
				default:
					throw new ApplicationException();
			}

			return new DiscreteSizes( device.frameSizeEnum( pixelFormat ) );
		}
	}

	sealed class DiscreteSizes: SizeSupported
	{
		public readonly CSize[] allSizes;

		internal DiscreteSizes( IEnumerable<sFrameSizeEnum> values )
		{
			allSizes = values.Select( f => f.discreteFrameSize )
				.OrderBy( s => s.cy )
				.ThenBy( s => s.cx )
				.ToArray();

			type = eFrameSizeType.Discrete;
		}

		public override CSize maxSize => allSizes[ allSizes.Length - 1 ];

		public override string ToString()
		{
			return string.Join( ", ", allSizes.Select( s => $"{ s.cx } × { s.cy }" ) );
		}
	}

	sealed class ContinuousSizes: SizeSupported
	{
		readonly sFrameSizeStepwise stepwise;

		internal ContinuousSizes( ref sFrameSizeEnum vals )
		{
			type = vals.type;
			stepwise = vals.stepwise;
		}

		public override CSize maxSize => new CSize( stepwise.maxWidth, stepwise.maxHeight );

		public override string ToString()
		{
			if( type == eFrameSizeType.Continuous )
				return $"from { stepwise.minWidth } × { stepwise.minHeight } up to { stepwise.maxWidth } × { stepwise.maxHeight }";

			if( stepwise.stepHeight == stepwise.stepWidth )
				return $"from { stepwise.minWidth } × { stepwise.minHeight } up to { stepwise.maxWidth } × { stepwise.maxHeight }, with step { stepwise.stepWidth }";
			return $"from { stepwise.minWidth } × { stepwise.minHeight } up to { stepwise.maxWidth } × { stepwise.maxHeight }, with step { stepwise.stepWidth } × { stepwise.stepHeight }";
		}
	}
}