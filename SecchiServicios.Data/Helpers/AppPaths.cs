using System.Runtime.InteropServices;

namespace SecchiServicios.Data.Helpers;

public class AppPaths
{
    public static String GetAppDataFolder()
    {
        string enviromentPath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            enviromentPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            enviromentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support");
        }
        else
        {
            enviromentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        }

        string dataPath = Path.Combine(enviromentPath, "SecchiServicios");

        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        return dataPath;
    }
    
    public static String GetDataFilePath(string filename)
    {
        return Path.Combine(GetAppDataFolder(), filename);
    }
    
    public static String GetRecibosPdfDataFolder()
    {
        string envPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Recibos - SecchiServicios",
            (DateTime.Today.Date).ToString("dd.MM.yyy"));

        if (!Directory.Exists(envPath))
        {
            Directory.CreateDirectory(envPath);
        }
        return envPath;
    }
}