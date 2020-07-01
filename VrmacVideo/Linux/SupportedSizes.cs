using System.Collections.Generic;
using System.Linq;
using Vrmac;

namespace VrmacVideo.Linux
{
	abstract class SupportedSizes
	{
		public eFrameSizeType type { get; protected set; }

		public abstract CSize maxSize { get; }
	}

	sealed class DiscreteSizes: SupportedSizes
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

		public override CSize maxSize => allSizes.Last();
	}

	sealed class ContinuousSizes: SupportedSizes
	{
		readonly sFrameSizeStepwise stepwise;

		internal ContinuousSizes( ref sFrameSizeEnum vals )
		{
			type = vals.type;
			stepwise = vals.stepwise;
		}

		public override CSize maxSize => new CSize( stepwise.maxWidth, stepwise.maxHeight );
	}
}