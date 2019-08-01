using System;
using StardewModdingAPI;

namespace TehPers.Core.Api
{
    public abstract class EventManager<T> : IDisposable
    {
        protected IModHelper Helper { get; }

        private bool _isDisposed;

        protected EventManager(IModHelper helper)
        {
            this.Helper = helper;
        }

        ~EventManager()
        {
            this.Dispose(false);
        }

        protected abstract void RegisterEventHandler();

        protected abstract void UnregisterEvent();

        protected abstract void HandleEvent(object sender, T args);

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            if (this._isDisposed)
            {
                return;
            }

            this._isDisposed = true;
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
