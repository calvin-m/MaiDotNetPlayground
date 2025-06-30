using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MaiDotNetPlayground.ProofOfConcept.FtpClients
{
    public class SshNetFtpClient : IDisposable
    {
        private readonly SftpClient _sftpClient;
        private const string FILE_TYPE_PREFIX = " - ";
        private const string FOLDER_TYPE_PREFIX = " * ";

        private const string FILE_PATH = "C:\temp\test.pdf";
        public SshNetFtpClient(IConfiguration configuration) 
        {
            var host = configuration.GetSection("Sftp:Host").Value;
            var username = configuration.GetSection("Sftp:Username").Value;
            var password = configuration.GetSection("Sftp:Password").Value;
            _sftpClient = new SftpClient(host ?? "", username ?? "", password ?? "");
            _sftpClient.Connect();
        }

        public void Dispose()
        {
            _sftpClient?.Disconnect();
            _sftpClient?.Dispose();
        }

        public async Task ListWorkingDirectory()
        {
            var remoteEntities = _sftpClient.ListDirectoryAsync(_sftpClient.WorkingDirectory, CancellationToken.None);
            // IAsyncEnumerable is a stream and cannot be awaited. If you intend to iterate over an IAsyncEnumerable, then put the await statement before the loop statement. i.e. await foreach
            // CS1061: 'IAsyncEnumerable<ISftpFile>' does not contains a definition for 'GetAwaiter' and no accessible extension method 'GetAwaiter' accepting a first argument of type 'IAsyncEnumerable<ISftpFile>'

            StringBuilder sbResult = new StringBuilder();
            await foreach (var entry in remoteEntities)
            {
                if (entry.IsDirectory)
                {
                    sbResult.Append(FOLDER_TYPE_PREFIX).AppendLine(entry.FullName);
                }
                else 
                {
                    sbResult.Append(FILE_TYPE_PREFIX).AppendLine(entry.FullName);
                }
            }
            Console.WriteLine(sbResult.ToString());
        }
        public async Task ListAllFiles()
        {
            List<SftpFile> remoteFiles = await GetFilesOnlyRecursively(_sftpClient, _sftpClient.WorkingDirectory, new List<SftpFile>());
            var fileCount = remoteFiles.Count;

            Console.WriteLine($"There are {fileCount} files on remote server");
            foreach (var f in remoteFiles)
            {
                Console.WriteLine($"{FILE_TYPE_PREFIX}{f.Name}");
            }
        }

        private async Task<List<SftpFile>> GetFilesOnlyRecursively(SftpClient sftpClient, string directory, List<SftpFile> files)
        {
            foreach (SftpFile sftpFile in sftpClient.ListDirectory(directory))
            {
                if (sftpFile.Name.StartsWith('.')) { continue; }

                if (sftpFile.IsDirectory)
                {
                    await GetFilesOnlyRecursively(sftpClient, sftpFile.FullName, files);
                }
                else
                {
                    files.Add(sftpFile);
                }
            }
            return files;
        }
        private decimal BytesToGB(long bytes)
        {
            return Convert.ToDecimal(bytes) / 1024 / 1024 / 1024;
        }


        private string GetContentTypeFromPath(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            switch (extension)
            {
                case ".pdf": return "application/pdf";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".txt": return "text/plain";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                // Add more content types as needed
                default: return "application/octet-stream"; // Default for unknown types
            }
        }
    }
}
