using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace FoundaryMediaPlayer.Application
{
    public class FFileWriter : IFileWriter
    {
        /// <inheritdoc />
        public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <inheritdoc />
        public void WriteFile(string data, string path, Encoding encoding = null)
        {
            WriteFile((encoding ?? DefaultEncoding).GetBytes(data), path);
        }
        
        /// <inheritdoc />
        public void WriteFile(string data, string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            WriteFile(data, Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }
        
        /// <inheritdoc />
        public void WriteFile(byte[] data, string path, Encoding encoding = null)
        {
            path.Should().NotBeNullOrWhiteSpace();

            var fileInfo = new FileInfo(path);
            if (!fileInfo.Directory?.Exists ?? false)
            {
                fileInfo.Directory.Create();
            }

            using (var fileStream = fileInfo.OpenWrite())
            {
                fileStream.Write(data, 0, data.Length);
            }
        }
        
        /// <inheritdoc />
        public void WriteFile(byte[] data, string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            WriteFile(data, Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }
        
        /// <inheritdoc />
        public bool TryWriteFile(string data, string path, Encoding encoding = null)
        {
            return TryWriteFile((encoding ?? DefaultEncoding).GetBytes(data), path);
        }
        
        /// <inheritdoc />
        public bool TryWriteFile(string data, string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            return TryWriteFile(data, Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }
        
        /// <inheritdoc />
        public bool TryWriteFile(byte[] data, string path, Encoding encoding = null)
        {
            try
            {
                WriteFile(data, path, encoding);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <inheritdoc />
        public bool TryWriteFile(byte[] data, string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            return TryWriteFile(data, Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }
        
        /// <inheritdoc />
        public Task WriteFileAsync(string data, string path, Encoding encoding = null)
        {
            return WriteFileAsync((encoding ?? DefaultEncoding).GetBytes(data), path);
        }
        
        /// <inheritdoc />
        public Task WriteFileAsync(string data, string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            return WriteFileAsync(data, Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }
        
        /// <inheritdoc />
        public async Task WriteFileAsync(byte[] data, string path, Encoding encoding = null)
        {
            path.Should().NotBeNullOrWhiteSpace();

            var fileInfo = new FileInfo(path);
            if (!fileInfo.Directory?.Exists ?? false)
            {
                fileInfo.Directory.Create();
            }

            using (var fileStream = fileInfo.OpenWrite())
            {
                await fileStream.WriteAsync(data, 0, data.Length);
            }
        }
        
        /// <inheritdoc />
        public Task WriteFileAsync(byte[] data, string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            return WriteFileAsync(data, Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }
    }
}
