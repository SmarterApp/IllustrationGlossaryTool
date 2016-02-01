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
            string archiveSuffix = Properties.Resources.ArchiveSuffix;
            string archiveDir = Properties.Resources.ArchiveDir;

            if (zipFile.EndsWith(zipFileExt))
            {
                FileInfo fi = new FileInfo(zipFile);
                string zipFileName = fi.Name;
                string zipFileDirectory = fi.Directory.FullName;
                
                string archiveFullPath = GetArchiveDestination(zipFileName, zipFileDirectory);
                string archiveDirectory = zipFileDirectory + "/" + archiveDir;

                Directory.CreateDirectory(archiveDirectory);
                if (!File.Exists(archiveFullPath))
                {
                    File.Copy(zipFile, archiveFullPath);
                }

                else
                {
                    throw new ArchiveAlreadyExistsException(archiveFullPath);
                }
            }
        }

        /// <summary>
        /// Gets a full path destination for the archive file and increments a 
        ///     value to make sure that the archive file name is unique.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="baseDirectory"></param>
        /// <returns></returns>
        private string GetArchiveDestination(string baseName, string baseDirectory)
        {
            string zipFileExt = Properties.Resources.ZipFileExtention;
            string archiveSuffix = Properties.Resources.ArchiveSuffix;
            string archiveDir = Properties.Resources.ArchiveDir;
            string archiveDirectory = baseDirectory + "/" + archiveDir;
            string archiveBaseName = baseName.Substring(0, baseName.LastIndexOf(zipFileExt)) + archiveDir;
            string archivePathJustName = archiveDirectory + "/" + archiveBaseName;
            string archiveFullPath = archivePathJustName + zipFileExt;
            int i = 0;
            if (File.Exists(archiveFullPath))
            {
                string archiveTempBaseName = archiveBaseName + "_" + i;
                archiveFullPath = archiveDirectory + "/" + archiveBaseName + zipFileExt;
                while (File.Exists(archiveFullPath))
                {
                    i += 1;
                    archiveTempBaseName = archiveBaseName + "_" + i;
                    archiveFullPath = archiveDirectory + "/" + archiveTempBaseName + zipFileExt;
                }
            }

            return archiveFullPath;
        }
    }
}
