using System;
using System.Collections.Generic;
using EstimoteSdk;
using Android.Widget;
using Android.Util;

namespace SmartFairMVP.Droid
{
	public class RangingListener : Java.Lang.Object, BeaconManager.IRangingListener
	{
		static readonly string Tag = typeof(RangingListener).FullName;

		public void OnBeaconsDiscovered(Region region, IList<Beacon> beacons)
		{
			Log.Debug(Tag, "Found {0} beacons", beacons.Count);
			foreach (Beacon b in beacons)
			{
				//Toast.MakeText(this., "Region exited " + uuidExited.ToString(), ToastLength.Short).Show();
				System.Console.WriteLine(Tag + " - Beacon encontrado: " + b.ProximityUUID + 
				                         " Major: " + b.Major + " minor: " + b.Minor);

				System.Console.WriteLine(Tag + " - Distancia: " + Utils.ComputeAccuracy(b));
			}

		}
	}
}