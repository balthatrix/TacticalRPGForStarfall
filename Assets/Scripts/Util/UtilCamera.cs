using System;
using UnityEngine;

namespace Util
{
	public class CameraUtil
	{

		public static float UnitsPerPixel(Camera cam) {
			var p1 = cam.ScreenToWorldPoint(Vector3.zero);
			var p2 = cam.ScreenToWorldPoint(Vector3.right);
			return Vector3.Distance(p1, p2);
		}

		public static float PixelsPerUnit(Camera cam) {
			return 1/UnitsPerPixel(cam);
		}
	}
}

