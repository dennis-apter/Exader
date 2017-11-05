using System;

namespace Exader.IO
{
    internal static class Guard
    {
        public static ArgumentOutOfRangeException FilePathParentsOutOfRange(FilePath self, int count)
        {
            return new ArgumentOutOfRangeException(
                nameof(count), count,
                $"В пути {self} количество родительских сегментов меньше {count}.");
        }

        public static ArgumentException FilePathNewNameRequired()
        {
            return new ArgumentException(
                "Имя файла или директории не может быть пустым или состоять из одних лишь точек.");
        }
    }
}