using System;
namespace SmartFairMVP
{
	public class Coordinates
	{
		public double x, y;

		public Coordinates()
		{
			x = y = 0;
		}

		public Coordinates(double xAxis, double yAxis)
		{
			x = xAxis;
			y = yAxis;
		}

		public override string ToString()
		{
			return string.Format("(" + x + ", " + y + ")");
		}
	}
}
