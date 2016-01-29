using IllustrationGlossaryPackage.Core.Exceptions;
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
            string zipFileExt = Properties.Resources.ZipFileExtention;

            if (zipFile.EndsWith(zipFileExt))
            {
                FileInfo fi = new FileInfo(zipFile);
                string zipFileName = fi.Name;
                string zipFileDirectory = fi.Directory.FullName;
                string archiveDir = Properties.Resources.ArchiveDir;
                string archiveDirectory = zipFileDirectory + "/" + archiveDir;
                string archiveSuffix = Properties.Resources.ArchiveSuffix;
                string archiveFileName = zipFileName.Substring(0, zipFileName.LastIndexOf(zipFileExt)) + archiveSuffix;
                string archiveDestination = archiveDirectory + "/" + archiveFileName;

                Directory.CreateDirectory(archiveDirectory);
                if (!File.Exists(archiveDestination))
                {
                    File.Copy(zipFile, archiveDestination);
                }

                else
                {
                    throw new ArchiveAlreadyExistsException(archiveDestination);
                }
            }
        }
    }
}
