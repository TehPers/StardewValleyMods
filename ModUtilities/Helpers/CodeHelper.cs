using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Input;

namespace ModUtilities.Helpers {
    public static class CodeHelper {
        public static readonly UnicodeCategory[] NonRenderingCategories = { UnicodeCategory.Control, UnicodeCategory.OtherNotAssigned, UnicodeCategory.Surrogate };

        /// <summary>Chaining function for performing multiple tasks in a single line of code</summary>
        /// <typeparam name="T">The type of object being chained</typeparam>
        /// <param name="obj">The object being chained</param>
        /// <param name="action">The action to perform on <see cref="obj"/></param>
        /// <returns><see cref="obj"/></returns>
        public static T Chain<T>(this T obj, Action<T> action) {
            action(obj);
            return obj;
        }

        /// <summary>Tries to load an assembly conditionally</summary>
        /// <param name="condition">Whether to try to load the assembly</param>
        /// <param name="assemblyString">The long name of an assembly</param>
        /// <returns>The assembly if it loaded, or null if failed</returns>
        public static Assembly OptionallyLoadAssembly(bool condition, string assemblyString) {
            if (!condition)
                return null;

            try {
                return Assembly.Load(assemblyString);
            } catch {
                return null;
            }
        }

        public static bool IsPrintable(this Keys key) => key.ToChar()?.IsPrintable() ?? false;

        public static bool IsPrintable(this char character) {
            return char.IsWhiteSpace(character) || !CodeHelper.NonRenderingCategories.Contains(char.GetUnicodeCategory(character));
        }

        public static char? ToChar(this Keys key) => key.ToChar(false, false);
        public static char? ToChar(this Keys key, bool shift, bool caps) {
            switch (key) {
                /* OEM */
                case Keys.OemSemicolon:
                    return ';';
                case Keys.OemBackslash:
                    return '\\';
                case Keys.OemQuestion:
                    return '?';
                case Keys.OemTilde:
                    return '`';
                case Keys.OemOpenBrackets:
                    return shift ? '{' : '[';
                case Keys.OemPipe:
                    return shift ? '|' : '\\';
                case Keys.OemCloseBrackets:
                    return shift ? '}' : ']';
                case Keys.OemQuotes:
                    return shift ? '"' : '\'';
                case Keys.OemComma:
                    return shift ? '<' : ',';
                case Keys.OemMinus:
                    return shift ? '_' : '-';
                case Keys.OemPeriod:
                    return shift ? '>' : '.';
                case Keys.OemPlus:
                    return shift ? '+' : '=';

                /* Alphabet */
                case Keys.A:
                    return shift ^ caps ? 'A' : 'a';
                case Keys.B:
                    return shift ^ caps ? 'B' : 'b';
                case Keys.C:
                    return shift ^ caps ? 'C' : 'c';
                case Keys.D:
                    return shift ^ caps ? 'D' : 'd';
                case Keys.E:
                    return shift ^ caps ? 'E' : 'e';
                case Keys.F:
                    return shift ^ caps ? 'F' : 'f';
                case Keys.G:
                    return shift ^ caps ? 'G' : 'g';
                case Keys.H:
                    return shift ^ caps ? 'H' : 'h';
                case Keys.I:
                    return shift ^ caps ? 'I' : 'i';
                case Keys.J:
                    return shift ^ caps ? 'J' : 'j';
                case Keys.K:
                    return shift ^ caps ? 'K' : 'k';
                case Keys.L:
                    return shift ^ caps ? 'L' : 'l';
                case Keys.M:
                    return shift ^ caps ? 'M' : 'm';
                case Keys.N:
                    return shift ^ caps ? 'N' : 'n';
                case Keys.O:
                    return shift ^ caps ? 'O' : 'o';
                case Keys.P:
                    return shift ^ caps ? 'P' : 'p';
                case Keys.Q:
                    return shift ^ caps ? 'Q' : 'q';
                case Keys.R:
                    return shift ^ caps ? 'R' : 'r';
                case Keys.S:
                    return shift ^ caps ? 'S' : 's';
                case Keys.T:
                    return shift ^ caps ? 'T' : 't';
                case Keys.U:
                    return shift ^ caps ? 'U' : 'u';
                case Keys.V:
                    return shift ^ caps ? 'V' : 'v';
                case Keys.W:
                    return shift ^ caps ? 'W' : 'w';
                case Keys.X:
                    return shift ^ caps ? 'X' : 'x';
                case Keys.Y:
                    return shift ^ caps ? 'Y' : 'y';
                case Keys.Z:
                    return shift ^ caps ? 'Z' : 'z';

                /* Number Row */
                case Keys.D0:
                    return shift ? ')' : '0';
                case Keys.D1:
                    return shift ? '!' : '1';
                case Keys.D2:
                    return shift ? '@' : '2';
                case Keys.D3:
                    return shift ? '#' : '3';
                case Keys.D4:
                    return shift ? '$' : '4';
                case Keys.D5:
                    return shift ? '%' : '5';
                case Keys.D6:
                    return shift ? '^' : '6';
                case Keys.D7:
                    return shift ? '&' : '7';
                case Keys.D8:
                    return shift ? '*' : '8';
                case Keys.D9:
                    return shift ? '(' : '9';

                /* Numpad */
                case Keys.NumPad0:
                    return shift ? (char?) null : '0';
                case Keys.NumPad1:
                    return shift ? (char?) null : '1';
                case Keys.NumPad2:
                    return shift ? (char?) null : '2';
                case Keys.NumPad3:
                    return shift ? (char?) null : '3';
                case Keys.NumPad4:
                    return shift ? (char?) null : '4';
                case Keys.NumPad5:
                    return shift ? (char?) null : '5';
                case Keys.NumPad6:
                    return shift ? (char?) null : '6';
                case Keys.NumPad7:
                    return shift ? (char?) null : '7';
                case Keys.NumPad8:
                    return shift ? (char?) null : '8';
                case Keys.NumPad9:
                    return shift ? (char?) null : '9';

                /* Operators */
                case Keys.Add:
                    return '+';
                case Keys.Subtract:
                    return '-';
                case Keys.Multiply:
                    return '*';
                case Keys.Divide:
                    return '/';
                case Keys.Decimal:
                    return shift ? '>' : '.';

                /* Whitespace */
                case Keys.Space:
                    return ' ';
                case Keys.Tab:
                    return '\t';
                case Keys.Enter:
                    return '\n';

                default:
                    return null;
            }
        }
    }
}
