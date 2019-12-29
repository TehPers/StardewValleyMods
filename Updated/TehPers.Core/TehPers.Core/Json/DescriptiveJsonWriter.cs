using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.Json
{
    internal class DescriptiveJsonWriter : JsonWriter
    {
        private readonly TextWriter writer;
        private int indent;
        private string propertyName;
        private string propertyComment;
        private readonly Stack<bool> needsComma = new Stack<bool>();
        private bool needsNewLine;

        /// <summary>Whether to make the JSON as small as possible</summary>
        public bool Minify { get; set; } = false;

        /// <summary>The indentation to use</summary>
        public string Indentation { get; set; } = "    ";

        /// <summary>The quote character used to denote strings</summary>
        public char QuoteChar { get; set; } = '"';

        public DescriptiveJsonWriter(TextWriter textWriter)
        {
            this.writer = textWriter;
            this.needsComma.Push(false);
        }

        #region Overrides
        public override void Flush()
        {
            this.writer.Flush();
        }

        public override void WritePropertyName(string name)
        {
            this.propertyName = name;
        }

        public void WritePropertyComment(string comment)
        {
            this.propertyComment = comment;
        }

        public void Indent()
        {
            if (this.Minify)
            {
                return;
            }

            this.writer.Write(this.Indentation.Repeat(this.indent));
        }

        public void NextLine()
        {
            this.needsNewLine = false;
            if (this.Minify)
                return;

            this.writer.WriteLine();
            this.Indent();
        }

        private void WriteComma()
        {
            this.writer.Write(',');
            if (!this.Minify && this.propertyComment != null)
            {
                this.writer.WriteLine();
                this.Indent();
            }
        }

        public override void WriteComment(string text)
        {
            if (this.needsNewLine)
            {
                this.NextLine();
            }

            this.writer.Write($"/*{text.Replace("*/", "* /")}*/");
        }

        public override void WriteStartObject()
        {
            // Put a comma if needed
            if (this.needsComma.Pop())
            {
                this.WriteComma();
            }

            // Go to the next line if needed
            if (this.needsNewLine)
            {
                this.NextLine();
            }

            // Keep track of whether this will need a comma or not
            this.needsComma.Push(true);

            // If this is a property, write the comment and name
            this.BeginProperty();

            // Begin object
            this.writer.Write('{');
            this.needsComma.Push(false);
            this.indent++;
            this.needsNewLine = true;
        }

        public override void WriteStartArray()
        {
            // Put a comma if needed
            if (this.needsComma.Pop())
            {
                this.WriteComma();
            }

            // Go to the next line if needed
            if (this.needsNewLine)
            {
                this.NextLine();
            }

            // Keep track of whether this will need a comma or not
            this.needsComma.Push(true);

            // If this is a property, write the comment and name
            this.BeginProperty();

            // Begin array
            this.writer.Write('[');
            this.needsComma.Push(false);
            this.indent++;
            this.needsNewLine = true;
        }

        public override void WriteEndArray()
        {
            this.indent--;
            if (this.needsNewLine)
            {
                this.NextLine();
            }

            this.writer.Write(']');
            this.needsComma.Pop();

            this.needsNewLine = true;
        }

        public override void WriteEndObject()
        {
            this.indent--;
            if (this.needsNewLine)
            {
                this.NextLine();
            }

            this.writer.Write('}');
            this.needsComma.Pop();

            this.needsNewLine = true;
        }

        #region Value Writers
        public override void WriteNull()
        {
            this.WriteValueToken(JTokenType.Null, "null");
        }

        public override void WriteValue(bool value)
        {
            this.WriteValueToken(JTokenType.Boolean, value ? "true" : "false");
        }

        public override void WriteValue(sbyte value)
        {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(byte value)
        {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(short value)
        {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(ushort value)
        {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(int value)
        {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(uint value)
        {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(long value)
        {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(ulong value)
        {
            this.WriteValueToken(JTokenType.Integer, value.ToString(this.Culture));
        }

        public override void WriteValue(float value)
        {
            this.WriteValueToken(JTokenType.Float, value.ToString(this.Culture));
        }

        public override void WriteValue(double value)
        {
            this.WriteValueToken(JTokenType.Float, value.ToString(this.Culture));
        }

        public override void WriteValue(decimal value)
        {
            this.WriteValueToken(JTokenType.Float, value.ToString(this.Culture));
        }

        public override void WriteValue(char value)
        {
            this.WriteValueToken(JTokenType.String, this.ToJsonString(value.ToString()));
        }

        public override void WriteValue(string value)
        {
            this.WriteValueToken(JTokenType.String, this.ToJsonString(value));
        }

        public override void WriteValue(DateTime value)
        {
            if (string.IsNullOrEmpty(this.DateFormatString))
            {
                // ISO-8601
                this.writer.Write(value.ToString("O"));
            }
            else
            {
                // Custom date format string
                this.WriteValueToken(JTokenType.Date, $"{this.QuoteChar}{value.ToUniversalTime().ToString(this.DateFormatString, this.Culture)}{this.QuoteChar}");
            }
        }

        public override void WriteValue(DateTimeOffset value)
        {
            if (string.IsNullOrEmpty(this.DateFormatString))
            {
                // ISO-8601
                this.WriteValueToken(JTokenType.Date, $"{this.QuoteChar}{value:O}{this.QuoteChar}");
            }
            else
            {
                // Custom date format string
                this.WriteValueToken(JTokenType.Date, $"{this.QuoteChar}{value.UtcDateTime.ToString(this.DateFormatString, this.Culture)}{this.QuoteChar}");
            }
        }

        public override void WriteValue(Guid value)
        {
            this.WriteValueToken(JTokenType.String, $"{this.QuoteChar}{value.ToString("D", this.Culture)}{this.QuoteChar}");
        }

        public override void WriteValue(TimeSpan value)
        {
            this.WriteValueToken(JTokenType.TimeSpan, $"{this.QuoteChar}{value.ToString(null, this.Culture)}{this.QuoteChar}");
        }

        public override void WriteValue(Uri value)
        {
            if (value == null)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(value.OriginalString);
            }
        }

        public override void WriteValue(byte[] value)
        {
            if (value == null)
            {
                this.WriteNull();
            }
            else
            {
                this.WriteValue(Convert.ToBase64String(value));
            }
        }
        #endregion
        #endregion

        protected void WriteValueToken(JTokenType type, string value)
        {
            // Put a comma if needed
            if (this.needsComma.Pop())
            {
                this.WriteComma();
            }

            // Go to the next line if needed
            if (this.needsNewLine)
            {
                this.NextLine();
            }

            // If this is a property, write the comment and name
            this.BeginProperty();

            // Write the value
            this.writer.Write(value);

            // Indicate that a value was written and this is the end of the line
            this.needsNewLine = true;
            this.needsComma.Push(true);
        }

        protected void BeginProperty()
        {
            if (this.propertyName == null)
            {
                return;
            }

            // Write property comment
            if (this.propertyComment != null && !this.Minify)
            {
                this.writer.Write("/* ");
                this.writer.Write(this.propertyComment.Replace("*/", "* /"));
                this.writer.Write(" */");
                this.NextLine();
            }

            // Write property name
            this.writer.Write(this.ToJsonString(this.propertyName));

            // Write delimiter
            this.writer.Write(":");
            if (!this.Minify)
            {
                this.writer.Write(' ');
            }

            // Reset property name and comment
            this.propertyName = null;
            this.propertyComment = null;
        }

        protected string ToJsonString(string str)
        {
            return JsonConvert.ToString(str);
        }
    }
}