using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesertChangerMain
{
    class LocationSettings
    {
        public static Boolean TryGetLocationFromSettings(out System.Device.Location.GeoCoordinate Location)
        {
            if (!string.IsNullOrEmpty(AppSettings.ReadSettingEncrypted("Location")))
            {
                string S = AppSettings.ReadSettingEncrypted("Location");
                string[] Item = S.Split('|');
                Location = new System.Device.Location.GeoCoordinate(double.Parse(Item[0]), double.Parse(Item[1]));
                return true;
            }
            Location = new System.Device.Location.GeoCoordinate(0, 0);
            return false;
        }

        public static void SaveLocationToConfig(System.Device.Location.GeoCoordinate Location)
        {
            AppSettings.AddupdateAppSettingsEncrypted("Location", string.Join("|", Location.Latitude, Location.Longitude));
        }

        public static System.Device.Location.GeoCoordinate GetLocationFromZip()
        {
            return LocationSettings.GetLatLong(int.Parse(LocationSettings.TryGetZip()));
        }

        public static Boolean TryGetLocationFromDevice(out System.Device.Location.GeoCoordinate Location)
        {
            Location = new System.Device.Location.GeoCoordinate(0, 0);

            Boolean value = false;
            System.Device.Location.GeoCoordinateWatcher W = new System.Device.Location.GeoCoordinateWatcher();

            Boolean WW = W.TryStart(false, TimeSpan.FromMilliseconds(1000));
            if (WW == true && W.Position.Location.IsUnknown != true)
            {
                Location = new System.Device.Location.GeoCoordinate(W.Position.Location.Latitude, W.Position.Location.Longitude);
                value = true;
            }
            W.Dispose();

            return value;
        }


        public static string GetZip()
        {
            string zip = string.Empty;
            object locker = new object();
            lock (locker)
            {
                while (!int.TryParse(zip, out int zipparse))
                {
                    zip = Microsoft.VisualBasic.Interaction.InputBox("Please enter your zip code.", "Zip Code", Rawzip);
                    if (string.IsNullOrWhiteSpace(zip))
                    {
                        if (MessageBox.Show("Application needs your zip code. try again, or close", "error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                        {
                            Application.Exit();
                        }
                    }
                }
            }
            Rawzip = zip;
            return zip;
        }

        public static string Rawzip
        {
            get
            {
                return AppSettings.ReadSettingEncrypted("zipcode");
            }
            set
            {
                AppSettings.AddupdateAppSettingsEncrypted("zipcode", value);
            }

        }

        public static string TryGetZip()
        {
            if (string.IsNullOrWhiteSpace(Rawzip)) return GetZip();
            return Rawzip;
        }

        public static System.Device.Location.GeoCoordinate GetLatLong(int zip)
        {
            foreach (string s in System.IO.File.ReadLines("zipcode.csv"))
            {
                ZipItem z = new ZipItem(s);
                if (z.HasValue && z.ZipCode == zip) { return new System.Device.Location.GeoCoordinate(z.Latitude, z.Longitude); }; 
            }
            return new System.Device.Location.GeoCoordinate();
        }

        public class ZipItem
        {
            public Boolean HasValue = false;
            public int ZipCode;
            public double Latitude;
            public double Longitude;
 
            //public string City;
            //public string State;
            //public int Timezone;
            //public Boolean DST;

            public ZipItem(string s)
            {
                const string cm = "\"";
                if (s.Trim().Length > 0 && !s.StartsWith("\"zip")) {  
                string[] items =s.Replace(cm, string.Empty).Split(',');
                    ZipCode = int.Parse(items[0]);
                    Latitude = double.Parse(items[3]);
                    Longitude = double.Parse(items[4]);
                    HasValue = true;
                    // City = items[1];
                    // State = items[2];
                    // Timezone = int.Parse(items[5]);
                    // DST = (items[6] == "1");// Boolean.Parse(items[6]);
                }
            }

        }
    }
}
