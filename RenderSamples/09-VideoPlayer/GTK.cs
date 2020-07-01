using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace RenderSamples.GTK
{
	/// <summary>Utility class to call ~30 GTK+ functions to implement an equivalent of GetOpenFileName WinAPI call</summary>
	static class OpenFileName
	{
		const string gtk = "gtk-x11-2.0";
		const string glib = "glib-2.0";
		const string gobject = "gobject-2.0";

		[DllImport( gtk )]
		static extern void gtk_init( ref int argc, ref IntPtr doublePointer );

		[DllImport( gtk )]
		static extern IntPtr gtk_file_chooser_dialog_new( [MarshalAs( UnmanagedType.LPUTF8Str )] string title,
			IntPtr parent, GtkFileChooserAction action,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string btn1, GtkResponseType resp1,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string btn2, GtkResponseType res2,
			IntPtr terminatingNull );

		[DllImport( gtk )]
		static extern void gtk_widget_destroy( IntPtr widget );

		[DllImport( gtk )]
		static extern UIntPtr gtk_dialog_get_type();
		[DllImport( gtk )]
		static extern UIntPtr gtk_file_chooser_get_type();
		[DllImport( gtk )]
		static extern IntPtr gtk_file_chooser_get_filename( IntPtr chooser );

		[DllImport( gtk )]
		static extern GtkResponseType gtk_dialog_run( IntPtr dialog );

		[DllImport( gtk )]
		static extern bool gtk_events_pending();
		[DllImport( gtk )]
		static extern bool gtk_main_iteration();

		[DllImport( glib )]
		static extern void g_free( IntPtr ptr );
		[DllImport( gobject )]
		static extern IntPtr g_object_ref_sink( IntPtr ptr );
		[DllImport( gobject )]
		static extern IntPtr g_object_unref( IntPtr ptr );
		[DllImport( gobject )]
		static extern IntPtr g_type_check_instance_cast( IntPtr instance, UIntPtr iface_type );

		enum GtkFileChooserAction: int
		{
			Open = 0,
			Save = 1,
			SelectFolder = 2,
			CreateFolder = 3,
		}

		enum GtkResponseType: int
		{
			None = -1,
			Reject = -2,
			Accept = -3,
			// If the dialog is deleted.
			DeleteEvent = -4,
			// These are returned from GTK dialogs, and you can also use them yourself if you like.
			OK = -5,
			Cancel = -6,
			Close = -7,
			Yes = -8,
			No = -9,
			Apply = -10,
			Help = -11
		}

		[DllImport( gtk )]
		static extern IntPtr gtk_file_filter_new();
		[DllImport( gtk )]
		static extern void gtk_file_filter_set_name( IntPtr filter, string name );
		[DllImport( gtk )]
		static extern void gtk_file_filter_add_mime_type( IntPtr filter, string mimeType );
		[DllImport( gtk )]
		static extern void gtk_file_filter_add_pattern( IntPtr filter, string pattern );
		[DllImport( gtk )]
		static extern void gtk_file_chooser_add_filter( IntPtr chooser, IntPtr filter );

		static void addFilter( IntPtr chooser, string name, IEnumerable<string> mimeTypes, IEnumerable<string> patterns )
		{
			IntPtr f = gtk_file_filter_new();
			gtk_file_filter_set_name( f, name );
			foreach( string mt in mimeTypes )
				gtk_file_filter_add_mime_type( f, mt );
			foreach( string p in patterns )
				gtk_file_filter_add_pattern( f, p );
			gtk_file_chooser_add_filter( chooser, f );
		}

		static void setupFilters( IntPtr chooser )
		{
			const string mp4Mime = "video/mp4";
			const string mkvMime = "video/x-matroska";

			addFilter( chooser, "All supported files",
				new string[] { mp4Mime, mkvMime },
				new string[] { "*.mp4", "*.mpeg4", "*.mkv" } );

			addFilter( chooser, "MPEG-4 part 12 files",
				new string[] { mp4Mime },
				new string[] { "*.mp4", "*.mpeg4" } );

			addFilter( chooser, "Matroska multimedia files",
				new string[] { mkvMime },
				new string[] { "*.mkv" } );
		}

		public static string getFileName()
		{
			byte[] cmdLine = File.ReadAllBytes( $"/proc/{ Process.GetCurrentProcess().Id }/cmdline" );
			Span<byte> span = stackalloc byte[ cmdLine.Length + 1 ];
			cmdLine.AsSpan().CopyTo( span );
			span[ cmdLine.Length ] = 0;

			unsafe
			{
				fixed ( byte* p = span )
				{
					IntPtr pointer = (IntPtr)p;
					IntPtr doublePointer = (IntPtr)( &pointer );
					int argc = 1;
					gtk_init( ref argc, ref doublePointer );
				}
			}

			// https://developer.gnome.org/gtk3/stable/GtkFileChooserDialog.html#gtk-file-chooser-dialog-new
			IntPtr dialog = gtk_file_chooser_dialog_new( "Pick a media file to play",
									  IntPtr.Zero,
									  GtkFileChooserAction.Open,
									  "_Cancel",
									  GtkResponseType.Cancel,
									  "_Open",
									  GtkResponseType.Accept,
									  IntPtr.Zero );

			if( IntPtr.Zero == dialog )
			{
				// Console.WriteLine( "gtk_file_chooser_dialog_new failed" );
				return null;
			}

			g_object_ref_sink( dialog );

			try
			{
				IntPtr chooser = g_type_check_instance_cast( dialog, gtk_file_chooser_get_type() );
				setupFilters( chooser );

				GtkResponseType res = gtk_dialog_run( dialog );
				if( res != GtkResponseType.Accept )
					return null;
				IntPtr u8 = gtk_file_chooser_get_filename( chooser );
				string str = Marshal.PtrToStringUTF8( u8 );
				return str;
			}
			finally
			{
				gtk_widget_destroy( dialog );
				g_object_unref( dialog );

				// GTK's version of DestroyWindow is asynchronous.
				// Unless dispatching some final messages, the window stays on the screen in some weird state.
				// Attempting to close brings a message from Linux telling me my software is broken, with a helpful option to kill the complete process.
				while( gtk_events_pending() )
					gtk_main_iteration();
			}
		}
	}
}