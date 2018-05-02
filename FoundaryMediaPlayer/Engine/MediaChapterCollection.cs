using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Foundary.Extensions;
using FoundaryMediaPlayer.Interfaces;

using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Engine
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FMediaChapterCollection : IDSMChapterBag
    {
        /// <summary>
        /// Whether the <see cref="FMediaChapterCollection"/> is sorted.
        /// </summary>
        protected bool bSorted { get; set; }

        /// <summary>
        /// A collection of chapters.
        /// </summary>
        protected List<FMediaChapter> Chapters { get; } = new List<FMediaChapter>();

        /// <inheritdoc />
        public uint ChapGetCount()
        {
            return (uint)Chapters.Count;
        }

        /// <inheritdoc />
        public int ChapGet(ulong iIndex, out long prt, out string ppName)
        {
            int index = (int)iIndex;
            if (!Chapters.IsValidIndex(index))
            {
                prt = 0;
                ppName = null;
                return unchecked((int) HResult.E_INVALIDARG);
            }

            var chapter = Chapters[index];
            prt = chapter.Time;
            ppName = chapter.Name;

            return (int) HResult.S_OK;
        }

        /// <inheritdoc />
        public int ChapSet(ulong iIndex, long rt, string pName)
        {
            int index = (int)iIndex;
            if (!Chapters.IsValidIndex(index))
            {
                return unchecked((int) HResult.E_INVALIDARG);
            }

            var chapter = Chapters[index];
            chapter.Time = rt;
            chapter.Name = pName;

            bSorted = false;

            return (int) HResult.S_OK;
        }

        /// <inheritdoc />
        public int ChapAppend(long rt, string pName)
        {
            Chapters.Add(new FMediaChapter(pName, rt));

            return (int) HResult.S_OK;
        }

        /// <inheritdoc />
        public int ChapRemoveAt(ulong iIndex)
        {
            int index = (int)iIndex;
            if (!Chapters.IsValidIndex(index))
            {
                return unchecked((int) HResult.E_INVALIDARG);
            }

            Chapters.RemoveAt(index);

            return (int) HResult.S_OK;
        }

        /// <inheritdoc />
        public int ChapRemoveAll()
        {
            Chapters.Clear();

            bSorted = false;

            return (int) HResult.S_OK;
        }

        /// <inheritdoc />
        public long ChapLookup(ref long prt, out string ppName)
        {
            var time = prt;
            var chapterIndex = Chapters.FindIndex(c => c.Time == time);

            ppName = chapterIndex >= 0 ? Chapters[chapterIndex].Name : null;
            
            return chapterIndex;
        }

        public long ChapLookup(ref string ppName, out long prt)
        {
            var name = ppName;
            var chapterIndex = Chapters.FindIndex(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            prt = chapterIndex >= 0 ? Chapters[chapterIndex].Time : 0;
            
            return chapterIndex;
        }

        /// <inheritdoc />
        public int ChapSort()
        {
            if (bSorted)
            {
                return (int) HResult.S_FALSE;
            }

            Chapters.Sort();

            bSorted = true;

            return (int) HResult.S_OK;
        }
    }
}
