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

		public Coordinates normalize(nfloat alto, nfloat ancho)
		{
			// (0,0) para xamarin es arriba a la izq, para la imagen abajo a la izq y rotado
			return new Coordinates(x/6 * ancho, alto - (y/11 * alto));
		}

		public Coordinates normalizeInverted(nfloat alto, nfloat ancho)
		{
			// mando frutas para los bordes
			if (x > 6)
				x = 6;

			if (y > 11)
				y = 11;

			if (x < 0)
				x = 0;

			if (y < 0)
				y = 0;

			return normalize(alto, ancho);
		}
	}
}
