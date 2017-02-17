using System;

using UIKit;
using Estimote;
using CoreLocation;
using CoreBluetooth;
using Foundation;
using System.Text;

using CoreFoundation;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace SmartFairMVP.iOS
{
	public partial class ViewController : UIViewController
	{
		static readonly string Tag = typeof(ViewController).FullName;

		// "F7826DA6-4FA2-4E98-8024-BC5B71E0893E" "f7826da6bc5b71e0893e" "f7826da6-4fa2-4e98-8024-bc5b71e0893e"
		const string PROXIMITY_UUID = "f7826da6-4fa2-4e98-8024-bc5b71e0893e";

		BeaconManager beaconManager;
		CLBeaconRegion region;
		BeaconSet beacons;

		nfloat dx = 0;
		nfloat dy = 0;
		/*PointF pointA = new PointF(95.5f, 585.5f); // Beacon arriba Repisa
		PointF pointB = new PointF(127.5f, 169.5f); // Beacon arriba mesa (esquina superior izquierda)
		PointF pointC = new PointF(269f, 190f); // Beacon arriba recibidor (esquina superior derecha)*/
		UIPanGestureRecognizer panGesture;

		public ViewController(IntPtr ptr) : base(ptr) { }

		public static NSUuid BeaconUUID
		{
			//Virtual
			get { return new NSUuid(PROXIMITY_UUID); }
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.Title = "Select Beacon";

			beaconManager = new BeaconManager();
			beaconManager.ReturnAllRangedBeaconsAtOnce = true;
			beaconManager.RequestAlwaysAuthorization();
			beaconManager.AuthorizationStatusChanged += BeaconManagerAuthorizationStatusChanged;
			beaconManager.RangedBeacons += BeaconManagerRangedBeacons;
			beaconManager.EnteredRegion += BeaconManagerEnteredRegion;
			beaconManager.ExitedRegion += BeaconManager_ExitedRegion;

			beacons = new BeaconSet();
			beacons.addBeacon("62438:46841","izq arriba", 0.2, 7);	//VQ2i (barra)
			beacons.addBeacon("13797:46308","der abajo", 2, 5);	//v3gt (ventana)
			beacons.addBeacon("61356:40626", "der arriba", 2.5, 8);	//CVdx (tele)

			/*** 
			 * agrego lo de tincho
			 ***/

			// imagen de fondo
			UIGraphics.BeginImageContext(this.View.Frame.Size);
			UIImage i = UIImage.FromFile("mapa fede.png"); //822 × 1418
			i = i.Scale(this.View.Frame.Size);
			this.View.BackgroundColor = UIColor.FromPatternImage(i);

			//Creo una imagen en la posicion 100,100 con un tamaño de 50 x 50
			var imageView = new UIImageView(new RectangleF(100, 100, 40, 40));
			imageView.Image = UIImage.FromFile("pin.png");
			imageView.UserInteractionEnabled = true;
			imageView.Center = new PointF(100, 100);
			this.View.AddSubview(imageView);

			//Creo imagen para beacon
			foreach (BeaconInfo bi in beacons.beacons.Values)
			{
				Coordinates cb = bi.position.normalize(this.View.Frame.Size.Height, this.View.Frame.Size.Width);
				var imageBeacon = new UIImageView(new RectangleF(Convert.ToSingle(cb.x), Convert.ToSingle(cb.y), 25, 25));
				imageBeacon.Image = UIImage.FromFile("kontaktio-beacon.png");
				imageBeacon.UserInteractionEnabled = false;
				imageBeacon.Center = new PointF(Convert.ToSingle(cb.x), Convert.ToSingle(cb.y));
				this.View.AddSubview(imageBeacon);
			}

			RandomButton.AccessibilityIdentifier = "myRandomButton";
			RandomButton.TouchUpInside += delegate
			{
				imageView.Center = new PointF(new Random().Next(0, (int)this.View.Frame.Right),
											 new Random().Next(0, (int)this.View.Frame.Bottom));
				PositionLabel.Text = string.Format("Pos: <{0},{1}>", imageView.Center.X, imageView.Center.Y);

			};

			ResetButton.AccessibilityIdentifier = "myResetButton";
			ResetButton.TouchUpInside += delegate
			{
				var title = string.Format("Ubicar persona");
				ResetButton.SetTitle(title, UIControlState.Normal);
				Coordinates cell = beacons.calculatePosition();
				cell = cell.normalizeInverted(this.View.Frame.Size.Height, this.View.Frame.Size.Width);
				imageView.Center = new PointF(Convert.ToSingle(cell.x), Convert.ToSingle(cell.y));
				PositionLabel.Text = string.Format("Pos: <{0},{1}>", imageView.Center.X, imageView.Center.Y);


			};

			panGesture = new UIPanGestureRecognizer(() =>
			{
				if ((panGesture.State == UIGestureRecognizerState.Began || panGesture.State == UIGestureRecognizerState.Changed) && (panGesture.NumberOfTouches == 1))
				{

					var p0 = panGesture.LocationInView(View);

					if (dx == 0)
						dx = p0.X - imageView.Center.X;

					if (dy == 0)
						dy = p0.Y - imageView.Center.Y;

					var p1 = new PointF((float)p0.X - (float)dx, (float)p0.Y - (float)dy);

					imageView.Center = p1;
					PositionLabel.Text = string.Format("Pos: <{0},{1}>", imageView.Center.X, imageView.Center.Y);
				}
				else if (panGesture.State == UIGestureRecognizerState.Ended)
				{
					dx = 0;
					dy = 0;
				}
			});

			imageView.AddGestureRecognizer(panGesture);
		}

		void BeaconManagerAuthorizationStatusChanged(object sender, AuthorizationStatusChangedEventArgs e)
		{
			System.Console.WriteLine(Tag + " Status: {0}", e.Status);

			if (e.Status == CLAuthorizationStatus.Authorized)
			{
				region = new CLBeaconRegion(BeaconUUID, "BeaconSample")
				{
					NotifyEntryStateOnDisplay = true,
					NotifyOnEntry = true,
					NotifyOnExit = true
				};

				// Comenzar scanning...
				beaconManager.StartMonitoringForRegion(region);
				beaconManager.StartRangingBeaconsInRegion(region);
			}
		}

		void BeaconManagerRangedBeacons(object sender, RangedBeaconsEventArgs e)
		{
			System.Console.WriteLine(Tag + " iBeacon ranged: " + e.Beacons.Length);

			foreach (CLBeacon b in e.Beacons) {
				beacons.updateBeacon(b.ProximityUuid, b.Major, b.Minor, b.Accuracy);
			}
			//TODO sacar de aca...
			beacons.calculatePosition();
		}

		void BeaconManagerEnteredRegion(object sender, EnteredRegionEventArgs e)
		{
			System.Console.WriteLine(Tag + " Entered region: " + e.Region.ProximityUuid);
		}

		void BeaconManager_ExitedRegion(object sender, ExitedRegionEventArgs e)
		{
			System.Console.WriteLine(Tag + " Exited region: " + e.Region.ProximityUuid);
		}

	}
}
