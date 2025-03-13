using UnityEngine;
using System;

public class GPStoUTM : MonoBehaviour
{
    [HideInInspector]
    public double res_X = 0.0, res_Z = 0.0;
    
    [HideInInspector]
    public float[] result_position = new float[2];


    // Llamado al instanciarse el item
    public float[] calculateUnityPosition(double latitude_1, double longitude_1)
    {
        double latitude_0 = CoordinatesOffsetController.latitude, longitude_0 = CoordinatesOffsetController.longitude; //Center coords

        //Convert to radians
        latitude_0 = (Math.PI / 180) * latitude_0;
        longitude_0 = (Math.PI / 180) * longitude_0;
        latitude_1 = (Math.PI / 180) * latitude_1;
        longitude_1 = (Math.PI / 180) * longitude_1;

        //Origin coords
        double[] res_0 = calculateUTM(latitude_0, longitude_0);

        //Desired coords
        double[] res_1 = calculateUTM(latitude_1, longitude_1);

        //Difference between coords = coords displacement = Unity's coords
        res_X = res_0[0] - res_1[0];
        res_Z = res_0[1] - res_1[1];

        //Debug.Log("Result: " + "X = " + res_X + ", Z = " + res_Z);

        result_position[0] = (float) res_X;
        result_position[1] = (float) res_Z;

        return result_position;
    }

    public double[] calculateUTM(double latitude, double longitude){
        
        double[] result = new double[2];
        double S, K_1, K_2, K_3, K_4, K_5, X, Z;

       //Constants
        const double a = 6378137.0, b = 6356752.3142; //WGS84 -> a = Equatorial Radius (meters); b = Polar Radius (meters)
        const double K_0 = 0.9996; //scale along longitude_0, it's a constant
        double longitude_0 = Mathf.Deg2Rad * -3; // central meridian of UTM zone 30N = -3º = 3W, http://www.jaworski.ca/utmzones.htm
        double p = longitude - longitude_0; // offset from the central meridian of our zone
        double e = Math.Sqrt(1.0 - Math.Pow(b,2) / Math.Pow(a,2)); // eccentricity of the earth’s elliptical cross-section
        double e_prime = Math.Pow((e*a/b),2); //  quantity e' only occurs in even powers so it need only be calculated as e'^2
        double nu = a /  Math.Pow((1.0 - Math.Pow(e,2) * Math.Pow(Math.Sin(latitude),2)),(1/2)); //  radius of curvature of the earth perpendicular to the meridian plane. It is also the distance from the point in question to the polar axis, measured perpendicular to the earth’s surface

        //Meridional Arc
        double i_1, i_2, i_3, i_4; // Calculating the arc length of an ellipse involves functions called elliptic integrals, which don’t reduce to neat closed formulas. So they have to be represented as series

        i_1 = 1 - Math.Pow(e,2) / 4 - 3 * Math.Pow(e,4) / 64 - 5 * Math.Pow(e,6) / 256;
        i_2 = 3 * Math.Pow(e,2) / 8 + 3 * Math.Pow(e,4) / 32 + 45 * Math.Pow(e,6) / 1024;
        i_3 = 15 * Math.Pow(e,4) / 256 + 45 * Math.Pow(e,6) / 1024;
        i_4 = 35 * Math.Pow(e,6) / 3072;

        S = a * (i_1 * latitude - i_2 * Math.Sin(2 * latitude) + i_3 * Math.Sin(4 * latitude) - i_4 * Math.Sin(6 * latitude)); //USGS format

        //Northing = K_1 + K_2*p^2 + K_3*p^4
        K_1 = S * K_0;
        K_2 = K_0 * nu * Math.Sin(latitude) * Math.Cos(latitude) / 2;
        K_3 = (K_0 * nu * Math.Sin(latitude) * Math.Pow(Math.Cos(latitude) / 24,3)) * ((5 - Math.Pow(Math.Tan(latitude),2) + 9 * e_prime));

        Z = K_1 + K_2 * Math.Pow(p,2) + K_3 * Math.Pow(p,4);

        //Easting = K_4*p + K_5*p^3
        K_4 = K_0 * nu * Math.Cos(latitude);
        K_5  = (K_0 * nu * Math.Pow(Math.Cos(latitude),3) / 6) * (1.0 - Math.Pow(Math.Tan(latitude),2) + e_prime * Math.Pow(Math.Cos(latitude),2));

        X = K_4 * p + K_5 * Math.Pow(p,3);

        //Fill result
        result[0] = X;
        result[1] = Z;

        //Return result
        return result;
    }

}
