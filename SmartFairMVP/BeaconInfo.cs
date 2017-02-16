using System.Collections.Generic;

using Foundation;

namespace SmartFairMVP
{
	public class BeaconInfo : NSObject
	{
		public string id { get; set; }
		public Coordinates position;
		public string uuid { get; set; }
		public NSNumber major { get; set;}
		public NSNumber minor { get; set; }
		private List<double> distances;
		public double distance { get; set;}

		private readonly static int maxSize = 5;

		public BeaconInfo()
		{
			distances = new List<double>();
			distance = 0;
		}

		public BeaconInfo(string name, double posX, double posY)
		{
			distances = new List<double>();
			distance = 0;

			position = new Coordinates (posX, posY);
			id = name;
		}

		public void addDistance(double measure)
		{
			if (distances.Count == maxSize)
				distances.Remove(distances[0]);

			distances.Add(measure);
			calculateDistance();
		}

		private void calculateDistance()
		{
			double total = 0;
			foreach (double d in distances) {
				total += d;
			}

			distance = total / distances.Count;
		}
	}
}
