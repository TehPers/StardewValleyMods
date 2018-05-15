using System;
using Microsoft.Xna.Framework.Input;

namespace TehPers.Core.Helpers.EventHandlers {
    public class EventArgsKeyRepeated : EventArgs {
        public Keys RepeatedKey { get; }
        public char? Character { get; }

        public EventArgsKeyRepeated(Keys repeatedKey) {
            this.RepeatedKey = repeatedKey;
            this.Character = repeatedKey.ToChar();
        }
    }
}