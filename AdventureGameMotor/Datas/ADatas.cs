using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvG
{
    class ADatas
    {

        public static void WriteFile(string folder, string file, object obj)
        {
            string json = JsonConvert.SerializeObject(obj);

            bool exists = System.IO.Directory.Exists(Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), folder));
            if (!exists)
                System.IO.Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), folder));

            System.IO.File.WriteAllText(Path.Combine(Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), folder), file), json);

        }

        public static T ReadFile<T>(string folder, string file)
        {

            string text = System.IO.File.ReadAllText(Path.Combine(Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), folder), file));

            return JsonConvert.DeserializeObject<T>(text);

        }


    }
}
