namespace MatrixIO.IO.MpegTs
{
    public enum NalUnitTypeCode
    {
        Unspecified = 0,
        SliceLayerWithoutPartitioningNonIDR = 1,
        SliceDataPartitionLayerA = 2,
        SliceDataPartitionLayerB = 3,
        SliceDataPartitionLayerC = 4,
        SliceLayerWithoutPartitioningIDR = 5,
        SupplementalEnahancementInformation = 6,
        SequanceParameterSet = 7,
        PictureParameterSet = 8,
        AccessUnitDelimiter = 9,
        EndOfSequence = 10,
        EndOfStream = 11,
        FillerData = 12,
        // 13..23 Reserved
        // 24..31 Unspecified
    }
}