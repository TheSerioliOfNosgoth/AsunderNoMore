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
                    string fileExt = Path.GetExtension(fileInfo.FullName);
                    int endOfName = filePath.Length - fileExt.Length;

                    string fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                    if (fileName == "bigfile")
                    {
                        continue;
                    }

                    BigfileEntry bigFileEntry = new BigfileEntry();
                    bigFileEntry.FilePath = filePath;
                    bigFileEntry.FileHash = getSR1HashName(filePath);
                    bigFileEntry.FileLength = (uint)fileInfo.Length;
                    bigFileEntry.FileCode.code0 = char.ToUpperInvariant(filePath[endOfName - 4]);
                    bigFileEntry.FileCode.code1 = char.ToUpperInvariant(filePath[endOfName - 3]);
                    bigFileEntry.FileCode.code2 = char.ToUpperInvariant(filePath[endOfName - 2]);
                    bigFileEntry.FileCode.code3 = char.ToUpperInvariant(filePath[endOfName - 1]);
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

        public bool CreateBigfile(string fileName)
        {
            if (!IsValid)
            {
                return false;
            }

            FileStream file = new FileStream(fileName, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(_bigFileEntries.Count);

            uint offset = 0; // Offset from start of the bigfile or start of the contained files.
            foreach (BigfileEntry entry in _bigFileEntries)
            {
                writer.Write(entry.FileHash);
                writer.Write(entry.FileLength);
                writer.Write(offset);
                writer.Write(entry.FileCode.code);

                offset += entry.FileLength;
            }
            writer.Flush();

            // TODO - Append the actual files here.

            writer.Close();
            file.Close();

            return true;
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