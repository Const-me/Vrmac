using Diligent.Graphics;
using System;

namespace Vrmac.Utils.Cursor.Render
{
	interface iCursorRender: IDisposable
	{
		void updatePosition( ref Vector4 positionAndSize );

		void render( IDeviceContext context, IBuffer vertexBuffer );
	}
}