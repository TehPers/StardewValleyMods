namespace TehPers.Core.Api.DI
{
    public static class ModServices
    {
        /// <summary>
        /// Gets the <see cref="IModKernelFactory"/> for creating mod kernels. This value is
        /// guaranteed to be <see langword="null"/> if <c>TehPers.Core</c> has not been loaded yet.
        /// To ensure that your mod loads after the core mod, add <c>TehPers.Core</c> as a
        /// dependency in your mod's manifest. If you do not need the core mod to be loaded for
        /// your mod to function, then you may add it as an optional dependency instead.
        /// </summary>
        public static IModKernelFactory? Factory { get; internal set; }
    }
}