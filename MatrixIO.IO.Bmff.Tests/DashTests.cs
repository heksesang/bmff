using System;
using System.IO;
using System.Linq;
using MatrixIO.IO.Bmff.Boxes;
using MatrixIO.IO.Bmff.IO.Bmff.Boxes.Dash;
using Xunit;

namespace MatrixIO.IO.Bmff.Tests
{
    public class DashTests
    {
        [Fact]
        public void CanReadMp4Segment()
        {
            using (var stream = File.Open(TestHelper.GetTestFilePath("0_000.m4s"), FileMode.Open))
            {
                var reader = new BmffReader(stream);

                var boxes = reader.RootBoxes;

                var styp = (SegmentTypeBox)reader.RootBoxes[0];
                var sidx_1 = (SegmentIndexBox)reader.RootBoxes[1]; // Video
                var sidx_2 = (SegmentIndexBox)reader.RootBoxes[2]; // Audio
                var moof = (MovieFragmentBox)reader.RootBoxes[3];
                var mdat = (MovieDataBox)reader.RootBoxes[4];

                Assert.Equal(0ul, styp.Offset.Value);
                Assert.Equal(24u, styp.Size);

                Assert.Equal("msdh", styp.MajorBrand.ToString());
                Assert.Equal(new[] { "msdh", "msix" }, styp.CompatibleBrands.Select(b => b.ToString()).ToArray());

                Assert.Equal(1, sidx_1.Version);
                Assert.Equal(1u, sidx_1.ReferenceId);
                Assert.Equal(24000u, sidx_1.Timescale);
                Assert.Equal(2002ul, sidx_1.EarliestPresentationTime);
                Assert.Equal(52ul, sidx_1.FirstOffset);
                Assert.Equal(1, sidx_1.ReferenceCount);

                Assert.Equal((false, 198178u, 48048u, true, 0u, 0u), (
                    sidx_1.Entries[0].ReferenceType,
                    sidx_1.Entries[0].ReferenceSize,
                    sidx_1.Entries[0].Duration,
                    sidx_1.Entries[0].StartWithSap,
                    sidx_1.Entries[0].SapType,
                    sidx_1.Entries[0].SapDeltaTime));

                Assert.Equal(1, sidx_2.Version);
                Assert.Equal(2u, sidx_2.ReferenceId);
                Assert.Equal(44100u, sidx_2.Timescale);
                Assert.Equal(2655u, sidx_2.EarliestPresentationTime);
                Assert.Equal(0ul, sidx_2.FirstOffset);
                Assert.Equal(1, sidx_2.ReferenceCount);

                Assert.Equal(86016u, sidx_2.Entries[0].Duration);
            }
        }
    }
}