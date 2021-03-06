﻿// This source file is generated by a tool.
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member

namespace Vrmac.Input.Linux
{
	/// <summary>Absolute axis of an input device</summary>
	public enum eAbsoluteAxis: byte
	{
		X = 0x00,
		Y = 0x01,
		Z = 0x02,
		Rx = 0x03,
		Ry = 0x04,
		Rz = 0x05,
		Throttle = 0x06,
		Rudder = 0x07,
		Wheel = 0x08,
		Gas = 0x09,
		Brake = 0x0a,
		Hat0x = 0x10,
		Hat0y = 0x11,
		Hat1x = 0x12,
		Hat1y = 0x13,
		Hat2x = 0x14,
		Hat2y = 0x15,
		Hat3x = 0x16,
		Hat3y = 0x17,
		Pressure = 0x18,
		Distance = 0x19,
		TiltX = 0x1a,
		TiltY = 0x1b,
		ToolWidth = 0x1c,
		Volume = 0x20,
		Misc = 0x28,
		/// <summary>MT slot being modified</summary>
		MultiTouchSlot = 0x2f,
		/// <summary>Major axis of touching ellipse</summary>
		MultiTouchTouchMajor = 0x30,
		/// <summary>Minor axis (omit if circular)</summary>
		MultiTouchTouchMinor = 0x31,
		/// <summary>Major axis of approaching ellipse</summary>
		MultiTouchWidthMajor = 0x32,
		/// <summary>Minor axis (omit if circular)</summary>
		MultiTouchWidthMinor = 0x33,
		/// <summary>Ellipse orientation</summary>
		MultiTouchOrientation = 0x34,
		/// <summary>Center X touch position</summary>
		MultiTouchPositionX = 0x35,
		/// <summary>Center Y touch position</summary>
		MultiTouchPositionY = 0x36,
		/// <summary>Type of touching device</summary>
		MultiTouchToolType = 0x37,
		/// <summary>Group a set of packets as a blob</summary>
		MultiTouchBlobId = 0x38,
		/// <summary>Unique ID of initiated contact</summary>
		MultiTouchTrackingId = 0x39,
		/// <summary>Pressure on contact area</summary>
		MultiTouchPressure = 0x3a,
		/// <summary>Contact hover distance</summary>
		MultiTouchDistance = 0x3b,
		/// <summary>Center X tool position</summary>
		MultiTouchToolX = 0x3c,
		/// <summary>Center Y tool position</summary>
		MultiTouchToolY = 0x3d,
	}
}