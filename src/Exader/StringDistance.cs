using System;
using JetBrains.Annotations;

namespace Exader
{
    /// <summary>
    /// http://grantorinoteam.blogspot.com/2009/03/blog-post.html
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class StringDistance
    {
        /// <summary>
        /// ¬ычисление меры схожести двух предложений             
        /// ѕолучение дистанции дл€ двух слов
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static int GetWordDistance(string s, string t)
        {
            try
            {
                // шаг 1
                if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t))
                {
                    throw new Exception("ќдно из слов не задано.");
                }

                s = s.ToLower();
                t = t.ToLower();
                int n = s.Length;// длина строки s    
                int m = t.Length; // длина строки t      
                var d = new int[n + 1, m + 1];

                // матрица дл€ хранени€ вычислений     
                // шаг 2 
                for (int i = 0; i <= n; i++)
                {
                    d[i, 0] = i;
                }

                for (int j = 0; j <= m; j++)
                {
                    d[0, j] = j;
                }

                // шаг 3
                for (int i = 1; i <= n; i++)
                {
                    int si = s[i - 1];
                    for (int j = 1; j <= m; j++)
                    {
                        int tj = t[j - 1];
                        int cost = 0;
                        // определение оценки          
                        if (si != tj)
                            cost = 1;
                        // получить минимум дл€ €чейки матрцы           
                        d[i, j] = Minimum(d[i - 1, j] + 1, d[i, j - 1] + 1, d[i - 1, j - 1] + cost);
                    }
                }
                
                int dist = d[n, m]; // дистанци€ редактировани€   
                // получение относительного числа похожести      
                var procent = (int)Math.Round((1 - dist / (double)Math.Max(s.Length, t.Length)) * 100, 0);
                return procent;
            }
            catch (Exception ex)
            {
                throw new Exception("ќшибка при сравнении слов", ex);
            }
        }

        /// ћинимум из трех значений                           
        private static int Minimum(int a, int b, int c)
        {
            int min = a;
            if (b < min)
            {
                min = b;
            }

            if (c < min)
            {
                min = c;
            }

            return min;
        }

        private readonly string _sentenceOne;
        private readonly string _sentenceTwo;

        /// <summary>
        /// јбсолютное значение функции похожести
        /// </summary>
        private int _absoluteMeasure;

        /// <summary>
        /// ќтносительное значение функции похожести
        /// </summary>
        private int _procentMeasure;

        public StringDistance(string sentenceOne, string sentenceTwo)
        {
            this._sentenceOne = sentenceOne;
            this._sentenceTwo = sentenceTwo;
            SentenceSeemsMeasure();
        }

        private void SentenceSeemsMeasure()
        {
            // получение слов в предложени€х                      
            string[] sOneWords = _sentenceOne.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string[] sTwoWords = _sentenceTwo.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // заполнение матрицы отношений между словами предложени€           
            var wordRelations = new int[sOneWords.Length, sTwoWords.Length];

            // значени€ максимальных значений в строках матрицы         
            var maxRowElements = new int[sOneWords.Length];

            try
            {
                for (int i = 0; i < sOneWords.Length; i++)
                {
                    int max = 0;
                    for (int j = 0; j < sTwoWords.Length; j++)
                    { 
                        // получение дистанции дл€ пары слов в предлажении     
                        wordRelations[i, j] = GetWordDistance(sOneWords[i], sTwoWords[j]);

                        // сравение с максимальным значением              
                        if (max < wordRelations[i, j])
                        {
                            max = wordRelations[i, j];
                        }
                    }

                    maxRowElements[i] = max;
                }

                // получени€ меры схожести между предложени€ми        
                int measure = 0;
                for (int i = 0; i < maxRowElements.Length; i++)
                {
                    int max = maxRowElements[i];
                    for (int j = 0; j < sTwoWords.Length; j++)
                    {
                        if (max < wordRelations[i, j])
                        {
                            max = wordRelations[i, j];
                        }
                    }

                    measure += max;
                }

                _absoluteMeasure = measure;

                // получени€ относительного значени€ схожести двух предложений
                _procentMeasure = (int)Math.Round(
                    measure / (double)Math.Max(sOneWords.Length, sTwoWords.Length), 0);
            }
            catch (Exception error)
            {
                throw new Exception("ќшибка при сравнении предложений", error);
            }
        }

        public int AbsoluteMeasure { get {  return _absoluteMeasure; } }
        public int ProcentMeasure { get { return _procentMeasure; } }
    }
}