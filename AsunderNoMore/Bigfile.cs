using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsunderNoMore
{
    class Bigfile
    {
        List<BigfileEntry> _bigFileEntries = new List<BigfileEntry>();
        public string Error { get; set; } = "";

        public Bigfile()
        {
        }

        public bool Import(string folderName, string compareFileName)
        {
            Error = "";

            try
            {
                _bigFileEntries.Clear();

                bool useCompareFile = false;// (compareFileName != "");
                if (useCompareFile)
                {
                    FileStream inputFile = new FileStream(compareFileName, FileMode.Open);
                    BinaryReader reader = new BinaryReader(inputFile);

                    uint entryCount = reader.ReadUInt32();
                    while (entryCount > 0)
                    {
                        BigfileEntry entry = new BigfileEntry();
                        entry.FileHash = reader.ReadUInt32();
                        entry.FileLength = reader.ReadUInt32();
                        entry.FileOffset = reader.ReadUInt32();
                        entry.FileCode.code = reader.ReadUInt32();
                        _bigFileEntries.Add(entry);

                        entryCount--;
                    }

                    reader.Close();
                    inputFile.Close();
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(folderName);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

                if (fileInfos.Length <= 0)
                {
                    throw new Exception("No files in \"" + folderName + "\".");
                }

                foreach (FileInfo fileInfo in fileInfos)
                {
                    string relativePath = fileInfo.FullName.Substring(Directory.GetParent(folderName).FullName.Length);
                    string extension = Path.GetExtension(fileInfo.FullName);
                    int endOfName = relativePath.Length - extension.Length;
                    uint hashID = getSR1HashName(relativePath);

                    BigfileEntry newEntry = new BigfileEntry();
                    newEntry.FilePath = fileInfo.FullName;
                    newEntry.FileHash = hashID;
                    newEntry.FileLength = (uint)fileInfo.Length;
                    newEntry.FileCode.code0 = char.ToUpperInvariant(relativePath[endOfName - 4]);
                    newEntry.FileCode.code1 = char.ToUpperInvariant(relativePath[endOfName - 3]);
                    newEntry.FileCode.code2 = char.ToUpperInvariant(relativePath[endOfName - 2]);
                    newEntry.FileCode.code3 = char.ToUpperInvariant(relativePath[endOfName - 1]);

                    if (useCompareFile)
                    {
                        BigfileEntry existingEntry = _bigFileEntries.Find(x => x.FileHash == newEntry.FileHash);
                        if (existingEntry == null)
                        {
                            throw new Exception("No entry found for \"" + relativePath + "\"");
                        }

                        existingEntry.FilePath = newEntry.FilePath;
                        existingEntry.FileHash = newEntry.FileHash;
                        existingEntry.FileLength = newEntry.FileLength;
                        existingEntry.FileOffset = newEntry.FileOffset;
                        existingEntry.FileCode = newEntry.FileCode;
                    }
                    else
                    {
                        _bigFileEntries.Add(newEntry);
                    }
                }
            }
            catch (Exception exception)
            {
                _bigFileEntries.Clear();
                Error = exception.Message;
                return false;
            }

            return true;
        }

        public bool Save(string fileName)
        {
            Error = "";

            try
            {
                if (_bigFileEntries.Count <= 0)
                {
                    throw new Exception("No files loaded.");
                }

                FileStream outputFile = new FileStream(fileName, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(outputFile);
                writer.Write(_bigFileEntries.Count);

                uint fileIndexSize = 4u + (16u * (uint)_bigFileEntries.Count);
                fileIndexSize += 0x00000800;
                fileIndexSize &= 0xFFFFF800;
                uint offset = fileIndexSize;
                foreach (BigfileEntry entry in _bigFileEntries)
                {
                    writer.Write(entry.FileHash);
                    writer.Write(entry.FileLength);
                    writer.Write(offset);
                    writer.Write(entry.FileCode.code);

                    offset += entry.FileLength;

                    while ((offset & 0xFFFF800) != offset)
                    {
                        offset++;
                    }
                }

                while (((uint)writer.BaseStream.Position & 0xFFFF800) != (uint)writer.BaseStream.Position)
                {
                    writer.Write('\0');
                }

                writer.Flush();

                foreach (BigfileEntry entry in _bigFileEntries)
                {
                    FileStream inputFile = new FileStream(entry.FilePath, FileMode.Open);
                    inputFile.CopyTo(outputFile);
                    inputFile.Close();

                    while (((uint)writer.BaseStream.Position & 0xFFFF800) != (uint)writer.BaseStream.Position)
                    {
                        writer.Write('\0');
                    }

                    writer.Flush();
                }

                writer.Close();
                outputFile.Close();
            }
            catch (Exception exception)
            {
                Error = exception.Message;
                return false;
            }

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