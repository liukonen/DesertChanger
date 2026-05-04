# DesertChanger
`C#` `WPF` `WinForms` `GeoLocation` `Wallpaper Automation`

A lightweight application that dynamically changes your desktop wallpaper based on the time of day, inspired by macOS Mojave.

## Context & Story
DesertChanger was created to bring the dynamic wallpaper feature of macOS Mojave to Windows. It uses astronomical calculations to determine sunrise and sunset times, ensuring your wallpaper matches the time of day. The project also integrates various technical concepts, making it a learning ground for advanced C# features.

## Architecture & Decisions
- **C# with WPF and WinForms**: Chosen for seamless integration with Windows desktop features.
- **GeoLocation API**: Determines the user's location for accurate sunrise and sunset calculations.
- **CSV Parsing**: Utilized for ZIP code-based location data.
- **DLL Imports**: Enables low-level bindings for wallpaper changes.
- **Event-Driven Design**: Handles notifications and wallpaper updates efficiently.

## Key Features
- Dynamically updates wallpaper based on sunrise and sunset times.
- Supports custom image spans for different times of the day.
- Lightweight system tray application with a notification menu.
- Uses GeoLocation to calculate accurate astronomical timings.
- Includes mutex handling to prevent multiple instances.

## Quick Start
1. Clone the repository:
   ```bash
   git clone https://github.com/liukonen/DesertChanger.git
   ```
2. Open `DesertChanger.sln` in Visual Studio.
3. Build and run the solution.
4. Configure your location and image settings via the system tray menu.

## Credits
- **Jean Meeus**: Astronomical Algorithms for sunrise/sunset calculations.
- **Schuyler Erle**: CivicSpace US ZIP Code Database.
- **Konrad Michalik**: Weather Iconic (https://github.com/jackd248/weather-iconic).


