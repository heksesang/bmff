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

                var fileType  = (FileTypeBox)boxes[0];
                var movie = (MovieBox)boxes[1];
                var freeSpace = (FreeSpaceBox)boxes[2];
                var movieData = (MovieDataBox)boxes[3];

                // File Type
                Assert.Equal(0u, fileType.Offset);
                Assert.Equal(32u, fileType.ContentOffset);
                Assert.Equal(Guid.Parse("70797466-1100-1000-8000-00aa00389b71"), fileType.Type.UserType);

                // Movie
                Assert.Equal(3, movie.Children.Count);
                
                // FreeSpace
                Assert.Equal(982u, freeSpace.ContentOffset);

                // Movie Data
                Assert.Equal(990u, movieData.ContentOffset);
                Assert.Equal(1314169u, movieData.ContentSize);
            }
        }
    }
}