using System.IO;

namespace CSharpUtilities.Core
{
    public class DirectoryHelper
    {
        public static void CopyFiles(string source, string destination)
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            var dirInfo = new DirectoryInfo(source);
            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();

            foreach (var file in files) file.CopyTo(Path.Combine(destination, file.Name), true);

            foreach (var tempDirectory in directories)
                CopyFiles(Path.Combine(source, tempDirectory.Name), Path.Combine(destination, tempDirectory.Name));
        }

        public static void CutFiles(string source, string destination)
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            var dirInfo = new DirectoryInfo(source);
            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();

            foreach (var file in files)
            {
                file.CopyTo(Path.Combine(destination, file.Name), true);
                file.Delete();
            }


            foreach (var tempDirectory in directories)
                CutFiles(Path.Combine(source, tempDirectory.Name), Path.Combine(destination, tempDirectory.Name));
        }

        public static void DeleteFiles(string source, string destination)
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            var dirInfo = new DirectoryInfo(source);
            var files = dirInfo.GetFiles();
            var directories = dirInfo.GetDirectories();

            foreach (var file in files) file.Delete();


            foreach (var tempDirectory in directories)
                DeleteFiles(Path.Combine(source, tempDirectory.Name), Path.Combine(destination, tempDirectory.Name));
        }
    }
}