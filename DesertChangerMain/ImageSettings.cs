using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DesertChangerMain
{
    class ImageSettings
    {


        public static System.Collections.Generic.Dictionary<TimeSpan, string> GenerateDayImages(string[] images, TimeSpan Sunrise, TimeSpan Sunset)
        {
            System.Collections.Generic.Dictionary<TimeSpan, string> response = new System.Collections.Generic.Dictionary<TimeSpan, string>();
            int Count = images.Length + 1;
           double TotalMinutes = Sunset.Subtract(Sunrise).TotalMinutes;
            int Spacer = (int)(TotalMinutes / Count);

            int I = 0;
            foreach (string item in images)
            {
                response.Add(Sunrise.Add(new TimeSpan(0, I * Spacer, 0)), item);
                I += 1;
            }
            return response;

        }

        public static System.Collections.Generic.Dictionary<TimeSpan, string> GenerateNightImages(string[] images, TimeSpan Sunrise, TimeSpan Sunset)
        {
            System.Collections.Generic.Dictionary<TimeSpan, string> response = new System.Collections.Generic.Dictionary<TimeSpan, string>();
            int Count = images.Length + 1;
            double TotalMinutes = 1400-Sunset.Subtract(Sunset).TotalMinutes;
            int Spacer = (int)(TotalMinutes / Count);

            int I = 0;
            foreach (string item in images)
            {
                TimeSpan T = Sunset.Add(new TimeSpan(0, I * Spacer, 0));
                if (T.Days > 0) {T= T.Add(new TimeSpan(-T.Days, 0, 0, 0)); }
                response.Add(T, item);
                I += 1;
            }
            return response;

        }

        public static string[] GetDayImagesFromConfig()
        {
            string value = AppSettings.ReadSettingEncrypted("DayImages");
            if (string.IsNullOrWhiteSpace(value)) { return null; }
            return value.Split('|');
        }

        public static string[] PopupMenuForDayImages()
        {
            System.Windows.Forms.OpenFileDialog dialog = new OpenFileDialog
            {
                DefaultExt = "Jpg(*.jpg) | *.jpg",
                Title = "Day time image(s)",
                Multiselect = true
            };
            dialog.ShowDialog();
            AppSettings.AddupdateAppSettingsEncrypted("DayImages", string.Join("|", dialog.FileNames));
            return dialog.FileNames;
         }

        public static string[] GetNightImagesFromConfig()
        {
            string value = AppSettings.ReadSettingEncrypted("NightImages");
            if (string.IsNullOrWhiteSpace(value)) { return null; }
            return value.Split('|');
        }

        public static string[] PopupMenuForNightImages()
        {
            System.Windows.Forms.OpenFileDialog dialog = new OpenFileDialog
            {
                DefaultExt = "Jpg(*.jpg) | *.jpg",
                Title = "Night time image(s)",
                Multiselect = true
            };
            dialog.ShowDialog();
            AppSettings.AddupdateAppSettingsEncrypted("NightImages", string.Join("|", dialog.FileNames));
            return dialog.FileNames;
        }

    }
}
