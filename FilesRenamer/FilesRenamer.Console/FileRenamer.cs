using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace FilesRenamer.Console
{
    /// <summary>
    /// Переименовыватель +) файлов
    /// </summary>
    public class FileRenamer
    {
        #region Поля, константы, свойства

        /// <summary>
        /// Длина нового имени файла по умолчанию
        /// </summary>
        private const int DefaultTotalWidth = 3;

        /// <summary>
        /// Минимальное значение задержки в мс
        /// </summary>
        private const int MinDelay = 300;

        /// <summary>
        /// Максимальное значение задержки в мс
        /// </summary>
        private const int MaxDelay = 800;

        /// <summary>
        /// Рандомайзер
        /// </summary>
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Вставлять ли рандомную задержку
        /// </summary>
        private readonly bool _insertRandomDelay;

        /// <summary>
        /// Логгер
        /// </summary>
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Количество символов в новых именах файлов
        /// </summary>
        private readonly int _totalWidth;

        #endregion

        #region Конструкторы

        /// <summary>
        /// Создает экземпляр переименовывателя файлов
        /// </summary>
        public FileRenamer() : this(DefaultTotalWidth)
        {
        }

        /// <summary>
        /// Создает экземпляр переименовывателя файлов
        /// </summary>
        /// <param name="totalWidth">Длина нового имени файла (количество символов в имени)</param>
        public FileRenamer(int totalWidth)
            : this(totalWidth, false)
        {
        }

        /// <summary>
        /// Создает экземпляр переименовывателя файлов
        /// </summary>
        /// <param name="insertRandomDelay">вставлять ли рандомную задержку</param>
        public FileRenamer(bool insertRandomDelay)
            : this(DefaultTotalWidth, insertRandomDelay)
        {
        }

        /// <summary>
        /// Создает экземпляр переименовывателя файлов
        /// </summary>
        /// <param name="totalWidth">Длина нового имени файла (количество символов в имени)</param>
        /// <param name="insertRandomDelay">вставлять ли рандомную задержку</param>
        public FileRenamer(int totalWidth, bool insertRandomDelay)
        {
            _totalWidth = totalWidth;
            _insertRandomDelay = insertRandomDelay;
        }

        #endregion

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

        /// <summary>
        /// Переименовывыает файл <paramref name="file" />
        /// </summary>
        /// <param name="file">файл</param>
        /// <param name="number">порядковый номер файла в папке</param>
        private void Rename(FileInfo file, int number)
        {
            if (file == null) throw new ArgumentNullException("file");

            var newFileName = number.ToString(CultureInfo.InvariantCulture).PadLeft(_totalWidth, '0');
            var newFilePath = Path.Combine(file.DirectoryName, newFileName + file.Extension);
            if (_insertRandomDelay)
            {
                Thread.Sleep(Random.Next(MinDelay, MaxDelay));
            }
            File.Move(file.FullName, newFilePath);
            _logger.Debug("{0}\t-> {1}", file.FullName.Replace(@"c:\1", ""), newFilePath.Replace(@"c:\1", ""));
        }
    }
}