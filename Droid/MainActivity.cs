using Android.App;
using Android.Widget;
using Android.OS;

using EstimoteSdk;

using System.Collections.Generic;

namespace SmartFairMVP.Droid
{
	[Activity(Label = "SmartFair", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity, BeaconManager.IServiceReadyCallback
	{
		static readonly string Tag = typeof(MainActivity).FullName;

		int count = 1;
		BeaconManager _beaconManager;
		RangingListener _rangingListener;
		Region _region;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			//nombre boton xa arrancar -> scanButton

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };



			/******* Beacons!!! - Estimote test **********/
			//Button beaconBtn = FindViewById<Button>(Resource.Id.scanButton);
			//_rangingListener = new RangingListener();

			// Create beacon manager
			_beaconManager = new BeaconManager(this);
			_region = new Region("region mvp", "F7826DA6-4FA2-4E98-8024-BC5B71E0893E");

			// Default values are 5s of scanning and 25s of waiting time to save CPU cycles.
			// In order for this demo to be more responsive and immediate we lower down those values.
			//_beaconManager.SetBackgroundScanPeriod(5, 25);
			_beaconManager.SetRangingListener(new RangingListener());
			_beaconManager.EnteredRegion += (sender, e) =>
			{// Do something as the device has entered in region for the Estimote.

				IList<Beacon> beaconsFound = e.Beacons;
				foreach (Beacon b in beaconsFound) {
					string text = b.ProximityUUID + "//" + b.Major + "//" + b.Minor;
					System.Console.WriteLine(Tag + "Beacon found: " + text);
				}
			};

			_beaconManager.ExitedRegion += (sender, e) =>
			{// Do something as the device has left the region for the Estimote.           
				var uuidExited = e.P0.ProximityUUID;
				System.Console.WriteLine(Tag + "Region exited " + uuidExited.ToString());
			};

			_beaconManager.SetEddystoneListener(new EddystoneListener());
		}

		protected override void OnResume()
		{//Connect to beacon manager to start scanning
			base.OnResume();
			_beaconManager.Connect(this);
		}

		public void OnServiceReady()
		{// This method is called when BeaconManager is up and running.
			_beaconManager.StartMonitoring(_region);
			_beaconManager.StartRanging(_region);
			_beaconManager.StartEddystoneScanning();
		}

		protected override void OnDestroy()
		{// Make sure we disconnect from the Estimote.
			_beaconManager.StopRanging(_region);
			_beaconManager.StopMonitoring(_region);
			_beaconManager.Disconnect();
			base.OnDestroy();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

	}

}

