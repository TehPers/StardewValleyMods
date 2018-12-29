using System;

namespace TehPers.CoreMod.Api.Conflux.Responsive {
    internal class SelectResponsiveOperator<TSource, TResult> : ResponsiveValue<TResult> {
        public SelectResponsiveOperator(ResponsiveValue<TSource> source, Func<TSource, TResult> selector) : base(selector(source.Value)) {
            source.ValueChanged += newValue => this.Value = selector(newValue);
        }
    }
}