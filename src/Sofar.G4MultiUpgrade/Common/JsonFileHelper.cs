using System.Text;
using Newtonsoft.Json;

namespace Sofar.G4MultiUpgrade.Common
{
    internal class JsonFileHelper
    {
        private static readonly string _saveFolder = Path.Combine(Application.StartupPath, "config");

        public static void SaveConfig(string fileName, object? obj)
        {
            string contents = JsonConvert.SerializeObject(obj, Formatting.Indented);

            var dirInfo = Directory.CreateDirectory(_saveFolder);
            File.WriteAllText(Path.Combine(dirInfo.FullName, fileName), contents, Encoding.UTF8);
        }

        public static TModel? LoadConfig<TModel>(string fileName)
        {
            string fileFullPath = Path.Combine(_saveFolder, fileName);
            if (!File.Exists(fileFullPath))
            {
                return default;
            }
            else
            {
                var contents = File.ReadAllText(Path.Combine(_saveFolder, fileName), Encoding.UTF8);
                var model = JsonConvert.DeserializeObject<TModel>(contents);
                return model;
            }
        }
    }
}
