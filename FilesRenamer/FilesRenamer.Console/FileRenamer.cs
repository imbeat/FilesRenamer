using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FilesRenamer.Console
{
    public class FileRenamer
    {
        public const int TotalWidth = 3;

        public void Rename(FileInfo file, int number)
        {
            if (file == null) throw new ArgumentNullException("file");
            var newFileName = number.ToString(CultureInfo.InvariantCulture).PadLeft(TotalWidth, '0');
            var newFilePath = Path.Combine(file.DirectoryName, newFileName + file.Extension);
            File.Move(file.FullName, newFilePath);
        }

        public void RenameFilesInDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException();
            }
            var filePathsList = Directory.GetFiles(directoryPath);
            var subdirectoryPathsList = Directory.GetDirectories(directoryPath);
            if (subdirectoryPathsList.Any())
            {
                foreach (var subdirectoryPath in subdirectoryPathsList)
                {
                    RenameFilesInDirectory(subdirectoryPath);
                }
            }

            var files = filePathsList.Select(x => new FileInfo(x)).OrderBy(x => x.Name).ToArray();
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                Rename(file, i+1);
            }
        }

    }
}