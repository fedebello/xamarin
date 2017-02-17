using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;

namespace SmartFairMVP
{
	public class BeaconSet
	{
		static readonly string Tag = typeof(BeaconSet).FullName;

		public Hashtable beacons = new Hashtable();
		public SortedDictionary<double, string> distances = new SortedDictionary<double, string>();

		public BeaconSet() {
			//loadBeaconsInfo();
		}

		//Carga inicial de beacons, esto se deberia sacar de la BD (xls?) ...
		private void loadBeaconsInfo()
		{   //(0,0) pared con enchufe lampara -> x: hacia baño, y: hacia calle
			beacons.Add("62438:46841", new BeaconInfo("izq arriba",	0.2, 	6));  	    //VQ2i (barra)
			beacons.Add("13797:46308", new BeaconInfo("der abajo",	2, 		4));    	//v3gt (ventana)
			beacons.Add("61356:40626", new BeaconInfo("der arriba", 2, 		6.5));		//CVdx (tele)
		}

		public bool addBeacon(string key, string desc, double x, double y)
		{
			if (beacons == null)
				return false;
			
			beacons.Add(key, new BeaconInfo(desc, x, y));
			return true;
		}

		public BeaconInfo updateBeacon(NSUuid uuid, NSNumber major, NSNumber minor, double distance)
		{
			System.Console.WriteLine(Tag + "add beacon");

			String key = major + ":" + minor;
			BeaconInfo beaconInfo = null;

			// si existe, actualizo
			if (beacons.Contains(key)) {	
				beaconInfo = (BeaconInfo)beacons[key];

				// saco distancia vieja
				if (distances.ContainsValue(key))
					distances.Remove(beaconInfo.distance);
				
				beaconInfo.addDistance(distance);
			} //sino, agrego
			else {
				beaconInfo = new BeaconInfo();
				beaconInfo.uuid = uuid.AsString();
				beaconInfo.major = major;
				beaconInfo.minor = minor;
				beaconInfo.addDistance(distance);

				beacons.Add(key, beaconInfo);
			}

			//actualizo distancias
			try {
				distances.Add(beaconInfo.distance, key);
			} catch (ArgumentException e) {
				//TODO que hacemos? le sumamos un cm y la dibujamos?
				System.Console.WriteLine(Tag + "add beacon -> clave duplicada en distancias");
				System.Console.WriteLine(Tag + e.Message);
			}
			return beaconInfo;
		}

		public Coordinates calculatePosition()
		{
			BeaconInfo [] beaconsOrdered = new BeaconInfo[4];

			//busca las 4 distancias menores, calcula distancia cada 3 beacons y hace promedio.
			var top4DistancesKeys = distances.Values;

			int pos = 0;
			foreach (string beaconKey in top4DistancesKeys) {
				beaconsOrdered[pos++] = (BeaconInfo)beacons[beaconKey];
				System.Console.WriteLine(Tag + " Beacon used: " + beaconKey + " distancia: " + beaconsOrdered[pos - 1].distance);
				if (pos > 4) break;
			}

			// sumo coordenadas temporales 
			Coordinates coord = new Coordinates();
			int total = 0;
			for (int i = 0; i < 4; i++) {
				Coordinates tmp = calculateDistanceTrilateration(beaconsOrdered[i % 4], beaconsOrdered[(i + 1) % 4], beaconsOrdered[(i + 2) % 4]);
				if (!Double.IsNaN(tmp.x) && !Double.IsNaN(tmp.y)) {
					coord.x += tmp.x;
					coord.y += tmp.y;
					total++;
				}
			}

			//hago promedio
			if (total > 0) {
				coord.x /= total;
				coord.y /= total;
			} // si no hay temporales devuelvo NaN
			else {
				coord.x = Double.NaN;
				coord.y = Double.NaN;
			}

			System.Console.WriteLine(Tag + "Posicion calculada: (" + coord.x + ", " + coord.y + ")");
			return coord;
		}

		private Coordinates calculateDistanceTrilateration(BeaconInfo beaconA, BeaconInfo beaconB, BeaconInfo beaconC)
		{
			if (beaconA == null || beaconB == null || beaconC == null)
				return new Coordinates(Double.NaN, Double.NaN);
			
			double latA = beaconA.position.x;
			double lonA = beaconA.position.y;
			double disA = beaconA.distance;

			double latB = beaconB.position.x;
			double lonB = beaconB.position.y;
			double disB = beaconB.distance;

			double latC = beaconC.position.x;
			double lonC = beaconC.position.y;
			double disC = beaconC.distance;

			double[] P1 = { lonA, latA, 0 };
			double[] P2 = { lonB, latB, 0 };
			double[] P3 = { lonC, latC, 0 };

			double[] ex = { 0, 0, 0 };
			double P2P1 = 0;

			for (int i = 0; i < 3; i++)
			{
				P2P1 += Math.Pow(P2[i] - P1[i], 2);
			}

			for (int i = 0; i < 3; i++)
			{
				ex[i] = (P2[i] - P1[i]) / Math.Sqrt(P2P1);
			}

			double[] p3p1 = { 0, 0, 0 };
			for (int i = 0; i < 3; i++)
			{
				p3p1[i] = P3[i] - P1[i];
			}

			double ivar = 0;
			for (int i = 0; i < 3; i++)
			{
				ivar += (ex[i] * p3p1[i]);
			}

			double p3p1i = 0;
			for (int i = 0; i < 3; i++)
			{
				p3p1i += Math.Pow(P3[i] - P1[i] - ex[i] * ivar, 2);
			}

			double[] ey = { 0, 0, 0 };
			for (int i = 0; i < 3; i++)
			{
				ey[i] = (P3[i] - P1[i] - ex[i] * ivar) / Math.Sqrt(p3p1i);
			}

			double[] ez = { 0, 0, 0 };
			// if 2-dimensional vector then ez = 0

			double d = Math.Sqrt(P2P1);

			double jvar = 0;
			for (int i = 0; i < 3; i++)
			{
				jvar += (ey[i] * p3p1[i]);
			}

			// from wikipedia
			// plug and chug using above values
			double x = (Math.Pow(disA, 2) - Math.Pow(disB, 2) + Math.Pow(d, 2)) / (2 * d);
			double y = ((Math.Pow(disA, 2) - Math.Pow(disC, 2) + Math.Pow(ivar, 2) + Math.Pow(jvar, 2)) / (2 * jvar)) - ((ivar / jvar) * x);

			// only one case shown here
			double z = Math.Sqrt(Math.Pow(disA, 2) - Math.Pow(x, 2) - Math.Pow(y, 2));

			if (Double.IsNaN(z))
			{
				z = 0;
			}

			// triPt is an array with ECEF x,y,z of trilateration point
			// triPt = P1 + x*ex + y*ey + z*ez
			double[] triPt = { 0, 0, 0 };

			for (int i = 0; i < 3; i++)
			{
				triPt[i] = P1[i] + ex[i] * x + ey[i] * y + ez[i] * z;
			}

			// convert back to lat/long from ECEF
			// convert to degrees
			double xPhone = triPt[0];
			double yPhone = triPt[1];
			System.Console.WriteLine(Tag + " Trilateration - x= " + xPhone + " y=" + yPhone);

			return new Coordinates(xPhone, yPhone);
		}
	}
}
