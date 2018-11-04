using System.Collections.Generic;

namespace TehPers.CoreMod.Conflux.Collections {
    public interface ISliceableList<T> : IList<T>, ISliceable<T> { }
}