using IllustrationGlossaryPackage.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllustrationGlossaryPackage.Core.Infrastructure
{
    public class Archiver : IArchiver
    {
        /// <summary>
        /// Copies the zip file into an archive directory
        /// </summary>
        /// <param name="zipFile"></param>
        public void CreateArchive(string zipFile)
        {
            if (zipFile.EndsWith(".zip"))
            {
                FileInfo fi = new FileInfo(zipFile);
                string zipFileName = fi.Name;
                string zipFileDirectory = fi.Directory.FullName;
                string archiveDirectory = zipFileDirectory + "/Archive";
                string archiveFileName = "/" + zipFileName.Substring(0, zipFileName.LastIndexOf(".zip")) + "_Archive.zip";
                string archiveDestination = archiveDirectory + archiveFileName;

                Console.WriteLine("Creating Directory: Archive");
                Directory.CreateDirectory(archiveDirectory);

                Console.WriteLine("Copying {0} into Archive directory", zipFileName);
                if (!File.Exists(archiveDestination))
                {
                    File.Copy(zipFile, archiveDestination);
                }
            }
        }
    }
}
