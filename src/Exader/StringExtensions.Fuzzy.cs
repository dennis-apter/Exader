using System;

namespace Exader
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Возвращает меру различия строк, схожей с расстоянием Левенштейна,
        /// но более быструю и надежную.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double Distance(this string self, string other)
        {
            return self.Distance(other, 5);
        }

        /// <summary>
        /// Возвращает меру различия строк, схожей с расстоянием Левенштейна,
        /// но более быструю и надежную.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="maxOffset"></param>
        /// <returns></returns>
        public static double Distance(this string self, string other, int maxOffset)
        {
            if (string.IsNullOrEmpty(self)) return string.IsNullOrEmpty(other) ? 0 : other.Length;

            if (string.IsNullOrEmpty(other)) return self.Length;

            int index = 0;
            int selfOffset = 0;
            int otherOffset = 0;
            int lcsLen = 0; // Longest Common Substring length
            while ((index + selfOffset < self.Length) && (index + otherOffset < other.Length))
            {
                if (self[index + selfOffset] == other[index + otherOffset])
                {
                    lcsLen++;
                }
                else
                {
                    selfOffset = 0;
                    otherOffset = 0;
                    for (int i = 0; i < maxOffset; i++)
                    {
                        if ((index + i < self.Length) && (self[index + i] == other[index]))
                        {
                            selfOffset = i;
                            break;
                        }

                        if ((index + i < other.Length) && (self[index] == other[index + i]))
                        {
                            otherOffset = i;
                            break;
                        }
                    }
                }

                index++;
            }

            return ((self.Length + other.Length) / 2d) - lcsLen;
        }

        /// <summary>
        /// Возвращает расстояние Левенштейна (также редакционное расстояние или дистанция редактирования).
        /// </summary>
        /// <remarks>
        /// Расстояние Левенштейна (также редакционное расстояние или дистанция редактирования)
        /// в теории информации и компьютерной лингвистике — это мера разницы двух последовательностей
        /// символов (строк) относительно минимального количества операций вставки, удаления и замены,
        /// необходимых для перевода одной строки в другую.
        /// </remarks>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static int LevensteinDistance(this string self, string other)
        {
            int selfLength = self.Length;
            int otherLength = other.Length;

            if (selfLength == 0) return otherLength;

            if (otherLength == 0) return selfLength;

            var matrix = new int[selfLength + 1, otherLength + 1];

            for (int selfIndex = 0; selfIndex <= selfLength; selfIndex++)
            {
                matrix[selfIndex, 0] = selfIndex;
            }

            for (int otherIndex = 0; otherIndex <= otherLength; otherIndex++)
            {
                matrix[0, otherIndex] = otherIndex;
            }

            for (int selfIndex = 1; selfIndex <= selfLength; selfIndex++)
            {
                for (int otherIndx = 1; otherIndx <= otherLength; otherIndx++)
                {
                    int cost = other[otherIndx - 1] == self[selfIndex - 1] ? 0 : 1;

                    matrix[selfIndex, otherIndx] = Math.Min(
                        matrix[selfIndex - 1, otherIndx - 1] + cost,
                        Math.Min(
                            matrix[selfIndex - 1, otherIndx] + 1,
                            matrix[selfIndex, otherIndx - 1] + 1));
                }
            }

            return matrix[selfLength, otherLength];
        }

        /// <summary>
        /// Возвращает относительное расстояние Левенштейна (также редакционное расстояние или дистанция редактирования).
        /// </summary>
        /// <remarks>
        /// Относительное расстояние вычисляется как отношение абсолютного расстояния к наибольшей длине строк.
        /// </remarks>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double LevensteinSimilarity(this string self, string other)
        {
            double distance = self.LevensteinDistance(other);
            int maxLength = Math.Max(self.Length, other.Length);
            return maxLength == 0 ? 1d : 1d - (distance / maxLength);
        }

        public static int SentenceLevensteinDistance(this string self, string other)
        {
            // получение слов в предложениях                      
            string[] selfWords = self.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] otherWords = other.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                // Каждое слово в предложении сравнивается 
                // со всеми словами другого предложения

                int distance = 0;
                for (int si = 0; si < selfWords.Length; si++)
                {
                    int sd = 0;
                    for (int oi = 0; oi < otherWords.Length; oi++)
                    {
                        // получение дистанции для пары слов в предложении     
                        int od = LevensteinDistance(selfWords[si], otherWords[oi]);
                        if (sd < od)
                        {
                            sd = od;
                        }
                    }

                    distance += sd;
                }

                return distance;
            }
            catch (Exception error)
            {
                throw new Exception("Ошибка при сравнении предложений", error);
            }
        }

        public static double SentenceLevensteinSimilarity(this string self, string other)
        {
            double distance = self.SentenceLevensteinDistance(other);
            double maxLength = Math.Max(self.Length, other.Length);
            // получения относительного значения схожести двух предложений
            return (0.1 < Math.Abs(0 - maxLength)) && (0.0001 < Math.Abs(0 - distance))
                ? 1d - (distance < maxLength ? distance / maxLength : maxLength / distance)
                : 1d;
        }

        public static double Similarity(this string self, string other)
        {
            double distance = self.Distance(other);
            int maxLength = Math.Max(self.Length, other.Length);
            return 0 == maxLength ? 1d : 1d - (distance / maxLength);
        }
    }
}
