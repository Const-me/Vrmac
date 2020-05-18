namespace Vrmac.ModeSet
{
	/// <summary>Linux kernel pixel format</summary>
	/// <seealso href="https://en.wikipedia.org/wiki/Direct_Rendering_Manager" />
	public enum eDrmFormat: uint
	{
		/// <summary>[7:0] C</summary>
		C8 = 0x20203843,	// DRM_FORMAT_C8
		/// <summary>[7:0] R</summary>
		R8 = 0x20203852,	// DRM_FORMAT_R8
		/// <summary>[15:0] G:R 8:8 little endian</summary>
		GR88 = 0x38385247,	// DRM_FORMAT_GR88
		/// <summary>[7:0] R:G:B 3:3:2</summary>
		RGB332 = 0x38424752,	// DRM_FORMAT_RGB332
		/// <summary>[7:0] B:G:R 2:3:3</summary>
		BGR233 = 0x38524742,	// DRM_FORMAT_BGR233
		/// <summary>[15:0] x:R:G:B 4:4:4:4 little endian</summary>
		XRGB4444 = 0x32315258,	// DRM_FORMAT_XRGB4444
		/// <summary>[15:0] x:B:G:R 4:4:4:4 little endian</summary>
		XBGR4444 = 0x32314258,	// DRM_FORMAT_XBGR4444
		/// <summary>[15:0] R:G:B:x 4:4:4:4 little endian</summary>
		RGBX4444 = 0x32315852,	// DRM_FORMAT_RGBX4444
		/// <summary>[15:0] B:G:R:x 4:4:4:4 little endian</summary>
		BGRX4444 = 0x32315842,	// DRM_FORMAT_BGRX4444
		/// <summary>[15:0] A:R:G:B 4:4:4:4 little endian</summary>
		ARGB4444 = 0x32315241,	// DRM_FORMAT_ARGB4444
		/// <summary>[15:0] A:B:G:R 4:4:4:4 little endian</summary>
		ABGR4444 = 0x32314241,	// DRM_FORMAT_ABGR4444
		/// <summary>[15:0] R:G:B:A 4:4:4:4 little endian</summary>
		RGBA4444 = 0x32314152,	// DRM_FORMAT_RGBA4444
		/// <summary>[15:0] B:G:R:A 4:4:4:4 little endian</summary>
		BGRA4444 = 0x32314142,	// DRM_FORMAT_BGRA4444
		/// <summary>[15:0] x:R:G:B 1:5:5:5 little endian</summary>
		XRGB1555 = 0x35315258,	// DRM_FORMAT_XRGB1555
		/// <summary>[15:0] x:B:G:R 1:5:5:5 little endian</summary>
		XBGR1555 = 0x35314258,	// DRM_FORMAT_XBGR1555
		/// <summary>[15:0] R:G:B:x 5:5:5:1 little endian</summary>
		RGBX5551 = 0x35315852,	// DRM_FORMAT_RGBX5551
		/// <summary>[15:0] B:G:R:x 5:5:5:1 little endian</summary>
		BGRX5551 = 0x35315842,	// DRM_FORMAT_BGRX5551
		/// <summary>[15:0] A:R:G:B 1:5:5:5 little endian</summary>
		ARGB1555 = 0x35315241,	// DRM_FORMAT_ARGB1555
		/// <summary>[15:0] A:B:G:R 1:5:5:5 little endian</summary>
		ABGR1555 = 0x35314241,	// DRM_FORMAT_ABGR1555
		/// <summary>[15:0] R:G:B:A 5:5:5:1 little endian</summary>
		RGBA5551 = 0x35314152,	// DRM_FORMAT_RGBA5551
		/// <summary>[15:0] B:G:R:A 5:5:5:1 little endian</summary>
		BGRA5551 = 0x35314142,	// DRM_FORMAT_BGRA5551
		/// <summary>[15:0] R:G:B 5:6:5 little endian</summary>
		RGB565 = 0x36314752,	// DRM_FORMAT_RGB565
		/// <summary>[15:0] B:G:R 5:6:5 little endian</summary>
		BGR565 = 0x36314742,	// DRM_FORMAT_BGR565
		/// <summary>[23:0] R:G:B little endian</summary>
		RGB888 = 0x34324752,	// DRM_FORMAT_RGB888
		/// <summary>[23:0] B:G:R little endian</summary>
		BGR888 = 0x34324742,	// DRM_FORMAT_BGR888
		/// <summary>[31:0] x:R:G:B 8:8:8:8 little endian</summary>
		XRGB8888 = 0x34325258,	// DRM_FORMAT_XRGB8888
		/// <summary>[31:0] x:B:G:R 8:8:8:8 little endian</summary>
		XBGR8888 = 0x34324258,	// DRM_FORMAT_XBGR8888
		/// <summary>[31:0] R:G:B:x 8:8:8:8 little endian</summary>
		RGBX8888 = 0x34325852,	// DRM_FORMAT_RGBX8888
		/// <summary>[31:0] B:G:R:x 8:8:8:8 little endian</summary>
		BGRX8888 = 0x34325842,	// DRM_FORMAT_BGRX8888
		/// <summary>[31:0] A:R:G:B 8:8:8:8 little endian</summary>
		ARGB8888 = 0x34325241,	// DRM_FORMAT_ARGB8888
		/// <summary>[31:0] A:B:G:R 8:8:8:8 little endian</summary>
		ABGR8888 = 0x34324241,	// DRM_FORMAT_ABGR8888
		/// <summary>[31:0] R:G:B:A 8:8:8:8 little endian</summary>
		RGBA8888 = 0x34324152,	// DRM_FORMAT_RGBA8888
		/// <summary>[31:0] B:G:R:A 8:8:8:8 little endian</summary>
		BGRA8888 = 0x34324142,	// DRM_FORMAT_BGRA8888
		/// <summary>[31:0] x:R:G:B 2:10:10:10 little endian</summary>
		XRGB2101010 = 0x30335258,	// DRM_FORMAT_XRGB2101010
		/// <summary>[31:0] x:B:G:R 2:10:10:10 little endian</summary>
		XBGR2101010 = 0x30334258,	// DRM_FORMAT_XBGR2101010
		/// <summary>[31:0] R:G:B:x 10:10:10:2 little endian</summary>
		RGBX1010102 = 0x30335852,	// DRM_FORMAT_RGBX1010102
		/// <summary>[31:0] B:G:R:x 10:10:10:2 little endian</summary>
		BGRX1010102 = 0x30335842,	// DRM_FORMAT_BGRX1010102
		/// <summary>[31:0] A:R:G:B 2:10:10:10 little endian</summary>
		ARGB2101010 = 0x30335241,	// DRM_FORMAT_ARGB2101010
		/// <summary>[31:0] A:B:G:R 2:10:10:10 little endian</summary>
		ABGR2101010 = 0x30334241,	// DRM_FORMAT_ABGR2101010
		/// <summary>[31:0] R:G:B:A 10:10:10:2 little endian</summary>
		RGBA1010102 = 0x30334152,	// DRM_FORMAT_RGBA1010102
		/// <summary>[31:0] B:G:R:A 10:10:10:2 little endian</summary>
		BGRA1010102 = 0x30334142,	// DRM_FORMAT_BGRA1010102
		/// <summary>[63:0] x:B:G:R 16:16:16:16 little endian</summary>
		XBGR16161616F = 0x48344258,	// DRM_FORMAT_XBGR16161616F
		/// <summary>[63:0] A:B:G:R 16:16:16:16 little endian</summary>
		ABGR16161616F = 0x48344241,	// DRM_FORMAT_ABGR16161616F
		/// <summary>[31:0] Cr0:Y1:Cb0:Y0 8:8:8:8 little endian</summary>
		YUYV = 0x56595559,	// DRM_FORMAT_YUYV
		/// <summary>[31:0] Cb0:Y1:Cr0:Y0 8:8:8:8 little endian</summary>
		YVYU = 0x55595659,	// DRM_FORMAT_YVYU
		/// <summary>[31:0] Y1:Cr0:Y0:Cb0 8:8:8:8 little endian</summary>
		UYVY = 0x59565955,	// DRM_FORMAT_UYVY
		/// <summary>[31:0] Y1:Cb0:Y0:Cr0 8:8:8:8 little endian</summary>
		VYUY = 0x59555956,	// DRM_FORMAT_VYUY
		/// <summary>[31:0] A:Y:Cb:Cr 8:8:8:8 little endian</summary>
		AYUV = 0x56555941,	// DRM_FORMAT_AYUV
		/// <summary>2x2 subsampled Cr:Cb plane</summary>
		NV12 = 0x3231564e,	// DRM_FORMAT_NV12
		/// <summary>2x2 subsampled Cb:Cr plane</summary>
		NV21 = 0x3132564e,	// DRM_FORMAT_NV21
		/// <summary>2x1 subsampled Cr:Cb plane</summary>
		NV16 = 0x3631564e,	// DRM_FORMAT_NV16
		/// <summary>2x1 subsampled Cb:Cr plane</summary>
		NV61 = 0x3136564e,	// DRM_FORMAT_NV61
		/// <summary>4x4 subsampled Cb (1) and Cr (2) planes</summary>
		YUV410 = 0x39565559,	// DRM_FORMAT_YUV410
		/// <summary>4x4 subsampled Cr (1) and Cb (2) planes</summary>
		YVU410 = 0x39555659,	// DRM_FORMAT_YVU410
		/// <summary>4x1 subsampled Cb (1) and Cr (2) planes</summary>
		YUV411 = 0x31315559,	// DRM_FORMAT_YUV411
		/// <summary>4x1 subsampled Cr (1) and Cb (2) planes</summary>
		YVU411 = 0x31315659,	// DRM_FORMAT_YVU411
		/// <summary>2x2 subsampled Cb (1) and Cr (2) planes</summary>
		YUV420 = 0x32315559,	// DRM_FORMAT_YUV420
		/// <summary>2x2 subsampled Cr (1) and Cb (2) planes</summary>
		YVU420 = 0x32315659,	// DRM_FORMAT_YVU420
		/// <summary>2x1 subsampled Cb (1) and Cr (2) planes</summary>
		YUV422 = 0x36315559,	// DRM_FORMAT_YUV422
		/// <summary>2x1 subsampled Cr (1) and Cb (2) planes</summary>
		YVU422 = 0x36315659,	// DRM_FORMAT_YVU422
		/// <summary>non-subsampled Cb (1) and Cr (2) planes</summary>
		YUV444 = 0x34325559,	// DRM_FORMAT_YUV444
		/// <summary>non-subsampled Cr (1) and Cb (2) planes</summary>
		YVU444 = 0x34325659,	// DRM_FORMAT_YVU444
	}
}