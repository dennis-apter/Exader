using System;
using System.Text;
using JetBrains.Annotations;

namespace Exader
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class Countable
    {
        public static string ToCountableName(this decimal number, string namesString, char delimiter = '|') { return ToCountableName(Truncate(number), namesString, delimiter); }
        public static string ToCountableName(this double number, string namesString, char delimiter = '|') { return ToCountableName(Truncate(number), namesString, delimiter); }
        public static string ToCountableName(this float number, string namesString, char delimiter = '|') { return ToCountableName(Truncate(number), namesString, delimiter); }
        public static string ToCountableName(this int number, string namesString, char delimiter = '|') { return ToCountableName((long)number, namesString, delimiter); }
        public static string ToCountableName(this byte number, string namesString, char delimiter = '|') { return ToCountableName((long)number, namesString, delimiter); }
        public static string ToCountableName(this short number, string namesString, char delimiter = '|') { return ToCountableName((long)number, namesString, delimiter); }
        /// <summary>
        /// Возвращает числительное имя.
        /// </summary>
        /// <param name="number">Число.</param>
        /// <param name="namesString">Формы числительного в аффиксном формате (например: "элемент|а|ов").</param>
        /// <param name="delimiter">Разделитель аффиксов в формате (по умолчанию: '|').</param>
        /// <returns></returns>
        public static string ToCountableName(this long number, string namesString, char delimiter = '|')
        {
            var sb = new StringBuilder(namesString.Length);
            var words = namesString.Split(' ');
            for (int index = 0; index < words.Length; index++)
            {
                if (0 < index) sb.Append(' ');

                var name = GetName(number, words[index], delimiter);
                sb.Append(name);
            }

            return sb.ToString();
        }

        private static string GetName(long number, string namesString, char delimiter = '|')
        {
            if (namesString != string.Empty)
            {
                int minusIndex = namesString.IndexOf('-');
                // В новом формате через +- разделитель не должен следовать после пробела
                if (1 < minusIndex && !char.IsWhiteSpace(namesString[minusIndex - 1]))
                {
                    return GetNewName(number, namesString);
                }

                // раньше тут удалялись пустые итемы, это неправильно, т.к. в некоторых формах может быть нулевое окончание
                // например, "15 скважин"
                string[] parts = namesString.Split(delimiter);
                if (2 <= parts.Length && parts.Length <= 5)
                {
                    string newTail, suffix;
                    if (TryGetSuffix(parts[parts.Length - 1], out newTail, out suffix))
                    {
                        parts[parts.Length - 1] = newTail;
                    }

                    if (2 == parts.Length)
                    {
                        // упрощенный щаблон удобный для английского (0 items, 1 item, 2 items, 10 items, 21 items)
                        if (1 == number)
                        {
                            return parts[0] + suffix;
                        }

                        return parts[0] + parts[1] + suffix;
                    }

                    return GetNameFromParts(number, parts) + suffix;
                }
            }

            return namesString;
        }

        private static string GetNameFromParts(long number, string[] parts)
        {
            GrammaticalNumber grammaticalNumber = GetGrammaticalNumber(number);
            switch (grammaticalNumber)
            {
                case GrammaticalNumber.NominativeSingular:
                    return parts.Length == 3 ? parts[0] : parts[0] + parts[1];
                case GrammaticalNumber.GenitiveSingular:
                    return parts.Length == 3 ? parts[0] + parts[1] : parts[0] + parts[2];
                default:
                    return parts.Length == 3 ? parts[0] + parts[2] : parts[0] + parts[3];
            }
        }

        private static bool TryGetSuffix(string oldTail, out string newTail, out string suffix)
        {
            var buffer = new StringBuilder(oldTail.Length);
            var tail = oldTail;
            int tailLength;
            for (tailLength = tail.Length - 1; 0 <= tailLength; tailLength--)
            {
                if (char.IsLetter(tail[tailLength])) break;

                buffer.Insert(0, tail[tailLength].ToString());
            }

            if (0 < buffer.Length)
            {
                newTail = tail.Substring(0, tailLength + 1);
                suffix = buffer.ToString();
                return true;
            }

            newTail = oldTail;
            suffix = string.Empty;
            return false;
        }

        private static string GetNewName(long number, string namesString)
        {
            string[] parts = namesString.Split('-');
            if (2 <= parts.Length && parts.Length <= 4)
            {
                var heads = parts[0].Split('+');
                if (2 < heads.Length)
                {
                    // Invalid format
                    return namesString;
                }

                string newTail, suffix;
                if (TryGetSuffix(parts[parts.Length - 1], out newTail, out suffix))
                {
                    parts[parts.Length - 1] = newTail;
                }

                if (parts.Length == 2)
                {
                    // упрощенный щаблон удобный для английского (0 items, 1 item, 2 items, 10 items, 21 items)
                    if (heads.Length == 1)
                    {
                        if (1 == number)
                        {
                            return parts[0] + suffix;
                        }

                        return parts[0] + parts[1] + suffix;
                    }

                    if (1 == number)
                    {
                        return heads[0] + heads[1] + suffix;
                    }

                    return heads[0] + parts[1] + suffix;
                }

                if (heads.Length == 2)
                {
                    parts = parts.Growth(2).Move(1);
                    parts[0] = heads[0];
                    parts[1] = heads[1];
                    parts[parts.Length - 1] = parts[parts.Length - 2];
                }

                return GetNameFromParts(number, parts) + suffix;
            }

            return namesString;
        }

        public static string ToCountableName(this decimal number, string nomSing, string genSing, string genPlur) { return ToCountableName(Truncate(number), nomSing, genSing, genPlur); }
        public static string ToCountableName(this double number, string nomSing, string genSing, string genPlur) { return ToCountableName(Truncate(number), nomSing, genSing, genPlur); }
        public static string ToCountableName(this float number, string nomSing, string genSing, string genPlur) { return ToCountableName(Truncate(number), nomSing, genSing, genPlur); }
        public static string ToCountableName(this int number, string nomSing, string genSing, string genPlur) { return ToCountableName((long)number, nomSing, genSing, genPlur); }
        public static string ToCountableName(this byte number, string nomSing, string genSing, string genPlur) { return ToCountableName((long)number, nomSing, genSing, genPlur); }
        public static string ToCountableName(this short number, string nomSing, string genSing, string genPlur) { return ToCountableName((long)number, nomSing, genSing, genPlur); }
        /// <summary>
        /// Возвращает числительное имя.
        /// </summary>
        /// <param name="number">Число.</param>
        /// <param name="nomSing">Форма единственного числа (например: [1] элемент).</param>
        /// <param name="genSing">Форма множественного числа (например: [2] элемента).</param>
        /// <param name="genPlur">Форма множественного числа (например: [5] элементов).</param>
        /// <returns></returns>
        public static string ToCountableName(this long number, string nomSing, string genSing, string genPlur)
        {
            GrammaticalNumber grammaticalNumber = GetGrammaticalNumber(number);
            switch (grammaticalNumber)
            {
                case GrammaticalNumber.NominativeSingular:
                    return nomSing;
                case GrammaticalNumber.GenitiveSingular:
                    return genSing;
                default:
                    return genPlur;
            }
        }

        public static string ToCountableString(this decimal number, string namesString, string stringFormat = "{0} {1}", char delimiter = '|') { return Format(stringFormat, number, namesString, delimiter); }
        public static string ToCountableString(this double number, string namesString, string stringFormat = "{0} {1}", char delimiter = '|') { return Format(stringFormat, number, namesString, delimiter); }
        public static string ToCountableString(this float number, string namesString, string stringFormat = "{0} {1}", char delimiter = '|') { return Format(stringFormat, number, namesString, delimiter); }
        public static string ToCountableString(this int number, string namesString, string stringFormat = "{0} {1}", char delimiter = '|') { return Format(stringFormat, number, namesString, delimiter); }
        public static string ToCountableString(this byte number, string namesString, string stringFormat = "{0} {1}", char delimiter = '|') { return Format(stringFormat, number, namesString, delimiter); }
        public static string ToCountableString(this short number, string namesString, string stringFormat = "{0} {1}", char delimiter = '|') { return Format(stringFormat, number, namesString, delimiter); }
        public static string ToCountableString(this long number, string namesString, string stringFormat = "{0} {1}", char delimiter = '|') { return Format(stringFormat, number, namesString, delimiter); }

        public static string ToCountableInfixString(this decimal number, string format, char delimiter = '|') { return Format(format, number, delimiter); }
        public static string ToCountableInfixString(this double number, string format, char delimiter = '|') { return Format(format, number, delimiter); }
        public static string ToCountableInfixString(this float number, string format, char delimiter = '|') { return Format(format, number, delimiter); }
        public static string ToCountableInfixString(this int number, string format, char delimiter = '|') { return Format(format, number, delimiter); }
        public static string ToCountableInfixString(this byte number, string format, char delimiter = '|') { return Format(format, number, delimiter); }
        public static string ToCountableInfixString(this short number, string format, char delimiter = '|') { return Format(format, number, delimiter); }
        public static string ToCountableInfixString(this long number, string format, char delimiter = '|') { return Format(format, number, delimiter); }

        [StringFormatMethod("format")]
        public static string Format(string format, decimal number, string namesString, char delimiter = '|') { return string.Format(format, number, ToCountableName(number, namesString, delimiter)); }
        [StringFormatMethod("format")]
        public static string Format(string format, double number, string namesString, char delimiter = '|') { return string.Format(format, number, ToCountableName(number, namesString, delimiter)); }
        [StringFormatMethod("format")]
        public static string Format(string format, float number, string namesString, char delimiter = '|') { return string.Format(format, number, ToCountableName(number, namesString, delimiter)); }
        [StringFormatMethod("format")]
        public static string Format(string format, int number, string namesString, char delimiter = '|') { return Format(format, (long)number, namesString, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(string format, byte number, string namesString, char delimiter = '|') { return Format(format, (long)number, namesString, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(string format, short number, string namesString, char delimiter = '|') { return Format(format, (long)number, namesString, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(string format, long number, string namesString, char delimiter = '|')
        {
            return string.Format(format, number, ToCountableName(number, namesString, delimiter));
        }

        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, decimal number, string namesString, char delimiter = '|') { return string.Format(provider, format, number, ToCountableName(number, namesString, delimiter)); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, double number, string namesString, char delimiter = '|') { return string.Format(provider, format, number, ToCountableName(number, namesString, delimiter)); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, float number, string namesString, char delimiter = '|') { return string.Format(provider, format, number, ToCountableName(number, namesString, delimiter)); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, int number, string namesString, char delimiter = '|') { return Format(format, (long)number, namesString, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, byte number, string namesString, char delimiter = '|') { return Format(format, (long)number, namesString, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, short number, string namesString, char delimiter = '|') { return Format(format, (long)number, namesString, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, long number, string namesString, char delimiter = '|')
        {
            return string.Format(provider, format, number, ToCountableName(number, namesString, delimiter));
        }

        [StringFormatMethod("format")]
        public static string Format(string format, decimal number, char delimiter = '|') { return string.Format(GetFormat(format, Truncate(number), delimiter), number); }
        [StringFormatMethod("format")]
        public static string Format(string format, double number, char delimiter = '|') { return string.Format(GetFormat(format, Truncate(number), delimiter), number); }
        [StringFormatMethod("format")]
        public static string Format(string format, float number, char delimiter = '|') { return string.Format(GetFormat(format, Truncate(number), delimiter), number); }
        [StringFormatMethod("format")]
        public static string Format(string format, int number, char delimiter = '|') { return Format(format, (long)number, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(string format, byte number, char delimiter = '|') { return Format(format, (long)number, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(string format, short number, char delimiter = '|') { return Format(format, (long)number, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(string format, long number, char delimiter = '|')
        {
            return string.Format(GetFormat(format, number, delimiter), number);
        }

        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, decimal number, char delimiter = '|') { return string.Format(provider, GetFormat(format, Truncate(number), delimiter), number); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, double number, char delimiter = '|') { return string.Format(provider, GetFormat(format, Truncate(number), delimiter), number); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, float number, char delimiter = '|') { return string.Format(provider, GetFormat(format, Truncate(number), delimiter), number); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, int number, char delimiter = '|') { return Format(provider, format, (long)number, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, byte number, char delimiter = '|') { return Format(provider, format, (long)number, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, short number, char delimiter = '|') { return Format(provider, format, (long)number, delimiter); }
        [StringFormatMethod("format")]
        public static string Format(IFormatProvider provider, string format, long number, char delimiter = '|')
        {
            return string.Format(provider, GetFormat(format, number, delimiter), number);
        }

        private static string GetFormat(string format, long number, char delimiter)
        {
            string prefix = format.SubstringBefore('{');
            if (string.IsNullOrEmpty(prefix)) throw new ArgumentException();

            string suffix = format.SubstringAfter('}');

            string prefixSpace = prefix;
            //prefix = prefix.Trim();
            prefixSpace = prefixSpace.Substring(prefix.Length);

            string placeholder = format.Substring(prefix.Length, format.Length - suffix.Length - prefix.Length);
            var newFormat = ToCountableName(number, prefix, delimiter) + prefixSpace + placeholder + ToCountableName(number, suffix, delimiter);
            return newFormat;
        }

        /// <summary>
        /// Возвращает целый остаток от деления на 100.
        /// </summary>
        private static long Truncate(decimal num)
        {
            return (long)(decimal.Floor(num) % 100m);
        }

        /// <summary>
        /// Возвращает целый остаток от деления на 100.
        /// </summary>
        private static long Truncate(double num)
        {
            var str = Math.Floor(num).ToString("F0");
            str = str.Right(2);
            return (long)double.Parse(str);
        }

        private static GrammaticalNumber GetGrammaticalNumber(long num)
        {
            num = Math.Abs(num) % 100; // всякие 4115, -25120 ведут себя так же, как и, соответственно, 15 и 20

            if (1 == num)
            {
                return GrammaticalNumber.NominativeSingular;
            }

            if (1 < num && num < 5)
            {
                return GrammaticalNumber.GenitiveSingular;
            }

            if (1 < num && num < 21)
            {
                return GrammaticalNumber.GenitivePlural;
            }

            if (20 < num)
            {
                long mod = num % 10;
                return GetGrammaticalNumber(mod);
            }

            return GrammaticalNumber.GenitivePlural;
        }

        private enum GrammaticalNumber
        {
            /// <summary>
            /// 1 день (Им.п ед.ч)
            /// </summary>
            NominativeSingular,
            /// <summary>
            /// 2, 3, 4 дня (Род.п ед.ч)
            /// </summary>
            GenitiveSingular,
            /// <summary>
            /// (5, 6, 7, 8, 9), (10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20) дней (Род.п мн.ч)
            /// </summary>
            GenitivePlural,
        }
    }
}
