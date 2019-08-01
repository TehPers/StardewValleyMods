using TehPers.Core.DependencyInjection.Lifecycle;

namespace TehPers.Core.DependencyInjection
{
    internal class ModInit
    {
        private readonly LifecycleManager _lifecycleManager;

        public ModInit(LifecycleManager lifecycleManager)
        {
            this._lifecycleManager = lifecycleManager;
        }

        public void Init()
        {
            this._lifecycleManager.RegisterEvents();
        }
    }
}
