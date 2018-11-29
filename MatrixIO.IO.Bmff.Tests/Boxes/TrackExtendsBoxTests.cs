using Xunit;

namespace MatrixIO.IO.Bmff.Boxes.Tests
{
    public class TrackExtendsBoxTests
    {
        [Fact]
        public void CanRoundtrip()
        {
            var box = new TrackExtendsBox
            {
                TrackID = 18,
                DefaultSampleDependsOn = 1,
                DefaultDegredationPriority = 1,
                DefaultSampleIsDependedOn = 1
                
            };

            var deserialized = TestHelper.Decode<TrackExtendsBox>(TestHelper.Encode(box));

            Assert.Equal(18u, box.TrackID);
            Assert.Equal(1,   deserialized.DefaultSampleDependsOn);
            Assert.Equal(1,   deserialized.DefaultDegredationPriority);
            Assert.Equal(1,   deserialized.DefaultSampleIsDependedOn);
        }
    }
}