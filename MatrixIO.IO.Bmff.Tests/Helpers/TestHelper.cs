using System.IO;

namespace MatrixIO.IO.Bmff
{
    public class TestHelper
    {
        public static byte[] Encode(Box box)
        {
            using (var ms = new MemoryStream())
            {
                box.ToStream(ms);

                return ms.ToArray();
            }
        }

        public static Box Decode(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return Box.FromStream(ms);
            }
        }
    }

}
