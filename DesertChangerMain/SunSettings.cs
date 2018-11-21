using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace DesertChangerMain
{
    public class SunSettings
    {
        // General
        /*The calculations in the NOAA Sunrise/Sunset and Solar Position Calculators are based on equations 
          from Astronomical Algorithms, by Jean Meeus.
          The sunrise and sunset results are theoretically accurate to within a minute for locations between
           +/- 72° latitude, and within 10 minutes outside of those latitudes.However, due to variations in 
           atmospheric composition, temperature, pressure and conditions, observed values may vary from calculations. 

      */

        private System.Device.Location.GeoCoordinate GeoCord;
        public int TimeZoneOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;


        public SunSettings(System.Device.Location.GeoCoordinate location)
        {
            GeoCord = location;
        }


       public double JulieanDay => DateTime.Now.Date.ToOADate() + 2415018.5 + 0 - TimeZoneOffset / 24;

        public double JulieanCent => (JulieanDay - 2451545) / 36525;

        public double GeomMeanLongSun => MOD1(280.46646 + JulieanCent * (36000.76983 + JulieanCent * 0.0003032), 360);
   
        public double GeomMeanAnomSun => 357.52911 + JulieanCent * (35999.05029 - 0.0001537 * JulieanCent);

        public double EccentEarthOrbit => 0.016708634-JulieanCent*(0.000042037+0.0000001267*JulieanCent);

        public double SunEqofCtr => Math.Sin(RADIANS(GeomMeanAnomSun)) * (1.914602 - JulieanCent * (0.004817 + 0.000014 * JulieanCent)) + Math.Sin(RADIANS(2 * GeomMeanAnomSun)) * (0.019993 - 0.000101 * JulieanCent) + Math.Sin(RADIANS(3 * GeomMeanAnomSun)) * 0.000289;

        public double SunTrueLong => GeomMeanLongSun + SunEqofCtr;
 
        public double SunTrueAnom => GeomMeanAnomSun + SunEqofCtr;

        public double SunRadVector => (1.000001018 * (1 - EccentEarthOrbit * EccentEarthOrbit)) / (1 + EccentEarthOrbit * Math.Cos(RADIANS(SunTrueAnom)));

        public double SunAppLong => SunTrueLong - 0.00569 - 0.00478 * Math.Sin(RADIANS(125.04 - 1934.136 * JulieanCent));

        public double MeanObliqEcliptic => 23 + (26 + ((21.448 - JulieanCent * (46.815 + JulieanCent * (0.00059 - JulieanCent * 0.001813)))) / 60) / 60;

        public double ObliqCorr => MeanObliqEcliptic + 0.00256 * Math.Cos(RADIANS(125.04 - 1934.136 * JulieanCent));

        public double SunRtAscen => DEGREES(Math.Atan2(Math.Cos(RADIANS(SunAppLong)), Math.Cos(RADIANS(ObliqCorr)) * Math.Sin(RADIANS(SunAppLong))));

        public double SunDeclin => DEGREES(Math.Asin(Math.Sin(RADIANS(ObliqCorr)) * Math.Sin(RADIANS(SunAppLong))));

        public double Y => Math.Tan(RADIANS(ObliqCorr / 2)) * Math.Tan(RADIANS(ObliqCorr / 2));

        public double EqofTime => 4 * DEGREES(Y * Math.Sin(2 * RADIANS(GeomMeanLongSun)) - 2 * EccentEarthOrbit * Math.Sin(RADIANS(GeomMeanAnomSun)) + 4 * EccentEarthOrbit * Y * Math.Sin(RADIANS(GeomMeanAnomSun)) * Math.Cos(2 * RADIANS(GeomMeanLongSun)) - 0.5 * Y * Y * Math.Sin(4 * RADIANS(GeomMeanLongSun)) - 1.25 * EccentEarthOrbit * EccentEarthOrbit * Math.Sin(2 * RADIANS(GeomMeanAnomSun)));

        public double HASunrise => DEGREES(Math.Acos(Math.Cos(RADIANS(90.833)) / (Math.Cos(RADIANS(GeoCord.Latitude)) * Math.Cos(RADIANS(SunDeclin))) - Math.Tan(RADIANS(GeoCord.Latitude)) * Math.Tan(RADIANS(SunDeclin))));

        private double PSolarNoon => (720 - 4 * GeoCord.Longitude - EqofTime + TimeZoneOffset* 60) / 1440;

        public TimeSpan SolarNoon => DateTime.FromOADate(PSolarNoon).TimeOfDay;

        public TimeSpan SunriseTime => DateTime.FromOADate(PSolarNoon - HASunrise * 4 / 1440).TimeOfDay;

        public TimeSpan SunsetTime => DateTime.FromOADate(PSolarNoon + HASunrise * 4 / 1440).TimeOfDay;

        public double SunlightDuration => 8 * HASunrise;

        //public double TrueSolarTime => MOD1(DateTime.Now.TimeOfDay. * 1440 + EqofTime + 4 *GeoCord.Longitude - 60 * TimeZoneOffset, 1440);

        //public double HourAngle => (TrueSolarTime / 4 < 0) ? TrueSolarTime / 4 + 180 : TrueSolarTime / 4 - 180;

        //public double SolarZenithAngle => DEGREES(Math.Acos(Math.Sin(RADIANS(GeoCord.Latitude)) * Math.Sin(RADIANS(SunDeclin)) + Math.Cos(RADIANS(GeoCord.Latitude)) * Math.Cos(RADIANS(SunDeclin)) * Math.Cos(RADIANS(HourAngle))));

        //public double SolarElevationAngle => 90 - SolarZenithAngle;

        //public double ApproxAtmosphericRefraction => fIF(SolarElevationAngle > 85, 0, fIF(SolarElevationAngle > 5, 58.1 / Math.Tan(RADIANS(SolarElevationAngle)) - 0.07 / POWER(Math.Tan(RADIANS(SolarElevationAngle)), 3) + 0.000086 / POWER(Math.Tan(RADIANS(SolarElevationAngle)), 5), fIF(SolarElevationAngle > -0.575, 1735 + SolarElevationAngle * (-518.2 + SolarElevationAngle * (103.4 + SolarElevationAngle * (-12.79 + SolarElevationAngle * 0.711))), -20.772 / Math.Tan(RADIANS(SolarElevationAngle))))) / 3600;

        //public double SolarElevationCorrectedForATMRefraction => SolarElevationAngle + ApproxAtmosphericRefraction;

        //public double SolarAzimuthAngle => fIF(HourAngle > 0, MOD1(DEGREES(Math.Acos(((Math.Sin(RADIANS(GeoCord.Latitude)) * Math.Cos(RADIANS(SolarZenithAngle))) - Math.Sin(RADIANS(T2))) / (Math.Cos(RADIANS(GeoCord.Latitude)) * Math.Sin(RADIANS(AD2))))) + 180, 360), MOD1(540 - DEGREES(Math.Acos(((Math.Sin(RADIANS($B$3)) * Math.Cos(RADIANS(AD2))) - Math.Sin(RADIANS(T2))) / (Math.Cos(RADIANS(GeoCord.Latitude)) * Math.Sin(RADIANS(AD2))))), 360));

        private double MOD1(double Number, double Divider) { return Number % Divider; }

        private double RADIANS(double angle) { return (Math.PI / 180) * angle; }

        private double DEGREES(double radians) { return radians * (180 / Math.PI); }

        private double FIF(bool condition, double out1, double out2) { if (condition) { return out1; } return out2; }
        private double POWER(double A, double b) { return Math.Pow(A, b); }
    }
}



