using System.Collections.Generic;

namespace TehPers.CoreMod.Api.CSharpX.Collections {
    public interface ISliceableList<T> : IList<T>, ISliceable<T> { }
}