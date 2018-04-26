using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Foundary.Extensions;
using FoundaryMediaPlayer.Platforms.Windows;

using HResult = PInvoke.HResult.Code;

namespace FoundaryMediaPlayer.Engine.Windows
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class DSMChapterBag : IDSMChapterBag
    {
        /// <summary>
        /// Whether the <see cref="DSMChapterBag"/> is sorted.
        /// </summary>
        protected bool bSorted { get; set; }

        /// <summary>
        /// A collection of chapters.
        /// </summary>
        protected List<DSMChapter> Chapters { get; } = new List<DSMChapter>();

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
            Chapters.Add(new DSMChapter(pName, rt));

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
            ChapSort();

            uint i = range_bsearch(prt);
            if (i != uint.MaxValue)
            {
                var chapter = Chapters[(int) i];
                prt = chapter.Time;
                ppName = chapter.Name;
            }
            else
            {
                ppName = null;
            }

            return i;
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

        private uint range_bsearch(long rt)
        {
            int k = Chapters.Count - 1;
            if ((k < 0) || (rt >= Chapters[k].Time))
            {
                return (uint)k;
            }

            uint ret = uint.MaxValue;
            if (k == 0)
            {
                return ret;
            }

            uint i = 0, j = (uint)k;
            do
            {
                uint mid = (i + j) >> 1;
                long midrt = Chapters[(int) mid].Time;
                if (rt == midrt)
                {
                    ret = mid;
                    break;
                }

                if (rt < midrt)
                {
                    ret = uint.MaxValue;
                    if (j == mid)
                    {
                        --mid;
                    }

                    j = mid;
                }
                else if (rt > midrt)
                {
                    ret = mid;
                    if (i == mid)
                    {
                        ++mid;
                    }

                    i = mid;
                }
            } while (i < j);

            return ret;
        }
    }
}
