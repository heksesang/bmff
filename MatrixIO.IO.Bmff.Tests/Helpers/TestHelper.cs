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


        public static string GetTestFilePath(string name)
        {
            var basePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent;

            return Path.Combine(basePath.FullName, "TestData", "chicago.mp4");
        }
    }

}
