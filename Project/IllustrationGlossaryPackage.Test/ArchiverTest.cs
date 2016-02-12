﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IllustrationGlossaryPackage.Core.Interfaces;
using IllustrationGlossaryPackage.Core.Infrastructure;
using System.IO;
using System.IO.Compression;

namespace IllustrationGlossaryPackage.Test
{
    [TestClass]
    public class ArchiverTest
    {
        [TestMethod]
        public void CreateArchiveTest()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string zipDirectory = "zipDirectory";
            string zipFolderPath = currentDirectory + "\\" + zipDirectory;
            Directory.CreateDirectory(zipFolderPath);

            string nestedFile = "thisIsMyArchiveFile.txt";
            string nestedFilePath = zipFolderPath + "\\" + nestedFile;
            string fileContents = "THIS IS THE CONTENTS OF MY FILE";
            File.Create(nestedFilePath).Close();
            File.AppendAllText(nestedFilePath, fileContents);

            string zippedFiles = "zippedDirectory";
            ZipFile.CreateFromDirectory(zipFolderPath, zippedFiles + ".zip");
            string zipPath = currentDirectory + "\\" + zippedFiles + ".zip";

            IArchiver archiver = new Archiver();
            archiver.CreateArchive(zipPath);

            string archiveDirectory = currentDirectory + "\\" + "Archive";
            // TODO Test that the archive was created correctly

            Directory.Delete(zipFolderPath, true);
            Directory.Delete(archiveDirectory, true);
            File.Delete(zipPath);
        }
    }
}
