namespace Vrmac.Draw
{
	/// <summary>Interface for path data in managed memory. Create these objects with <see cref="Shapes" /> and <see cref="PathBuilder" /> classes.</summary>
	public interface iPathData
	{
		/// <summary>Upload for Direct2D</summary>
		Direct2D.iPathGeometry createPathGeometry( Direct2D.iDrawDevice device );

		/// <summary>Upload for the built-in backend</summary>
		iPathGeometry createPathGeometry( iVrmacDraw utils );
	}
}