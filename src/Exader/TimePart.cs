namespace Exader
{
    public enum TimePart
    {
        None = 0,

        Millisecond = 1,

        Second = 2,

        Minute = 3,

        Hour = 4,

        /// <summary>
        /// Сутки.
        /// </summary>
        Day = 5,

        Week = 6,

        /// <summary>
        /// Десятидневка.
        /// </summary>
        Decade = 7,

        /// <summary>
        /// Двухнедельный период.
        /// </summary>
        Fortnight = 8,
    }
}
