using System;
using System.Collections.Generic;
using System.Linq;

namespace iWaterDataCollector.INI
{
    public class IniSection : IEnumerable<KeyValuePair<string, IniValue>>, IDictionary<string, IniValue>
    {
        private Dictionary<string, IniValue> values;

        #region Ordered
        private List<string> orderedKeys;

        public int IndexOf(string key)
        {
            return !Ordered
                ? throw new InvalidOperationException("Cannot call IndexOf(string) on IniSection: section was not ordered.")
                : IndexOf(key, 0, orderedKeys.Count);
        }

        public int IndexOf(string key, int index)
        {
            return !Ordered
                ? throw new InvalidOperationException("Cannot call IndexOf(string, int) on IniSection: section was not ordered.")
                : IndexOf(key, index, orderedKeys.Count - index);
        }

        public int IndexOf(string key, int index, int count)
        {
            if (Ordered)
            {
                if (index < 0 || index > orderedKeys.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index");
                }
                if (count < 0)
                {
                    throw new IndexOutOfRangeException("Count cannot be less than zero." + Environment.NewLine + "Parameter name: count");
                }
                if (index + count > orderedKeys.Count)
                {
                    throw new ArgumentException("Index and count were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }
                int end = index + count;
                for (int i = index; i < end; i++)
                {
                    if (Comparer.Equals(orderedKeys[i], key))
                    {
                        return i;
                    }
                }
                return -1;
            }
            throw new InvalidOperationException("Cannot call IndexOf(string, int, int) on IniSection: section was not ordered.");
        }

        public int LastIndexOf(string key)
        {
            return !Ordered
                ? throw new InvalidOperationException("Cannot call LastIndexOf(string) on IniSection: section was not ordered.")
                : LastIndexOf(key, 0, orderedKeys.Count);
        }

        public int LastIndexOf(string key, int index)
        {
            return !Ordered
                ? throw new InvalidOperationException("Cannot call LastIndexOf(string, int) on IniSection: section was not ordered.")
                : LastIndexOf(key, index, orderedKeys.Count - index);
        }

        public int LastIndexOf(string key, int index, int count)
        {
            if (Ordered)
            {
                if (index < 0 || index > orderedKeys.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index");
                }
                if (count < 0)
                {
                    throw new IndexOutOfRangeException("Count cannot be less than zero." + Environment.NewLine + "Parameter name: count");
                }
                if (index + count > orderedKeys.Count)
                {
                    throw new ArgumentException("Index and count were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }
                int end = index + count;
                for (int i = end - 1; i >= index; i--)
                {
                    if (Comparer.Equals(orderedKeys[i], key))
                    {
                        return i;
                    }
                }
                return -1;
            }
            throw new InvalidOperationException("Cannot call LastIndexOf(string, int, int) on IniSection: section was not ordered.");
        }

        public void Insert(int index, string key, IniValue value)
        {
            if (Ordered)
            {
                if (index < 0 || index > orderedKeys.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index");
                }
                values.Add(key, value);
                orderedKeys.Insert(index, key);
            }
            else
            {
                throw new InvalidOperationException("Cannot call Insert(int, string, IniValue) on IniSection: section was not ordered.");
            }
        }

        public void InsertRange(int index, IEnumerable<KeyValuePair<string, IniValue>> collection)
        {
            if (Ordered)
            {
                if (collection == null)
                {
                    throw new ArgumentNullException("Value cannot be null." + Environment.NewLine + "Parameter name: collection");
                }
                if (index < 0 || index > orderedKeys.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index");
                }
                foreach (KeyValuePair<string, IniValue> kvp in collection)
                {
                    Insert(index, kvp.Key, kvp.Value);
                    index++;
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot call InsertRange(int, IEnumerable<KeyValuePair<string, IniValue>>) on IniSection: section was not ordered.");
            }
        }

        public void RemoveAt(int index)
        {
            if (Ordered)
            {
                if (index < 0 || index > orderedKeys.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index");
                }
                string key = orderedKeys[index];
                orderedKeys.RemoveAt(index);
                _ = values.Remove(key);
            }
            else
            {
                throw new InvalidOperationException("Cannot call RemoveAt(int) on IniSection: section was not ordered.");
            }
        }

        public void RemoveRange(int index, int count)
        {
            if (Ordered)
            {
                if (index < 0 || index > orderedKeys.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index");
                }
                if (count < 0)
                {
                    throw new IndexOutOfRangeException("Count cannot be less than zero." + Environment.NewLine + "Parameter name: count");
                }
                if (index + count > orderedKeys.Count)
                {
                    throw new ArgumentException("Index and count were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }
                for (int i = 0; i < count; i++)
                {
                    RemoveAt(index);
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot call RemoveRange(int, int) on IniSection: section was not ordered.");
            }
        }

        public void Reverse()
        {
            if (Ordered)
            {
                orderedKeys.Reverse();
            }
            else
            {
                throw new InvalidOperationException("Cannot call Reverse() on IniSection: section was not ordered.");
            }
        }

        public void Reverse(int index, int count)
        {
            if (Ordered)
            {
                if (index < 0 || index > orderedKeys.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index");
                }
                if (count < 0)
                {
                    throw new IndexOutOfRangeException("Count cannot be less than zero." + Environment.NewLine + "Parameter name: count");
                }
                if (index + count > orderedKeys.Count)
                {
                    throw new ArgumentException("Index and count were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }
                orderedKeys.Reverse(index, count);
            }
            else
            {
                throw new InvalidOperationException("Cannot call Reverse(int, int) on IniSection: section was not ordered.");
            }
        }

        public ICollection<IniValue> GetOrderedValues()
        {
            if (Ordered)
            {
                List<IniValue> list = new List<IniValue>();
                for (int i = 0; i < orderedKeys.Count; i++)
                {
                    list.Add(values[orderedKeys[i]]);
                }
                return list;
            }
            throw new InvalidOperationException("Cannot call GetOrderedValues() on IniSection: section was not ordered.");
        }

        public IniValue this[int index]
        {
            get
            {
                if (Ordered)
                {
                    return index < 0 || index >= orderedKeys.Count
                        ? throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index")
                        : values[orderedKeys[index]];
                }
                throw new InvalidOperationException("Cannot index IniSection using integer key: section was not ordered.");
            }
            set
            {
                if (Ordered)
                {
                    if (index < 0 || index >= orderedKeys.Count)
                    {
                        throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine + "Parameter name: index");
                    }
                    string key = orderedKeys[index];
                    values[key] = value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot index IniSection using integer key: section was not ordered.");
                }
            }
        }

        public bool Ordered
        {
            get => orderedKeys != null;
            set
            {
                if (Ordered != value)
                {
                    orderedKeys = value ? new List<string>(values.Keys) : null;
                }
            }
        }
        #endregion

        public IniSection()
            : this(IniFile.DefaultComparer)
        {
        }

        public IniSection(IEqualityComparer<string> stringComparer)
        {
            this.values = new Dictionary<string, IniValue>(stringComparer);
        }

        public IniSection(Dictionary<string, IniValue> values)
            : this(values, IniFile.DefaultComparer)
        {
        }

        public IniSection(Dictionary<string, IniValue> values, IEqualityComparer<string> stringComparer)
        {
            this.values = new Dictionary<string, IniValue>(values, stringComparer);
        }

        public IniSection(IniSection values)
            : this(values, IniFile.DefaultComparer)
        {
        }

        public IniSection(IniSection values, IEqualityComparer<string> stringComparer)
        {
            this.values = new Dictionary<string, IniValue>(values.values, stringComparer);
        }

        public void Add(string key, IniValue value)
        {
            values.Add(key, value);
            if (Ordered)
            {
                orderedKeys.Add(key);
            }
        }

        public bool ContainsKey(string key)
        {
            return values.ContainsKey(key);
        }

        /// <summary>
        /// Returns this IniSection's collection of keys. If the IniSection is ordered, the keys will be returned in order.
        /// </summary>
        public ICollection<string> Keys => Ordered ? (ICollection<string>)orderedKeys : values.Keys;

        public bool Remove(string key)
        {
            bool ret = values.Remove(key);
            if (Ordered && ret)
            {
                for (int i = 0; i < orderedKeys.Count; i++)
                {
                    if (Comparer.Equals(orderedKeys[i], key))
                    {
                        orderedKeys.RemoveAt(i);
                        break;
                    }
                }
            }
            return ret;
        }

        public bool TryGetValue(string key, out IniValue value) => values.TryGetValue(key, out value);

        /// <summary>
        /// Returns the values in this IniSection. These values are always out of order. To get ordered values from an IniSection call GetOrderedValues instead.
        /// </summary>
        public ICollection<IniValue> Values => values.Values;

        void ICollection<KeyValuePair<string, IniValue>>.Add(KeyValuePair<string, IniValue> item)
        {
            ((IDictionary<string, IniValue>)values).Add(item);
            if (Ordered)
            {
                orderedKeys.Add(item.Key);
            }
        }

        public void Clear()
        {
            values.Clear();
            if (Ordered)
            {
                orderedKeys.Clear();
            }
        }

        bool ICollection<KeyValuePair<string, IniValue>>.Contains(KeyValuePair<string, IniValue> item)
        {
            return ((IDictionary<string, IniValue>)values).Contains(item);
        }

        void ICollection<KeyValuePair<string, IniValue>>.CopyTo(KeyValuePair<string, IniValue>[] array, int arrayIndex)
        {
            ((IDictionary<string, IniValue>)values).CopyTo(array, arrayIndex);
        }

        public int Count => values.Count;

        bool ICollection<KeyValuePair<string, IniValue>>.IsReadOnly => ((IDictionary<string, IniValue>)values).IsReadOnly;

        bool ICollection<KeyValuePair<string, IniValue>>.Remove(KeyValuePair<string, IniValue> item)
        {
            bool ret = ((IDictionary<string, IniValue>)values).Remove(item);
            if (Ordered && ret)
            {
                for (int i = 0; i < orderedKeys.Count; i++)
                {
                    if (Comparer.Equals(orderedKeys[i], item.Key))
                    {
                        orderedKeys.RemoveAt(i);
                        break;
                    }
                }
            }
            return ret;
        }

        public IEnumerator<KeyValuePair<string, IniValue>> GetEnumerator() => Ordered ? GetOrderedEnumerator() : values.GetEnumerator();

        private IEnumerator<KeyValuePair<string, IniValue>> GetOrderedEnumerator()
        {
            for (int i = 0; i < orderedKeys.Count; i++)
            {
                yield return new KeyValuePair<string, IniValue>(orderedKeys[i], values[orderedKeys[i]]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEqualityComparer<string> Comparer { get { return values.Comparer; } }

        public IniValue this[string name]
        {
            get => values.TryGetValue(name, out IniValue val) ? val : IniValue.Default;
            set
            {
                if (Ordered && !orderedKeys.Contains(name, Comparer))
                {
                    orderedKeys.Add(name);
                }
                values[name] = value;
            }
        }

        public static implicit operator IniSection(Dictionary<string, IniValue> dict)
        {
            return new IniSection(dict);
        }

        public static explicit operator Dictionary<string, IniValue>(IniSection section)
        {
            return section.values;
        }
    }
}
