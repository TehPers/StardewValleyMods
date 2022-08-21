using TehPers.Core.Gui.Api;
using TehPers.Core.Gui.Api.Components;
using TehPers.Core.Gui.Api.Internal;

namespace TehPers.Core.Gui;

public class InternalApi : IInternalApi
{
    private readonly IGuiBuilder guiBuilder;

    public InternalApi(IGuiBuilder guiBuilder)
    {
        this.guiBuilder = guiBuilder;
    }

    public IDefaultComponentProvider CreateDefaultComponentProvider()
    {
        return new DefaultComponentProvider(this.guiBuilder);
    }
}
