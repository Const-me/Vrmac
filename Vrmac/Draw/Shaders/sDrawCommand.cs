namespace Vrmac.Draw.Shaders
{
	struct sDrawCommand
	{
		public readonly iPathGeometry geometry;
		public readonly eMesh command;
		public readonly sStrokeStyle strokeStyle;
		public readonly float width;

		sDrawCommand( iPathGeometry geometry, eMesh command, sStrokeStyle ss, float w )
		{
			this.geometry = geometry;
			this.command = command;
			strokeStyle = ss;
			width = w;
		}

		public static sDrawCommand fill( iPathGeometry geometry )
		{
			return new sDrawCommand( geometry, eMesh.Filled, default, default );
		}
		public static sDrawCommand stroke( iPathGeometry geometry, sStrokeStyle ss, float w )
		{
			return new sDrawCommand( geometry, eMesh.Stroked, ss, w );
		}
	}
}