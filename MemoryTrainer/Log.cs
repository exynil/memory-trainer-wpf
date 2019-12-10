using System;
using System.IO;

namespace MemoryTrainer
{
    public class Log
    {
        private static readonly string LogFile = "log.txt";
        private static readonly string CurrentDirectory = Directory.GetCurrentDirectory();

        /// <summary>
        ///     Очистка лога.
        /// </summary>
        public static void Clear()
        {
            if (File.Exists(LogFile)) File.WriteAllText(LogFile, string.Empty);
        }

        /// <summary>
        ///     Логи DEBUG.
        /// </summary>
        /// <param name="text">Текст для записи в лог.</param>
        public static void Debug(string text)
        {
            Write(text, "DEBUG");
        }

        /// <summary>
        ///     Логи ERROR.
        /// </summary>
        /// <param name="text">Текст для записи в лог.</param>
        public static void Error(string text)
        {
            Write(text, "ERROR");
        }

        /// <summary>
        ///     Запись в лог.
        /// </summary>
        /// <param name="text">Текст для записи в лог.</param>
        /// <param name="type">Тип записи</param>
        private static void Write(string text, string type)
        {
            using (var w = new StreamWriter(Path.Combine(CurrentDirectory, LogFile), true))
            {
                w.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{type}] {text}");
            }
        }
    }
}