using System.IO;
using System.Drawing;
using System.Reflection;

namespace GDT.Utility.Visualization
{
    public static class FileUtilities
    {
        public static readonly string TestDataDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/../../../testdata/";

        public static string LoadFile(string fileName)
        {
            return File.ReadAllText(TestDataDirectory + fileName);
        }
        
        public static void WriteFile(string fileName, string contents)
        {
            WriteFile(fileName, TestDataDirectory, contents);
        }

        public static void WriteFile(string fileName, string directory, string contents)
        {
            File.WriteAllText(directory + fileName, contents);
        }

        public static Image LoadImage(string fileName)
        {
            return Image.FromFile(TestDataDirectory + fileName);
        }
    }
}