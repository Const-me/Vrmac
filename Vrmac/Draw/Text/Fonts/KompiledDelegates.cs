namespace Vrmac.Draw.Text
{
	struct KompiledDelegates
	{
		public Kompiler.RenderLineDelegate renderLine, renderLineTransformed;
		public Kompiler.RenderLineDelegate renderLineCT, renderLineCTV;

		public Kompiler.LeftAlignedBlockDelegate leftAlignBlock, leftBlockTransformed;

		public Kompiler.LeftAlignedBlockDelegate leftBlockCT, leftBlockCTV;

		public Kompiler.ConsoleBlockDelegate consoleBlockCT, consoleBlockCTV;

		public Kompiler.MeasureBlockDelegate measureGray, measureCT, measureCTV;

		public void dropGrayscale()
		{
			renderLine = renderLineTransformed = null;
			leftAlignBlock = leftBlockTransformed = null;
			measureGray = null;
		}

		public void dropCleartype()
		{
			renderLineCT = renderLineCTV = null;
			leftBlockCT = leftBlockCTV = null;
			consoleBlockCT = consoleBlockCTV = null;
			measureCT = measureCTV = null;
		}
	}
}