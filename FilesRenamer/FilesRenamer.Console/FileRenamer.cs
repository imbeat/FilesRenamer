using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace FilesRenamer.Console
{
    public class FileRenamer
    {
        static Random _random = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Количество символов в новых именах файлов
        /// </summary>
        public readonly int TotalWidth = 3;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Переименовывает файлы в папке
        /// </summary>
        /// <param name="directoryPath">Путь к папке в которой нужно переименовать файлы</param>
        /// <param name="includeSubdirectories">также переименовывать в подпапках</param>
        public void RenameFilesInDirectory(string directoryPath, bool includeSubdirectories = true)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException();
            }

            _logger.Debug("+++ Начинаем переименовывать файлы в папке: " + directoryPath);
            var filePathsList = Directory.GetFiles(directoryPath);

            var subdirectoryPathsList = Directory.GetDirectories(directoryPath);
            // обработка подпапок в отдельных потоках
            var subdirectoriesProcessTasks = subdirectoryPathsList.Select(x => Task.Factory.StartNew(() => RenameFilesInDirectory(x))).ToArray();

            var files = filePathsList.Select(x => new FileInfo(x)).OrderBy(x => x.Name).ToArray();
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                Rename(file, i + 1);
            }
            Task.WaitAll(subdirectoriesProcessTasks);
            _logger.Debug("--- Закончили переименовывать файлы в папке: " + directoryPath);
        }

        private void Rename(FileInfo file, int number)
        {
            if (file == null) throw new ArgumentNullException("file");

            var newFileName = number.ToString(CultureInfo.InvariantCulture).PadLeft(TotalWidth, '0');
            var newFilePath = Path.Combine(file.DirectoryName, newFileName + file.Extension);
            Thread.Sleep(_random.Next(300, 800));
            File.Move(file.FullName, newFilePath);
            _logger.Debug("{0}\t-> {1}", file.FullName.Replace(@"c:\1", ""), newFilePath.Replace(@"c:\1", ""));
        }
    }
}