// g++ sizeofs.cpp -o sizeofs
// ./sizeofs
#include <stdio.h>
#include <linux/videodev2.h>
#include <fdk-aac/aacdecoder_lib.h>

int main()
{
	printf( "v4l2_buffer\t%i\n", (int)sizeof( v4l2_buffer ) );
	printf( "v4l2_capability\t%i\n", (int)sizeof( v4l2_capability ) );
	printf( "v4l2_create_buffers\t%i\n", (int)sizeof( v4l2_create_buffers ) );
	printf( "v4l2_decoder_cmd\t%i\n", (int)sizeof( v4l2_decoder_cmd ) );
	printf( "v4l2_event\t%i\n", (int)sizeof( v4l2_event ) );
	printf( "v4l2_event_subscription\t%i\n", (int)sizeof( v4l2_event_subscription ) );
	printf( "v4l2_exportbuffer\t%i\n", (int)sizeof( v4l2_exportbuffer ) );
	printf( "v4l2_frmsizeenum\t%i\n", (int)sizeof( v4l2_frmsizeenum ) );
	printf( "v4l2_frmsize_stepwise\t%i\n", (int)sizeof( v4l2_frmsize_stepwise ) );
	printf( "v4l2_fmtdesc\t%i\n", (int)sizeof( v4l2_fmtdesc ) );
	printf( "v4l2_pix_format\t%i\n", (int)sizeof( v4l2_pix_format ) );
	printf( "v4l2_plane_pix_format\t%i\n", (int)sizeof( v4l2_plane_pix_format ) );
	printf( "v4l2_pix_format_mplane\t%i\n", (int)sizeof( v4l2_pix_format_mplane ) );
	printf( "v4l2_plane\t%i\n", (int)sizeof( v4l2_plane ) );
	printf( "v4l2_requestbuffers\t%i\n", (int)sizeof( v4l2_requestbuffers ) );
	printf( "v4l2_selection\t%i\n", (int)sizeof( v4l2_selection ) );
	printf( "v4l2_format\t%i\n", (int)sizeof( v4l2_format ) );
	printf( "v4l2_timecode\t%i\n", (int)sizeof( v4l2_timecode ) );
	return 0;
}