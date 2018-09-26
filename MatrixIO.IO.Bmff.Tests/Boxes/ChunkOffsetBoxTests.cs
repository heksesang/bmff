using Xunit;

namespace MatrixIO.IO.Bmff.Boxes.Tests
{
    public class ChunkOffsetBoxTests
    {
        [Fact]
        public void CanRoundtrip()
        {
            var box = new ChunkOffsetBox
            {
                Entries = new ChunkOffsetBox.ChunkOffsetEntry[]
                {
                    new ChunkOffsetBox.ChunkOffsetEntry(797),
                    new ChunkOffsetBox.ChunkOffsetEntry(13096)
                }
            };

            var deserialized = (ChunkOffsetBox)TestHelper.Decode(TestHelper.Encode(box));
            
            Assert.Equal(box.Entries, deserialized.Entries);
        }
    }
}