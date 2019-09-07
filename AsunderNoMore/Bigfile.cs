using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsunderNoMore
{
    class Bigfile
    {
        List<BigfileEntry> _bigFileEntries = new List<BigfileEntry>();
        public bool IsValid { get; } = false;
        public string Error { get; } = "";

        public Bigfile(string folderName)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderName);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileInfos)
                {
                    string filePath = fileInfo.FullName.Substring(folderName.Length);
                    BigfileEntry bigFileEntry = new BigfileEntry();
                    bigFileEntry.FilePath = filePath;
                    bigFileEntry.FileHash = getSR1HashName(filePath);
                    bigFileEntry.FileLength = fileInfo.Length;
                    _bigFileEntries.Add(bigFileEntry);
                }

                IsValid = true;
            }
            catch (Exception exception)
            {
                IsValid = false;
                Error = exception.Message;
            }
        }

        private uint getSR1HashName(string fileName)
        {
            int charsRead = 0, x1 = 0, x2 = 0;
            int currentLetter = 0, hashName = 0, index = 0, extID = 0;
            string extension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
            string[] extensions = { "pcm", "crm", "tim", "smp", "snd", "smf", "snf" };
            for (index = 0; index < 7; index++)
            {
                if (extension.Equals(extensions[index]))
                {
                    extID = index;
                    break;
                }
            }
            if (index < 7) index = fileName.Length - 5;
            else index = fileName.Length - 1;
            while (index >= 0)
            {
                currentLetter = (int)fileName[index];
                if (currentLetter >= 'a' && currentLetter <= 'z')
                {
                    currentLetter &= 0xDF;
                }
                if (currentLetter == '\\')
                {
                    index--;
                    continue;
                }
                currentLetter += 0xE6;
                currentLetter &= 0xFF;
                x1 = charsRead;
                x1 *= currentLetter;
                x2 += currentLetter;
                hashName ^= x1;
                charsRead++;
                index--;
            }
            charsRead <<= 0x0C;
            charsRead |= x2;
            charsRead <<= 0x0C;
            hashName |= charsRead;
            hashName <<= 0x03;
            hashName |= extID;
            return (uint)hashName;
        }
    }
}