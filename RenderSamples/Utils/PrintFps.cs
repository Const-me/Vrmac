using Diligent.Graphics;
using System;
using System.Threading.Tasks;

namespace RenderSamples.Utils
{
	class PrintFps
	{
		static readonly TimeSpan printFrequency = TimeSpan.FromSeconds( 5 );
		readonly FramesPerSecond framesPerSecond = new FramesPerSecond();

		public PrintFps()
		{
			runAsync();
		}

		public void rendered()
		{
			framesPerSecond.rendered();
		}

		async void runAsync()
		{
			while( true )
			{
				await Task.Delay( printFrequency );
				float? fps = framesPerSecond.framesPerSecond;
				if( fps.HasValue )
					Console.WriteLine( "FPS: {0:G3}", fps.Value );
			}
		}
	}
}