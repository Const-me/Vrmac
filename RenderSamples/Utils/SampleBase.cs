using Diligent.Graphics;
using RenderSamples.Utils;
using System.Linq;
using Vrmac;
using Vrmac.Animation;
using Vrmac.Input;
using Vrmac.Input.Linux;

namespace RenderSamples
{
	abstract class SampleBase: iScene, iKeyboardInput
	{
		// Change this to false if you want to compare with D2D. If you'll try to render text will fail with NotImplementedException, but vector graphics does work.
		static readonly bool preferVrmac2D = true;

		public readonly Context context;
		public bool isOpenGlDevice => context.isOpenGlDevice;
		public Animations animation => context.animation;

		public readonly string name;

		public SampleBase()
		{
			name = GetType().Name;
			eCreateContextFlags flags = eCreateContextFlags.CacheCompiledShaders;
			if( preferVrmac2D )
				flags |= eCreateContextFlags.PreferVrmac2D;
			context = new Context( this, flags );
		}

		readonly PrintFps fps = new PrintFps();

		protected void printFps()
		{
			fps.rendered();
		}

		// iKeyboardHandler iKeyboardInput.keyboardHandler => new LogKeyboardEvents();
		iKeyboardHandler iKeyboardInput.keyboardHandler => new SampleKeyboardHandler( context );
		RawDevice iKeyboardInput.getKeyboardDevice()
		{
			// I've only plugged 2 USB devices, wireless receivers for Logitech G700s and VelociFire VM02WS, however Linux says there're many keyboards.
			// The keyboard that actually works on my system is event4.
			return RawDevice.list().FirstOrDefault( d => d.eventInterface == "/dev/input/event4" );
		}

		protected abstract void createResources( IRenderDevice device );
		protected abstract void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil );

		void iScene.createResources( Context context, IRenderDevice device ) => createResources( device );
		void iScene.render( Context context, ITextureView swapChainRgb, ITextureView swapChainDepthStencil ) => render( swapChainRgb, swapChainDepthStencil );
	}
}