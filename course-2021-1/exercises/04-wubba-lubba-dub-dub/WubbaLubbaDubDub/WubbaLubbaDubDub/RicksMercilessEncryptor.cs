﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
namespace WubbaLubbaDubDub
{
    public static class RicksMercilessEncryptor
    {
        /// <summary>
        /// Возвращает массив строк исходного текста.
        /// </summary>
        public static string[] SplitToLines(this string text)
        {
            // У строки есть специальный метод. Давай здесь без регулярок
            return text.SplitToLines();
        }

        /// <summary>
        /// Возвращает массив слов исходной строки.
        /// </summary>
        public static string[] SplitToWords(this string line)
        {
            // А вот здесь поиграйся с регулярками.
            /*
             *
             * Хорошо, но разве не проще String.Split(' ')?
             *
             */
            //return Regex.Split(line, @"\s+");
            return line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Возвращает левую половину строки, где граница считается с округлением вниз.
        /// Т.е. и для длины 2n, и для длины 2n + 1 -> первые n символов.
        /// </summary>
        public static string GetLeftHalf(this string s)
        {
            // у строки есть метод получения подстроки
            return s.Substring(0, s.Length / 2);
        }

        /// <summary>
        /// Возвращает правую половину строки, где граница считается с округлением вниз.
        /// Т.е. для длины 2n: последние n, а для длины 2n + 1 -> последние n + 1 символов.
        /// </summary>
        public static string GetRightHalf(this string s)
        {
            return s.Substring(s.Length / 2);
        }

        /// <summary>
        /// Возвращает строку, в которой все вхождения строки <see cref="old"/> заменены на строку <see cref="@new"/>.
        /// </summary>
        public static string Replace(this string s, string old, string @new)
        {
            // и такой метод у строки, очевидно, тоже есть
            return s.Replace(old, @new);
        }

        /// <summary>
        /// Возвращает строку, у которой каждый символ заменен на \uFFFF,
        /// где FFFF - соответствующая шестнадцатиричная кодовая точка.
        /// </summary>
        public static string CharsToCodes(this string s)
        {
            /*
                Может быть удобным здесь же сначала написать локальную функцию
                которая содержит логику для преобразования одного символа,
                а затем использовать её для посимвольного преобразования всей строки.
                FYI: локальную функцию можно объявлять даже после строки с return.
                То же самое можно сделать и для всех оставшихся методов.
            */

            /// <summary>
            /// Возвращает кодовую точку одного символа.
            /// </summary>
            int GetCode(char c)
            {
                return Convert.ToByte(c);
            }

            string codes = "";
            foreach (char c in s)
            {
                codes += $"\\u{GetCode(c).ToString("x4")}";
            }
            return codes;
        }

        /// <summary>
        /// Возвращает строку задом наперёд.
        /// </summary>
        public static string GetReversed(this string s)
        {
            /*
                Собрать строку из последовательности строк можно несколькими способами.
                Один из низ - статический метод Concat. Но ты можешь выбрать любой.
            */
            char[] array = s.ToCharArray();
            Array.Reverse(array);
            return String.Concat(array);
        }

        /// <summary>
        /// Возвращает строку, у которой регистр букв заменён на противоположный.
        /// </summary>
        public static string InverseCase(this string s)
        {
            /*
                Здесь тебе помогут статические методы типа char.
                На минуту задержись здесь и посмотри, какие еще есть статические методы у char.
                Например, он содержит методы-предикаты для определения категории Юникода символа, что очень удобно.
            */
            char[] array = s.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                char c = array[i];
                if(Char.IsUpper(c))
                    array[i] = Char.ToLower(c);
                else
                    array[i] = Char.ToUpper(c);
            }
            return String.Concat(array);
        }

        /// <summary>
        /// Возвращает строку, у которой каждый символ заменен на следующий за ним символ Юникода.
        /// Т.е. каждый символ с кодовой точкой X заменен на символ с кодовой точкой X+1.
        /// </summary>
        public static string ShiftInc(this string s)
        {
            throw new NotImplementedException();
        }


        #region Чуть посложнее

        /// <summary>
        /// Возвращает список уникальных идентификаторов объектов, используемых в тексте <see cref="text"/>.
        /// Идентификаторы объектов имеют длину 8байт и представлены в тексте в виде ¶X:Y¶, где X - старшие 4 байта, а Y - младшие 4 байта.
        /// Текст <see cref="text"/> так же содержит строчные (//) и блоковые (/**/) комментарии, которые нужно игнорировать.
        /// Т.е. в комментариях идентификаторы объектов искать не нужно. И, кстати, блоковые комментарии могут быть многострочными.
        /// </summary>
        public static IImmutableList<long> GetUsedObjects(this string text)
        {
            /*
                Задача на поиграться с регулярками - вся сложность в том, чтобы аккуратно игнорировать комментарии.
                Экспериментировать онлайн можно, например, здесь: http://regexstorm.net/tester и https://regexr.com/
            */

            /*
             *
             * Непонятно условие - как именно записаны байты идентификатора в тексте?
             * Здесь предполагается что идентификаторы записаны в тексте так:
             *
             * ```
             * Never gonna 31b9:6f8c you up
             * Never gonna let you a3ba:188a
             * Never gonna run around and 14hh:00b5 you
             * Never gonna make you 58f4:7cdc
             * // Never gonna say 4dd9:00a0
             * Never gonna tell 6aac:2f9c and 31b9:6f8c you
             * ```
             *
             */

            HashSet<long> objectIDs = new HashSet<long>();
            MatchCollection matches = Regex.Matches(text, @"(?<!\n[^\n]*\/\/((?!\/\/).)*)(?<!\/\*((?!\*\/)([^$]))*)([\da-f]{4}:[\da-f]{4})");
            foreach (Match m in matches)
            {
                string hex = m.ToString().Substring(0, 4) + m.ToString().Substring(5);
                objectIDs.Add(long.Parse(hex, System.Globalization.NumberStyles.HexNumber));
            }

            return objectIDs.ToImmutableArray();
        }

        #endregion
    }
}
