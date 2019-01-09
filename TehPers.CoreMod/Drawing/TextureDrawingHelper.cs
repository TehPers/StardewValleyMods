using System;
using System.Collections.Generic;
using TehPers.CoreMod.Api.Drawing;

namespace TehPers.CoreMod.Drawing {
    internal class TextureDrawingHelper : ITextureDrawingHelper {
        private readonly IDrawingApi _api;
        private readonly HashSet<EventHandler<IDrawingInfo>> _drawingHandlers = new HashSet<EventHandler<IDrawingInfo>>();
        private readonly HashSet<EventHandler<IReadonlyDrawingInfo>> _drawnHandlers = new HashSet<EventHandler<IReadonlyDrawingInfo>>();

        public TextureDrawingHelper(IDrawingApi api) {
            this._api = api;
        }

        public IEnumerable<EventHandler<IDrawingInfo>> GetDrawingHandlers() {
            return this._drawingHandlers;
        }

        public IEnumerable<EventHandler<IReadonlyDrawingInfo>> GetDrawnHandlers() {
            return this._drawnHandlers;
        }

        public event EventHandler<IDrawingInfo> Drawing {
            add => this._drawingHandlers.Add(value);
            remove => this._drawingHandlers.Remove(value);
        }

        public event EventHandler<IReadonlyDrawingInfo> Drawn {
            add => this._drawnHandlers.Add(value);
            remove => this._drawnHandlers.Remove(value);
        }
    }
}