using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace FoundaryMediaPlayer.Application
{
    public class FFileReader : IFileReader
    {
        /// <inheritdoc />
        public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <exception cref="IOException"><paramref name="path"/> does not exist.</exception>
        public string ReadFile(string path, Encoding encoding = null)
        {
            path.Should().NotBeNullOrWhiteSpace();

            var fileInfo = new FileInfo(path);
            
            if (!fileInfo.Exists)
            {
                throw new IOException($"The file at path {path} does not exist.");
            }

            using (var fileStream = fileInfo.OpenRead())
            {
                var data = new byte[fileStream.Length];
                fileStream.Read(data, 0, (int)fileStream.Length);
                return (encoding ?? DefaultEncoding).GetString(data);
            }
        }

        /// <exception cref="IOException"><paramref name="path"/> does not exist.</exception>
        public string ReadFile(string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            return ReadFile(Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }

        /// <inheritdoc />
        public bool TryReadFile(out string data, string path, Encoding encoding = null)
        {
            try
            {
                data = ReadFile(path, encoding);
                return true;
            }
            catch
            {
                data = null;
                return false;
            }
        }

        /// <inheritdoc />
        public bool TryReadFile(out string data, string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            return TryReadFile(out data, Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }

        /// <exception cref="IOException"><paramref name="path"/> does not exist.</exception>
        public async Task<string> ReadFileAsync(string path, Encoding encoding = null)
        {
            path.Should().NotBeNullOrWhiteSpace();

            var fileInfo = new FileInfo(path);
            
            if (!fileInfo.Exists)
            {
                throw new IOException($"The file at path {path} does not exist.");
            }

            using (var fileStream = fileInfo.OpenRead())
            {
                var data = new byte[fileStream.Length];
                await fileStream.ReadAsync(data, 0, (int)fileStream.Length);
                return (encoding ?? DefaultEncoding).GetString(data);
            }
        }

        /// <exception cref="IOException"><paramref name="path"/> does not exist.</exception>
        public Task<string> ReadFileAsync(string path, params string[] appendPathParts)
        {
            path.Should().NotBeNullOrWhiteSpace();

            return ReadFileAsync(Path.Combine(Utilities.CombineStrings(path, appendPathParts)));
        }
    }
}
