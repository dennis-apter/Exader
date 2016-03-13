using System;
using System.Collections.Generic;
using Xunit;

namespace Exader
{
    public class ArrayExtensionsFacts
    {
        [Fact]
        public void Add()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = array1.Add(42);

            Assert.Equal(42, array2[3]);

            Assert.False(ReferenceEquals(array1, array2));
        }

        [Fact]
        public void Collapse()
        {
            Assert.Equal(new int?[] { 1, 3, 6 }, new int?[] { 1, null, 3, null, 6, null }.Collapse());
            Assert.Equal(new int?[] { 1, 3, 6 }, new int?[] { 1, null, 3, null, 6 }.Collapse());
            Assert.Equal(new int?[] { 3, 6 }, new int?[] { null, 3, null, 6 }.Collapse());

            var array1 = new int?[] { 1, 2, 3 };
            Assert.True(ReferenceEquals(array1, array1.Collapse()));
        }

        [Fact]
        public void Contains()
        {
            Assert.True(new[] { 1, 2, 3 }.Contains(2));
        }

        [Fact]
        public void EnsureSize()
        {
            int[] array = new[] { 1, 2, 3 }.EnsureSize(3);
            Assert.Equal(3, array.Length);
            Assert.Equal(6, array.EnsureSize(6).Length);
        }

        [Fact]
        public void ExpandLeft()
        {
            var expandedOn1 = new[]/*  */{ 0, 1, 2, 3, 9, 5 };
            expandedOn1.Expand(-1, 4, -5); //       <<<<
            Assert.Equal(new[]/*       */{ 1, 2, 3, 9, 9, 5 }, expandedOn1);

            var expandedOn2 = new[]/*  */{ 0, 1, 2, 3, 9, 5 };
            expandedOn2.Expand(-2, 4, -5); //    <<<<<<<
            Assert.Equal(new[]/*       */{ 2, 3, 9, 9, 9, 5 }, expandedOn2);

            var expandedOn3 = new[]/*  */{ 0, 1, 2, 3, 9, 5 };
            expandedOn3.Expand(-3, 4, -5); // <<<<<<<<<<
            Assert.Equal(new[]/*       */{ 3, 9, 9, 9, 9, 5 }, expandedOn3);
        }

        [Fact]
        public void ExpandRight()
        {
            var expandedOn1 = new[] { 0, 9, 5, 6, 7 };
            expandedOn1.Expand(1, 1); // >>>>
            Assert.Equal(new[]/*  */{ 0, 9, 9, 5, 6 }, expandedOn1);

            var expandedOn2 = new[] { 0, 9, 5, 6, 7 };
            expandedOn2.Expand(2, 1); // >>>>>>>
            Assert.Equal(new[]/*  */{ 0, 9, 9, 9, 5 }, expandedOn2);

            var expandedOn3 = new[] { 0, 9, 5, 6, 7 };
            expandedOn3.Expand(3, 1); // >>>>>>>>>>
            Assert.Equal(new[]/*  */{ 0, 9, 9, 9, 9 }, expandedOn3);
        }

        [Fact]
        public void Fill()
        {
            var array = new int[5];
            array.Fill(new[] { 1, 2, 3 });

            Assert.Equal(new[] { 1, 2, 3, 1, 2 }, array);
        }

        [Fact]
        public void Growth()
        {
            Assert.Equal(6, new[] { 1, 2, 3 }.Growth(3).Length);
        }

        [Fact]
        public void Insert()
        {
            int[] array = new[] { 1, 3, 4 }.Insert(1, 2);
            Assert.Equal(4, array.Length);
            Assert.Equal(1, array[0]);
            Assert.Equal(2, array[1]);
            Assert.Equal(3, array[2]);

            array = array.Insert(4, 5, 6, 7);
            Assert.Equal(7, array.Length);
            Assert.Equal(4, array[3]);
            Assert.Equal(5, array[4]);
            Assert.Equal(6, array[5]);
            Assert.Equal(7, array[6]);

            array = array.InsertRange(7, new Stack<int>(new[] { 0, 9, 8 }));
            Assert.Equal(10, array.Length);
            Assert.Equal(7, array[6]);
            Assert.Equal(8, array[7]);
            Assert.Equal(9, array[8]);
            Assert.Equal(0, array[9]);
        }

        [Fact]
        public void Move()
        {
            Assert.Equal(new[] { 1, 1, 2 }, new[] { 1, 2, 3 }.Move(1));
            Assert.Equal(new[] { 1, 2, 1 }, new[] { 1, 2, 3 }.Move(2));
            Assert.Equal(new[] { 2, 3, 3 }, new[] { 1, 2, 3 }.Move(-1));
            Assert.Equal(new[] { 3, 2, 3 }, new[] { 1, 2, 3 }.Move(-2));
        }

        [Fact]
        public void Remove()
        {
            Assert.Equal(new[] { 1, 3, 2 }, new[] { 1, 2, 3, 2 }.Remove(2));
            Assert.Equal(new[] { 1, 3 }, new[] { 1, 3, 2 }.Remove(2));
        }

        [Fact]
        public void RemoveAt()
        {
            int[] array = new[] { 1, 2, 3 }.RemoveAt(2);
            Assert.Equal(2, array.Length);
            Assert.Equal(1, array[0]);
            Assert.Equal(2, array[1]);
        }

        [Fact]
        public void RemoveLast()
        {
            Assert.Equal(new[] { 1, 2, 3 }, new[] { 1, 2, 3, 2 }.RemoveLast(2));
            Assert.Equal(new[] { 1, 3 }, new[] { 1, 3, 2 }.RemoveLast(2));
        }

        [Fact]
        public void Resize()
        {
            int[] array1 = { 1, 2, 3 };
            int[] array2 = array1.Resize(3);
            Assert.True(ReferenceEquals(array1, array2));

            array2 = array1.Resize(6);
            Assert.False(ReferenceEquals(array1, array2));
            Assert.Equal(6, array2.Length);
        }

        [Fact]
        public void Shift()
        {
            Assert.Equal(new[] { 1, 1, 2, 3 }, new[] { 1, 2, 3 }.Shift(1));
            Assert.Equal(new[] { 1, 2, 1, 2, 3 }, new[] { 1, 2, 3 }.Shift(2));
            Assert.Equal(new[] { 2, 3 }, new[] { 1, 2, 3 }.Shift(-1));
            Assert.Equal(new[] { 3 }, new[] { 1, 2, 3 }.Shift(-2));
            Assert.Equal(new int[0], new[] { 1, 2, 3 }.Shift(-3));
        }

        [Fact]
        public void ShiftOveflow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1, 2, 3 }.Shift(-4));
        }

        [Fact]
        public void Shrink()
        {
            Assert.Equal(3, new[] { 1, 2, 3, 5, 6, 7 }.Shrink(3).Length);
        }
    }
}
