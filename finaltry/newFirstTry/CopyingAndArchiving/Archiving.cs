using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace newFirstTry.CopyingAndArchiving
{
    public class Archiving
    {
        public static void ArchivingAndCopying(string FileEvent, string filePath,bool create,bool rename,string archivePath,string archiveDateTime,string archiveNameForGZ, string archiveGZextension,string recordingPathOfLog, string recordingDateTime,bool archived)
        {
            string path = Path.Combine(archivePath, DateTime.Now.ToString(archiveDateTime));
            Directory.CreateDirectory(path);
            string gzipPath = Path.Combine(path, archiveNameForGZ);
            Directory.CreateDirectory(gzipPath);
            StreamReader read = null;
            StreamWriter writer = null;
            FileStream gzFile = null;
            FileStream file = null;
            GZipStream gzip = null;
            StreamWriter exwriter = null;
            FileStream originalCompressed = null;
            FileStream decompressedFile = null;
            GZipStream forDecompression = null;
            try
            {
                FileInfo original = new FileInfo(filePath);
                string name = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                FileStream fs = File.Create(Path.Combine(path, name));
                fs.Close();
                using (read = original.OpenText())
                {
                    using (writer = new StreamWriter(Path.Combine(path, name), true))
                    {
                        string buffer = "";
                        while ((buffer = read.ReadLine()) != null)
                        {
                            writer.WriteLine(buffer);
                        }
                    }
                }
                name += archiveGZextension;

                using (file = new FileStream(filePath, FileMode.Open))
                {
                    if (file.Length != 0)
                    {
                        using (gzFile = new FileStream(Path.Combine(gzipPath, name), FileMode.Create, FileAccess.ReadWrite))
                        {

                            using (gzip = new GZipStream(gzFile, CompressionMode.Compress))
                            {
                                file.CopyTo(gzip);
                                archived = true;
                            }
                        }
                    }
                }
                if (FileEvent == "создан")
                {
                    create = true;
                }
                else if (FileEvent == "переименован")
                {
                    rename = true;
                }
                if (archived==true)
                {
                    string decompressedPath = Path.Combine(path, "decompressed");
                    Directory.CreateDirectory(decompressedPath);
                    FileInfo compressed = new FileInfo(Path.Combine(gzipPath, name));
                    name = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                    
                    using (originalCompressed = compressed.OpenRead())
                    {
                        using (decompressedFile = File.Create(Path.Combine(decompressedPath, name)))
                        {
                            using (forDecompression=new GZipStream(originalCompressed,CompressionMode.Decompress))
                            {
                                forDecompression.CopyTo(decompressedFile);
                            }
                        }
                    }
                    
                }

            }
            catch (Exception e)
            {
                using (exwriter = new StreamWriter(recordingPathOfLog, true))
                {
                    exwriter.WriteLine($"{DateTime.Now.ToString(recordingDateTime)} произошла ошибка {e.Message} ");


                    exwriter.Flush();
                }
                throw;
            }
            finally
            {
                read?.Close();
                read?.Dispose();
                writer?.Close();
                writer?.Dispose();
                gzFile?.Close();
                gzFile?.Dispose();
                file?.Close();
                file?.Dispose();
                gzip?.Close();
                gzip?.Dispose();
                exwriter?.Close();
                exwriter?.Dispose();
                originalCompressed?.Close();
                originalCompressed?.Dispose();
                decompressedFile?.Close();
                decompressedFile?.Dispose();
                forDecompression?.Close();
                forDecompression?.Dispose();
            }

        }
    }
}
