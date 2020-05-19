using Diligent.Graphics;
using System;
using System.Numerics;

namespace Vrmac.Utils.Cursor.Render
{
	interface iCursorRender: IDisposable
	{
		void updatePosition( ref Vector4 positionAndSize );

		void render( IDeviceContext context, IBuffer vertexBuffer );
	}
}