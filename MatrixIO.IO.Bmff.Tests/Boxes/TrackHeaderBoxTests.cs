using System;

using Xunit;

namespace MatrixIO.IO.Bmff.Boxes.Tests
{
    public class TrackHeaderBoxTests
    {
        [Fact]
        public void CanDecode()
        {
            var data = Convert.FromBase64String("AAAAXHRraGQAAAADAAAAAAAAAAAAAAABAAAAAAAABkAAAAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAABAAAAAAyAAAAGaAAA=");

            var box = (TrackHeaderBox)TestHelper.Decode(data);

            Assert.Equal(92u,    box.Size);
            Assert.Equal(0,      box.Version);
            Assert.Equal(1u,     box.TrackID);
            Assert.Equal(1600ul, box.Duration);
            Assert.Equal(0,      box.Layer);
            Assert.Equal(0,      box.AlternateGroup);
            Assert.Equal(0,      box.Volume);

            Assert.True(box.Flags[16]);
            Assert.True(box.Flags[17]);

            // Ensure it reencodes to the same bytes
            Assert.True(data.AsSpan().SequenceEqual(TestHelper.Encode(box)));
        }
    }
}