using System.Text;
using System.Threading.Tasks;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// An interface for a class that has the ability to read a file.
    /// </summary>
    public interface IFileReader
    {
        /// <summary>
        /// The encoding to use if the encoding is not specified.
        /// </summary>
        Encoding DefaultEncoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        string ReadFile(string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        /// <returns></returns>
        string ReadFile(string path, params string[] appendPathParts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        bool TryReadFile(out string data, string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        /// <returns></returns>
        bool TryReadFile(out string data, string path, params string[] appendPathParts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task<string> ReadFileAsync(string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        /// <returns></returns>
        Task<string> ReadFileAsync(string path, params string[] appendPathParts);
    }
}
