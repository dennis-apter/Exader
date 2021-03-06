﻿using Exader.IO;
using Xunit;

namespace Exader
{
    public static class TestData
    {
        public static readonly FilePath Path = FilePaths.ExecutingAssemblyDirectory.SubpathBefore("test") / "data/";

        public static FilePath Get(string fileName)
        {
            var path = Path / fileName;
            Assert.True(path.IsFileExists);
            return path;
        }
    }
}
