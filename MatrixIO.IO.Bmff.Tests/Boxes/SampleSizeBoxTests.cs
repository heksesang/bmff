using System;
using Xunit;

namespace MatrixIO.IO.Bmff.Boxes.Tests
{
    public class SampleSizeBoxTests
    {
        [Fact]
        public void CanRoundtrip()
        {
            var entries = new[] {
                new SampleSizeBox.SampleSizeEntry(977),
                new SampleSizeBox.SampleSizeEntry(938),
                new SampleSizeBox.SampleSizeEntry(939),
                new SampleSizeBox.SampleSizeEntry(938),
                new SampleSizeBox.SampleSizeEntry(934),
                new SampleSizeBox.SampleSizeEntry(945),
                new SampleSizeBox.SampleSizeEntry(948),
                new SampleSizeBox.SampleSizeEntry(956),
                new SampleSizeBox.SampleSizeEntry(955),
                new SampleSizeBox.SampleSizeEntry(930),
                new SampleSizeBox.SampleSizeEntry(933),
                new SampleSizeBox.SampleSizeEntry(934),
                new SampleSizeBox.SampleSizeEntry(972),
                new SampleSizeBox.SampleSizeEntry(977),
                new SampleSizeBox.SampleSizeEntry(958),
                new SampleSizeBox.SampleSizeEntry(949),
                new SampleSizeBox.SampleSizeEntry(962),
                new SampleSizeBox.SampleSizeEntry(848)
            };

            var box = new SampleSizeBox
            {
                Entries = entries
            };

            var deserialized = (SampleSizeBox)TestHelper.Decode(TestHelper.Encode(box));

            Assert.Equal(0,     deserialized.Version);
            Assert.Equal(0u,    deserialized.SampleSize); // variable
            Assert.Equal(18u,   deserialized.SampleCount);

            Assert.Equal(box.Entries, deserialized.Entries);
        }
    }
}