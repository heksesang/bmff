using System;
using Xunit;

namespace MatrixIO.IO.Bmff.Boxes.Tests
{
    public class MediaHeaderBoxTests
    {
        [Fact]
        public void CanRoundtrip()
        {
            var box = new MediaHeaderBox
            {
                CreationTime = new DateTime(2015, 10, 01, 12, 45, 13, DateTimeKind.Utc),
                ModificationTime = new DateTime(2018, 01, 18, 12, 5, 0, DateTimeKind.Utc),
                TimeScale = 48000,
                Duration = 34560,
                Language = "und"
            };

            var deserialized = (MediaHeaderBox)TestHelper.Decode(TestHelper.Encode(box));

            Assert.Equal(box.CreationTime, deserialized.CreationTime);
            Assert.Equal(box.ModificationTime, deserialized.ModificationTime);

            Assert.Equal(box.TimeScale, deserialized.TimeScale);
            Assert.Equal(box.Duration, deserialized.Duration);
            Assert.Equal(box.Language, deserialized.Language);
        }
    }
}