using System.Text;
using System.Threading.Tasks;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// An interface for a class that has the ability to write to a file.
    /// </summary>
    public interface IFileWriter
    {
        /// <summary>
        /// The encoding to use if the encoding is not specified.
        /// </summary>
        Encoding DefaultEncoding { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        void WriteFile(string data, string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        void WriteFile(string data, string path, params string[] appendPathParts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        void WriteFile(byte[] data, string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        void WriteFile(byte[] data, string path, params string[] appendPathParts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        bool TryWriteFile(string data, string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        /// <returns></returns>
        bool TryWriteFile(string data, string path, params string[] appendPathParts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        bool TryWriteFile(byte[] data, string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        /// <returns></returns>
        bool TryWriteFile(byte[] data, string path, params string[] appendPathParts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task WriteFileAsync(string data, string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        /// <returns></returns>
        Task WriteFileAsync(string data, string path, params string[] appendPathParts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        Task WriteFileAsync(byte[] data, string path, Encoding encoding = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <param name="appendPathParts"></param>
        /// <returns></returns>
        Task WriteFileAsync(byte[] data, string path, params string[] appendPathParts);
    }
}
