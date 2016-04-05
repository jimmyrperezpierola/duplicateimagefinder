using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Timers;
using System.Threading;

namespace MultiImageLoader
{
    class Program
    {
        private static PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);


        static void Main(string[] args)
        {

            var initialPath = @"C:\Pankaj\delete";

            var files = Directory.EnumerateFiles(initialPath);

            Dictionary<string,string> filehashes = new Dictionary<string, string>();
            var i = 1;

            foreach (var file in files)
            {
                Console.WriteLine(string.Format("Loading {0} of {1} : {2}",i++,files.Count(), file));
                var bitmap = LoadBitmap(file);
                var byteArr = ImageToByte(bitmap);

                var hash = ComputeHash(byteArr);
                Console.WriteLine(string.Format("Hash : {0}", hash));

                filehashes.Add(file,hash);

                bitmap.Dispose();
                byteArr = null;

                PrintMemoryConsumption();

                //Thread.Sleep(2000);
            }


            var distinctHashes = (from element in filehashes
                                  select element.Key).Distinct();

            var uniqueValues = filehashes.GroupBy(pair => pair.Value)
                         .Select(group => group.First())
                         .ToDictionary(pair => pair.Key, pair => pair.Value);

            Console.WriteLine("*******************");
            Console.WriteLine("Unique values : ");
            foreach (var item in uniqueValues)
            {
                Console.WriteLine(item.Key);
            }

            Console.ReadLine();

        }

        public static void PrintMemoryConsumption()
        {
            Console.WriteLine(Convert.ToInt32(ramCounter.NextValue()).ToString() + "Mb");
            Console.WriteLine("--------------------------------");
        }

        public static string ComputeHash(byte[] byteArray)
        {
            var hash = string.Empty;
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                hash = Convert.ToBase64String(sha1.ComputeHash(byteArray));
            }

            return hash;
        }
        public static Bitmap LoadBitmap(string path)
        {
            //Open file in read only mode
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //Get a binary reader for the file stream
            using (BinaryReader reader = new BinaryReader(stream))
            {
                //copy the content of the file into a memory stream
                var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                //make a new Bitmap object the owner of the MemoryStream
                return new Bitmap(memoryStream);
            }
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }

}
