﻿using CRMCore.Module.CustomCollection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRMCore.Module.CustomCollection.Entity.Content
{
    public abstract class ContentData<T> : Dictionary<T, ContentFieldData>, IEquatable<ContentData<T>>
    {
        public IEnumerable<KeyValuePair<T, ContentFieldData>> ValidValues
        {
            get { return this.Where(x => x.Value != null); }
        }

        protected ContentData(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        protected ContentData(IDictionary<T, ContentFieldData> copy, IEqualityComparer<T> comparer)
            : base(copy, comparer)
        {
        }

        protected static TResult Merge<TResult>(TResult source, TResult target) where TResult : ContentData<T>
        {
            if (ReferenceEquals(target, source))
            {
                return source;
            }

            foreach (var otherValue in source)
            {
                var fieldValue = target.GetOrAdd(otherValue.Key, x => new ContentFieldData());

                foreach (var value in otherValue.Value)
                {
                    fieldValue[value.Key] = value.Value;
                }
            }

            return target;
        }

        protected static TResult Clean<TResult>(TResult source, TResult target) where TResult : ContentData<T>
        {
            foreach (var fieldValue in source.ValidValues)
            {
                var resultValue = new ContentFieldData();

                foreach (var partitionValue in fieldValue.Value.Where(x => !x.Value.IsNull()))
                {
                    resultValue[partitionValue.Key] = partitionValue.Value;
                }

                if (resultValue.Count > 0)
                {
                    target[fieldValue.Key] = resultValue;
                }
            }

            return target;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContentData<T>);
        }

        public bool Equals(ContentData<T> other)
        {
            return other != null && (ReferenceEquals(this, other) || this.EqualsDictionary(other));
        }

        public override int GetHashCode()
        {
            return this.DictionaryHashCode();
        }
    }
}
