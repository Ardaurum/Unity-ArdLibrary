using System;
using System.Text;

namespace ArdEditor.CodeGeneration
{
    public sealed class CodeBuilder
    {
        private const string DEFAULT_INDENT_SYMBOL = "\t";

        private readonly string _indentSymbol;
        private readonly StringBuilder _sb;

        private int _indent;

        public CodeBuilder(int indent = 0, string indentSymbol = DEFAULT_INDENT_SYMBOL)
        {
            _sb = new StringBuilder();
            _indent = indent;
            _indentSymbol = indentSymbol;
        }

        private CodeBuilder AppendIndent()
        {
            for (var i = 0; i < _indent; i++)
            {
                _sb.Append(_indentSymbol);
            }

            return this;
        }

        public IDisposable CurlyScope()
        {
            return new CurlyIndent(this);
        }

        public CodeBuilder Append(string text)
        {
            AppendIndent();
            _sb.Append(text);
            return this;
        }

        public CodeBuilder AppendLine()
        {
            _sb.AppendLine();
            return this;
        }

        public CodeBuilder AppendLine(string text)
        {
            AppendIndent();
            _sb.AppendLine(text);
            return this;
        }

        public CodeBuilder AppendFormat(string text, params object[] args)
        {
            AppendIndent();
            _sb.AppendFormat(text, args);
            return this;
        }

        public CodeBuilder Clear()
        {
            _sb.Clear();
            return this;
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        private sealed class CurlyIndent : IDisposable
        {
            private readonly CodeBuilder _sb;

            public CurlyIndent(CodeBuilder sb)
            {
                _sb = sb;
                _sb.AppendLine("{");
                _sb._indent++;
            }

            public void Dispose()
            {
                _sb._indent--;
                _sb.AppendLine("}");
            }
        }
    }
}