using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Device.Location;
using System.Linq;

namespace DesertChangerMain
{
    public sealed class MainApp : IDisposable
    {
        #region "Globals"
        private NotifyIcon notifyIcon;
        private ContextMenu notificationMenu;
        private GeoCoordinate location;
        private TimeSpan SunRise;
        private TimeSpan SunSet;
        private Dictionary<TimeSpan, string> ImageSpans;
        private Byte LastDayUpdates;
        private string ActiveString = string.Empty;
        #endregion

        #region "Event Handles"

        public MainApp()
        {
            DeclareGlobals();
            notifyIcon = new NotifyIcon();
            notificationMenu = new ContextMenu(InitializeMenu());
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Text = "Desert Changer";
            notifyIcon.ContextMenu = notificationMenu;
            CreateTimer();
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            if (ImageSpans.Count > 0) { 
            string S = (from KeyValuePair<TimeSpan, string> i in ImageSpans where i.Key < DateTime.Now.TimeOfDay orderby i.Key descending select i.Value).FirstOrDefault();
                if (String.IsNullOrWhiteSpace(S)) { S = (from KeyValuePair<TimeSpan, string> i in ImageSpans  orderby i.Key descending select i.Value).FirstOrDefault(); }
                if (!string.Equals(S, ActiveString))
            {
                ActiveString = S;
                Wallpaper.Set(S, Wallpaper.Style.Stretched);
            }
            if ((int)LastDayUpdates != DateTime.Now.Day)
            {
                LastDayUpdates = (byte)DateTime.Now.Day;
                RefreshSettings();
            }
            }
        }

        public void MenuSettingsUpdateLocation(object sender, EventArgs e) { RefreshLocation(); }

        public void MenuSettingsUpdateDayImages(object sender, EventArgs e) { ImageSettings.PopupMenuForDayImages(); }

        public void MenuSettingsUpdateNightImages(object sender, EventArgs e) { ImageSettings.PopupMenuForNightImages(); }


        private void MenuExitClick(object sender, EventArgs e) { Application.Exit(); }

        private void MenuSettingsInfoClick(object sender, EventArgs e)
        {
            System.Text.StringBuilder SB = new System.Text.StringBuilder();
            SB.Append("").Append(new DateTime(SunRise.Ticks).ToShortTimeString()).Append(" - ").Append(new DateTime(SunSet.Ticks).ToShortTimeString()).Append(Environment.NewLine);
            SB.Append(Environment.NewLine).Append("Timed Items").Append(Environment.NewLine).Append("___________").Append(Environment.NewLine);
            foreach (var item in ImageSpans)
            {
                SB.Append(new DateTime(item.Key.Ticks).ToShortTimeString()).Append(" - ").Append(item.Value).Append(Environment.NewLine);
            }
            MessageBox.Show(SB.ToString(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuAboutClick(object sender, EventArgs e)
        {
            string Name = this.GetType().Assembly.GetName().Name;

            System.Text.StringBuilder message = new System.Text.StringBuilder();
            //Description
            //
            //Version: XXXX
            //Copyright:XXXXX
            //
            //Others
            var CustomDescriptionAttributes = this.GetType().Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false);
            if (CustomDescriptionAttributes.Length > 0) { message.Append(((System.Reflection.AssemblyDescriptionAttribute)CustomDescriptionAttributes[0]).Description).Append(Environment.NewLine); }
            message.Append(Environment.NewLine);
            message.Append("Version: ").Append(this.GetType().Assembly.GetName().Version.ToString()).Append(Environment.NewLine);
            var CustomInfoCopyrightCall = this.GetType().Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), false);
            if (CustomInfoCopyrightCall.Length > 0) { message.Append("Copyright: ").Append(((System.Reflection.AssemblyCopyrightAttribute)CustomInfoCopyrightCall[0]).Copyright).Append(Environment.NewLine); }
            message.Append(Environment.NewLine);
            message.Append("Credit also needs to go to Jean Meeus for formulas to calculate Sunset and rise, and Schuyler Erle for the CivicSpace US ZIP Code Database").Append(Environment.NewLine);
            message.Append("Icon from Konrad Michalik, https://github.com/jackd248/weather-iconic");
            MessageBox.Show(message.ToString(), Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region "Functions and Methods"

        private void CreateTimer()

        {
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 18000 };
            t.Tick += new EventHandler(OnTimedEvent);
            t.Start();
            OnTimedEvent(null, null);
        }


        private void DeclareGlobals()
        {
            if (!LocationSettings.TryGetLocationFromSettings(out location)) { RefreshLocation(); }
            RefreshSettings();
        }

        private void RefreshLocation()
        {
            if (!(LocationSettings.TryGetLocationFromSettings(out location) || LocationSettings.TryGetLocationFromDevice(out location))) { location = LocationSettings.GetLocationFromZip(); }
            location.Latitude = double.Parse(Shared.InputBox("Latitude", "Latitude", location.Latitude.ToString()));
            location.Longitude = double.Parse(Shared.InputBox("Longitude", "Longitude", location.Longitude.ToString()));
            LocationSettings.SaveLocationToConfig(location);
        }

        private void RefreshSettings()
        {
            Dictionary<TimeSpan, string> NightTimeImages;
            SunSettings Settings = new SunSettings(location);
            SunRise = Settings.SunriseTime;
            SunSet = Settings.SunsetTime;
            ImageSpans = ImageSettings.GenerateDayImages(ImageSettings.GetDayImagesFromConfig() ?? ImageSettings.PopupMenuForDayImages(), SunRise, SunSet);
            NightTimeImages = ImageSettings.GenerateNightImages(ImageSettings.GetNightImagesFromConfig() ?? ImageSettings.PopupMenuForNightImages(), SunRise, SunSet);
            foreach (KeyValuePair<TimeSpan, string> item in NightTimeImages)
            {
                ImageSpans.Add(item.Key, item.Value);
            }
            LastDayUpdates = (byte)DateTime.Now.Day;
        }

        #endregion

        #region "Menu Construction"
               
        private MenuItem[] InitializeMenu()
        {
            return new MenuItem[] {
                new MenuItem("Settings", GetSettings()),
                new MenuItem("About", MenuAboutClick),
                new MenuItem("Exit", MenuExitClick)
            };
        }


        private MenuItem[] GetSettings()
        {
            return new MenuItem[] {
                new MenuItem("Info", MenuSettingsInfoClick),
                new MenuItem("Update Location", MenuSettingsUpdateLocation),
                new MenuItem("Update Day Images", MenuSettingsUpdateDayImages),
                new MenuItem("Update Night Images", MenuSettingsUpdateNightImages)
            };
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //notifyIcon.Dispose();
                    //notificationMenu.Dispose(); }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose() { Dispose(true); }
        #endregion

        #region Main - Program entry point
        /// <summary>Program entry point.</summary>
        /// <param name="args">Command Line Arguments</param>
        [STAThread]
        public static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool isFirstInstance = false;
            // Please use a unique name for the mutex to prevent conflicts with other programs

            using (Mutex mtx = new Mutex(true, "DesertChanger", out isFirstInstance))
            {
                if (isFirstInstance)
                {
                    try
                    {
                        MainApp notificationIcon = new MainApp();
                        notificationIcon.notifyIcon.Visible = true;
                        GC.Collect();
                        Application.Run();
                        notificationIcon.notifyIcon.Dispose();
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show("Error: " + x.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    mtx.ReleaseMutex();
                }
                else
                {
                    GC.Collect();
                    MessageBox.Show("App appears to be running. if not, you may have to restart your machine to get it to work.");
                }
            }



        }
        #endregion

    }
}
