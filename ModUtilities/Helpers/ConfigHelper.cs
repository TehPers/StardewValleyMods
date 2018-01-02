using System;
using Microsoft.Xna.Framework.Input;

namespace ModUtilities.Helpers {
    public static class ConfigHelper {

        /// <summary>Converts a <see cref="string"/> to the <see cref="T"/> value with that name</summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <param name="name">The name of the key in the enum</param>
        /// <returns>The enum value with the given name, or null if failed to parse</returns>
        /// <remarks>SMAPI automatically handles <see cref="Keys"/> in configs</remarks>
        public static T? StringToEnum<T>(string name) where T : struct => Enum.TryParse(name, out T value) ? (T?) value : null;
    }
}
