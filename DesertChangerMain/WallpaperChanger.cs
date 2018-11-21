using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace DesertChangerMain
{
        public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;


        public enum Style : int { Tiled = 0, Centered = 1, Stretched = 2 }

        public static void Set(string path, Style style)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            string sStyle = ((int)style).ToString();
            string Tile = 0.ToString();
            if (style == Style.Tiled) { sStyle = 1.ToString(); Tile = 1.ToString(); }
            key.SetValue(@"WallpaperStyle", sStyle);
            key.SetValue(@"TileWallpaper", Tile);
            NativeMethods.SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.I4)]

        internal static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    }
}
