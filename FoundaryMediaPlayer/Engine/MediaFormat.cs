using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoundaryMediaPlayer.Application;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// A media format.
    /// </summary>
    public class FMediaFormat : IEquatable<FMediaFormat>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool bAudioOnly { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool bAssociable { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 
        /// </summary>
        public EEngineType EngineType { get; }

        /// <summary>
        /// 
        /// </summary>
        protected IEnumerable<string> Extensions { get; }


        /// <summary>
        /// 
        /// </summary>
        public FMediaFormat()
            : this(null, null, (IEnumerable<string>)null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="description"></param>
        /// <param name="extensions"></param>
        /// <param name="bAudioOnly"></param>
        /// <param name="engineType"></param>
        /// <param name="bAssociable"></param>
        public FMediaFormat(string label, string description, string extensions, bool bAudioOnly = false, EEngineType engineType = EEngineType.Custom, bool bAssociable = true)
            : this(label, description, extensions?.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries), bAudioOnly, engineType, bAssociable)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="description"></param>
        /// <param name="extensions"></param>
        /// <param name="bAudioOnly"></param>
        /// <param name="engineType"></param>
        /// <param name="bAssociable"></param>
        public FMediaFormat(string label, string description, IEnumerable<string> extensions, bool bAudioOnly = false, EEngineType engineType = EEngineType.Custom, bool bAssociable = true)
        {
            Label = label;
            Description = description;
            Extensions = extensions != null ? new List<string>(extensions.Where(ext => !string.IsNullOrWhiteSpace(ext)).Select(ext => ext.ToLowerInvariant().Trim('.'))) : new List<string>();
            this.bAudioOnly = bAudioOnly;
            this.bAssociable = bAssociable;
            EngineType = engineType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public FMediaFormat(FMediaFormat other)
        {
            Label = other.Label;
            Description = other.Description;
            Extensions = other.Extensions;
            bAudioOnly = other.bAudioOnly;
            bAssociable = other.bAssociable;
            EngineType = other.EngineType;
        }

        /// <inheritdoc />
        public bool Equals(FMediaFormat other)
        {
            if (other == null)
            {
                return false;
            }

            return
                Label == other.Label &&
                Description == other.Description &&
                Extensions.Equals(other.Extensions) &&
                bAudioOnly == other.bAudioOnly &&
                bAssociable == other.bAssociable &&
                EngineType == other.EngineType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool HasExtension(string extension)
        {
            return Extensions.Contains(extension.Trim('.').ToLowerInvariant());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bWithDot"></param>
        /// <returns></returns>
        public IEnumerable<string> GetExtensions(bool bWithDot = false)
        {
            return Extensions.Select(extension => bWithDot ? $".{extension}" : extension);
        }

        /// <summary>
        /// Returns the file extension filter for this <see cref="FMediaFormat"/>.
        /// </summary>
        /// <returns>The file extension filter for this <see cref="FMediaFormat"/>.</returns>
        /// <remarks>Useful for the open file dialog.</remarks>
        public string GetFilter()
        {
            var filter = new StringBuilder();
            var count = Extensions.Count();

            for (int i = 0; i < count; ++i)
            {
                var extension = Extensions.ElementAt(i);
                filter.Append(i != count - 1 ? $"*.{extension};" : $"*.{extension}");
            }

            return filter.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public void Update(FApplicationStore store)
        {
            // Read from the save for any adjustments to this format category when supported.
        }
    }
}
