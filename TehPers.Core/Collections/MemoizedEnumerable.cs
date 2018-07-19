using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Core.Collections {
    public class MemoizedEnumerable<T> : IEnumerable<T>, IDisposable {
        private readonly IEnumerator<T> _sourceEnumerator;
        private int _sourceIndex;
        private readonly List<T> _cache = new List<T>();

        public MemoizedEnumerable(IEnumerable<T> source) {
            this._sourceEnumerator = source.GetEnumerator();
            this._sourceIndex = 0;
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < this._sourceIndex; i++) {
                yield return this._cache[i];
            }

            while (this._sourceEnumerator.MoveNext()) {
                T cur = this._sourceEnumerator.Current;
                this._sourceIndex++;
                this._cache.Add(cur);
                yield return cur;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Dispose() {
            this._sourceEnumerator?.Dispose();
        }
    }
}
