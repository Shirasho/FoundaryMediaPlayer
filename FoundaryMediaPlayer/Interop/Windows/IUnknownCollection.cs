using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FluentAssertions;

namespace FoundaryMediaPlayer.Interop.Windows
{
    /// <summary>
    /// A collection of IUnknown objects.
    /// </summary>
    //TODO: Is restricting to ComObjects a bad thing? We can always loosen it later.
    public sealed class FIUnknownCollection : IList<object>
    {
        private List<object> _Collection { get; } = new List<object>();

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator() => _Collection.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public void Add(object item)
        {
            Marshal.IsComObject(item).Should().BeTrue();
            _Collection.Add(item);
        }

        /// <inheritdoc />
        public void Clear() => _Collection.Clear();

        /// <inheritdoc />
        public bool Contains(object item) => _Collection.Contains(item);

        /// <inheritdoc />
        public void CopyTo(object[] array, int arrayIndex) => _Collection.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove(object item) => _Collection.Remove(item);

        /// <inheritdoc />
        public int Count => _Collection.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public int IndexOf(object item) => _Collection.IndexOf(item);

        /// <inheritdoc />
        public void Insert(int index, object item)
        {
            Marshal.IsComObject(item).Should().BeTrue();
            _Collection.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index) => _Collection.RemoveAt(index);

        /// <inheritdoc />
        public object this[int index]
        {
            get { return _Collection[index]; }
            set
            {
                Marshal.IsComObject(value).Should().BeTrue();
                _Collection[index] = value;
            }
        }
    }
}
