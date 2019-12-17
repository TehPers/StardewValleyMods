using System;
using System.Reflection;

namespace TehPers.Core.Api.Harmony
{
    /// <summary>Service which can apply and remove patches. It is recommended to mark classes that inherit <see cref="PatchingService{TImplementation}"/> as <see langword="sealed"/>.</summary>
    /// <typeparam name="TImplementation">The type that is inheriting <see cref="PatchingService{TImplementation}"/>. This must be equal to the implementation type.</typeparam>
    public abstract class PatchingService<TImplementation> : IPatchingService
        where TImplementation : PatchingService<TImplementation>
    {
        /// <summary>Gets the singleton instance of the service.</summary>
        protected static TImplementation Instance { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="PatchingService{TImplementation}"/> class.</summary>
        protected PatchingService()
        {
            if (typeof(TImplementation) != this.GetType())
            {
                throw new InvalidOperationException($"{nameof(TImplementation)} must be {this.GetType().FullName}.");
            }
        }

        /// <summary>Finalizes an instance of the <see cref="PatchingService{TImplementation}"/> class.</summary>
        ~PatchingService()
        {
            this.Dispose(false);
        }

        /// <inheritdoc/>
        public void ApplyPatches()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Patches are already applied!");
            }

            Instance = (TImplementation)this;
            this.ApplyPatchesInternal();
        }

        /// <inheritdoc/>
        public void RemovePatches()
        {
            if (Instance != this)
            {
                return;
            }

            this.RemovePatchesInternal();
            Instance = null;
        }

        /// <summary>Called when patches should be applied.</summary>
        protected abstract void ApplyPatchesInternal();

        /// <summary>Called when the patches applied by this class should be removed.</summary>
        protected abstract void RemovePatchesInternal();

        /// <summary>Gets a <see cref="MethodInfo"/> for a method from a <see cref="Type"/>.</summary>
        /// <param name="sourceType">The source <see cref="Type"/>.</param>
        /// <param name="methodName">The name of the method to get the <see cref="MethodInfo"/> of.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> to use when searching for the method.</param>
        /// <returns>The method's <see cref="MethodInfo"/>.</returns>
        protected MethodInfo GetMethod(Type sourceType, string methodName, BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            _ = methodName ?? throw new ArgumentNullException(nameof(methodName));
            _ = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            return sourceType.GetMethod(methodName, bindingFlags) ?? throw new InvalidOperationException($"Could not get {nameof(MethodInfo)} for {sourceType.FullName}.{methodName}");
        }

        /// <summary>Called when this <see cref="PatchingService{TImplementation}"/> is being finalized or disposed of.</summary>
        /// <param name="disposing"><see langword="true"/> if managed resources should be freed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Instance == this)
            {
                this.RemovePatches();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}