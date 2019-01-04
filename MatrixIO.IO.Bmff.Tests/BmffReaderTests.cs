using System;
using System.IO;
using MatrixIO.IO.Bmff.Boxes;

using Xunit;

namespace MatrixIO.IO.Bmff.Tests
{
    public class BmffReaderTests
    {
        [Fact]
        public void CanReadMp4()
        {
            using (var stream = File.Open(TestHelper.GetTestFilePath("chicago.mp4"), FileMode.Open))
            {
                var reader = new BmffReader(stream);

                var boxes = reader.RootBoxes;

                var ftpy = (FileTypeBox)boxes[0];
                var moov = (MovieBox)boxes[1];
                var free = (FreeSpaceBox)boxes[2];
                var mdat = (MovieDataBox)boxes[3];

                // File Type
                Assert.Equal(0u, ftpy.Offset);
                Assert.Equal(32u, ftpy.ContentOffset);
                Assert.Equal(Guid.Parse("70797466-1100-1000-8000-00aa00389b71"), ftpy.Type.UserType);

                // Movie
                Assert.Equal(3, moov.Children.Count);

                // FreeSpace
                Assert.Equal(982u, free.ContentOffset);

                // Movie Data
                Assert.Equal(990u, mdat.ContentOffset);
                Assert.Equal(1314169u, mdat.ContentSize);
            }
        }
    }
}