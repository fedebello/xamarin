using System;
using System.Collections.Generic;
using Android.Util;
using EstimoteSdk;
using EstimoteSdk.EddystoneSdk;

namespace SmartFairMVP.Droid
{
	public class EddystoneListener : Java.Lang.Object, BeaconManager.IEddystoneListener
	{
		static readonly string Tag = typeof(EddystoneListener).FullName;

		public void OnEddystonesFound(IList<Eddystone> eddystones)
		{
			Log.Debug(Tag, "Found {0} eddystones", eddystones.Count);
			foreach (Eddystone b in eddystones)
			{
				//Toast.MakeText(this., "Region exited " + uuidExited.ToString(), ToastLength.Short).Show();
				System.Console.WriteLine(Tag + " - Eddystone encontrado: MAC=" + b.MacAddress);
				System.Console.WriteLine(Tag + " - Distancia: " + Utils.ComputeAccuracy(b));
			}
		}
	}
}
