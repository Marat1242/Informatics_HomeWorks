using System.Text.Json;

namespace ORIS.week7
{
    public class ServerSettings
    {
        public int Port { get; set; } = 7700;
        public string Path { get; set; } = @"./Site/";

        public ServerSettings() { }
        public ServerSettings(int port, string path)
        {
            Port = port;
            Path = path;
        }

        public void Serialize()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);

            using (StreamWriter streamWriter = new StreamWriter(@"Settings.JSON", false))
            {
                streamWriter.WriteLine(jsonString);
            }
        }
        public static ServerSettings Deserialize()
        {
            using (FileStream fs = new FileStream(@"Settings.JSON", FileMode.OpenOrCreate))
            {
                return JsonSerializer.Deserialize<ServerSettings>(fs);
            }
        }
    }
}