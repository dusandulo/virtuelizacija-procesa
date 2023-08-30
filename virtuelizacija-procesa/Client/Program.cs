using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client started....\n\n");
            while (true)
            {
                Console.WriteLine("===================================================");
                Console.WriteLine("Enter the path of the folder from which you want to read the data: ");
                string path = Console.ReadLine();

                //provera foldera
                if (Directory.Exists(path))
                {
                    int temp = 1;

                    foreach (string filePath in Directory.GetFiles(path, "*.csv"))
                    {
                        if (path.Contains("forecast")) //provera da li se salje forecast ili measured
                        {
                            SendFile(filePath, true);
                            Console.WriteLine(temp + ". - Successfully!");
                        }
                        else if (path.Contains("measured"))
                        {
                            SendFile(filePath, false);
                            Console.WriteLine(temp + ". - Successfully!");
                        }
                        temp++;

                    }
                    temp = 0;
                    Console.WriteLine("Import completed");
                }
                else
                {
                    Console.WriteLine("Folder does not exist.");
                }
            }
        }

        private static void SendFile(string path, bool forecastBool)
        {
            MemoryStream memoryStream = new MemoryStream();

            ChannelFactory<IFile> channel = new ChannelFactory<IFile>("ClientParser");
            IFile proxy = channel.CreateChannel();


            using (FileStream csvFileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                csvFileStream.CopyTo(memoryStream);
            }

            memoryStream.Position = 0;

            using (FileHandle options = new FileHandle(memoryStream, Path.GetFileName(path)))
            {
                proxy.ParseFile(options, forecastBool);
                options.Dispose();
            }
        }
    }
}
