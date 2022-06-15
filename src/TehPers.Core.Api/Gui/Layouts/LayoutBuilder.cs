using System;

namespace TehPers.Core.Api.Gui.Layouts
{
    /// <summary>
    /// Extension methods for layout builders.
    /// </summary>
    public static class LayoutBuilder
    {
        /// <summary>
        /// Adds a new component to this layout.
        /// </summary>
        /// <param name="layout">The layout builder.</param>
        /// <param name="component">The component to add.</param>
        /// <typeparam name="TState">The type of the state of the component being added.</typeparam>
        /// <typeparam name="TLayout">The type of the resulting layout.</typeparam>
        public static void Add<TState, TLayout>(
            this ILayoutBuilder<WrappedComponent.State, TLayout> layout,
            IGuiComponent<TState> component
        )
        {
            layout.Add(component.Wrapped());
        }

        /// <summary>
        /// Adds a nested vertical layout.
        /// </summary>
        /// <typeparam name="TLayout">The type of the outer layout.</typeparam>
        /// <param name="builder">The outer layout builder.</param>
        /// <param name="addComponents">A callback for building the inner layout.</param>
        public static void VerticalLayout<TLayout>(
            this ILayoutBuilder<WrappedComponent.State, TLayout> builder,
            Action<VerticalLayout.Builder> addComponents
        )
        {
            var innerBuilder = new VerticalLayout.Builder();
            addComponents(innerBuilder);
            builder.Add(innerBuilder.Build().Wrapped());
        }

        public static MappedLayoutBuilder<WrappedComponent.State, WrappedComponent.State, TLayout>
            Select<TResultState, TLayout>(
                this ILayoutBuilder<WrappedComponent.State, TLayout> layout,
                Func<IGuiComponent<WrappedComponent.State>, IGuiComponent<TResultState>> map
            )
        {
            return new(
                layout,
                map switch
                {
                    Func<IGuiComponent<WrappedComponent.State>,
                        IGuiComponent<WrappedComponent.State>> m => m,
                    _ => c => map(c).Wrapped(),
                }
            );
        }

        public static MappedLayoutBuilder<TState1, TState2, TLayout>
            Select<TState1, TState2, TLayout>(
                this ILayoutBuilder<TState2, TLayout> builder,
                Func<IGuiComponent<TState1>, IGuiComponent<TState2>> map
            )
        {
            return new(builder, map);
        }
    }

    public class MappedLayoutBuilder<TState1, TState2, TLayout> : ILayoutBuilder<TState1, TLayout>
    {
        private readonly ILayoutBuilder<TState2, TLayout> inner;
        private readonly Func<IGuiComponent<TState1>, IGuiComponent<TState2>> map;

        public MappedLayoutBuilder(
            ILayoutBuilder<TState2, TLayout> inner,
            Func<IGuiComponent<TState1>, IGuiComponent<TState2>> map
        )
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.map = map ?? throw new ArgumentNullException(nameof(map));
        }


        /// <inheritdoc />
        public void Add(IGuiComponent<TState1> component)
        {
            this.inner.Add(this.map(component));
        }

        /// <inheritdoc />
        public TLayout Build()
        {
            return this.inner.Build();
        }
    }
}
