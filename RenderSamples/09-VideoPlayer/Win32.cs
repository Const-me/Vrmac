using System;
using System.Runtime.InteropServices;

namespace RenderSamples.Win32
{
	/// <summary>Utility class to call a single GetOpenFileNameW WinAPI function</summary>
	static class OpenFileName
	{
		[DllImport( "Comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern bool GetOpenFileNameW( [In] ref OFN ofn );

		[Flags]
		enum eFlags: int
		{
			Explorer = 0x00080000,
			FileMustExist = 0x00001000,
			PathMustExist = 0x00000800,
		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		struct OFN
		{
			public int lStructSize;
			public IntPtr hwndOwner;
			public IntPtr hInstance;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpstrFilter;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpstrCustomFilter;
			public int nMaxCustFilter;
			public int nFilterIndex;
			public IntPtr lpstrFile;
			public int nMaxFile;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpstrFileTitle;
			public int nMaxFileTitle;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpstrInitialDir;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpstrTitle;
			public eFlags Flags;
			public short nFileOffset;
			public short nFileExtension;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpstrDefExt;
			public IntPtr lCustData;
			public IntPtr lpfnHook;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpTemplateName;
			public IntPtr pvReserved;
			public int dwReserved;
			public int flagsEx;
		}

		const int MAX_PATH = 260;

		public static string getFileName()
		{
			OFN ofn = new OFN()
			{
				lStructSize = Marshal.SizeOf<OFN>(),
				lpstrFilter = "All supported files\0*.mp4;*.mpeg4;*.mkv\0MPEG-4 part 12 files\0*.mp4;*.mpeg4\0Matroska multimedia files\0*.mkv\0\0",
				lpstrTitle = "Pick a media file to play",
				Flags = eFlags.Explorer | eFlags.FileMustExist | eFlags.PathMustExist,
			};
			ofn.nMaxFile = MAX_PATH;

			Span<char> buffer = stackalloc char[ MAX_PATH + 1 ];
			buffer[ MAX_PATH ] = '\0';

			unsafe
			{
				fixed ( char* ptr = buffer )
				{
					ofn.lpstrFile = (IntPtr)ptr;
					bool picked = GetOpenFileNameW( ref ofn );
					if( !picked )
						return null;
				}
			}
			int length = buffer.IndexOf( '\0' );
			return new string( buffer.Slice( 0, length ) );
		}
	}
}