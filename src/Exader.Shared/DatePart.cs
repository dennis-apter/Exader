namespace Exader
{
    public enum DatePart
    {
        None = 0,
        /// <summary>
        ///  алендарный день мес€ца.
        /// </summary>
        Day = 1,
        /// <summary>
        ///  алендарна€ недел€ мес€ца (1, 2, Е 5, 6).
        /// </summary>
        WeekOfMonth = 2,
        /// <summary>
        ///  алендарна€ недел€ года (1, 2, Е 52, 53).
        /// </summary>
        WeekOfYear = 3,
        Month = 4,
        Quarter = 5,
        Year = 6,
        /// <summary>
        /// ƒес€тилетие.
        /// </summary>
        Decade = 7,
        Century = 8,
        Millennium = 9,
    }
}
