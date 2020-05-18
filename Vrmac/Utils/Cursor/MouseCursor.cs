using Vrmac.Utils.Cursor.Render;

namespace Vrmac.Utils.Cursor
{
	sealed class MouseCursor
	{
		readonly CursorRenderer renderer;

		public MouseCursor( Context context )
		{
			using( var dev = context.device )
				renderer = new CursorRenderer( context, dev );
		}

		public void dispose()
		{

		}

		public void hide() => cursor = eCursor.None;

		public void render()
		{
			if( cursor == eCursor.None )
				return;
			renderer.render();
		}

		CPoint m_position;
		public CPoint position
		{
			get => m_position;
			set
			{
				m_position = value;
				renderer.position = value;
				if( m_cursor != eCursor.None )
					renderer.context.queueRenderFrame();
			}
		}

		eCursor m_cursor = eCursor.None;
		public eCursor cursor
		{
			get => m_cursor;
			set => setCursor( value );
		}

		readonly CursorTexture[] m_textures = new CursorTexture[ 6 ];

		void setCursor( eCursor cur )
		{
			if( cur == m_cursor )
				return;
			m_cursor = cur;
			if( cur == eCursor.None )
				return;

			CursorTexture texture = m_textures[ (byte)cur ];
			if( null == texture )
			{
				using( var dev = renderer.context.device )
					texture = BuiltinCursors.loadCursor( dev, cur );
				m_textures[ (byte)cur ] = texture;
			}
			renderer.cursor = texture;
		}
	}
}