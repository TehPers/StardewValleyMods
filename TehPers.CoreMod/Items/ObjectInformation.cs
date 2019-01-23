using System.Collections.Generic;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Items {
    internal class ObjectInformation : IObjectInformation {
        /// <inheritdoc />
        public int? Index { get; private set; }

        /// <inheritdoc />
        public IModObject Manager { get; }

        /// <inheritdoc />
        public ItemKey Key { get; }

        public ObjectInformation(in ItemKey key, IModObject manager, int? index = null) {
            this.Key = key;
            this.Manager = manager;
            this.Index = index;
        }

        public void SetIndex(int index, IDictionary<int, ObjectInformation> indexDict) {
            if (this.Index is int curIndex) {
                indexDict.Remove(curIndex);
            }

            indexDict.Add(index, this);
            this.Index = index;
        }

        public void RemoveIndex(IDictionary<int, ObjectInformation> indexDict) {
            if (this.Index is int curIndex) {
                indexDict.Remove(curIndex);
            }
        }
    }
}