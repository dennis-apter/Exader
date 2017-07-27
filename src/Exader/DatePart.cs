namespace Exader
{
    public enum DatePart
    {
        None = 0,
        /// <summary>
        /// ����������� ���� ������.
        /// </summary>
        Day = 1,
        /// <summary>
        /// ����������� ������ ������ (1, 2, � 5, 6).
        /// </summary>
        WeekOfMonth = 2,
        /// <summary>
        /// ����������� ������ ���� (1, 2, � 52, 53).
        /// </summary>
        WeekOfYear = 3,
        Month = 4,
        Quarter = 5,
        Year = 6,
        /// <summary>
        /// �����������.
        /// </summary>
        Decade = 7,
        Century = 8,
        Millennium = 9,
    }
}
