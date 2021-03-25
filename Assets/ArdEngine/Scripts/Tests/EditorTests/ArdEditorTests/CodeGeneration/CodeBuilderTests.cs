using System;
using ArdEditor.CodeGeneration;
using NUnit.Framework;

namespace EditorTests.ArdEditorTests.CodeGeneration
{
    public sealed class CodeBuilderTests
    {
        private CodeBuilder _codeBuilder;

        [SetUp]
        public void SetUp()
        {
            _codeBuilder = new CodeBuilder(1);
        }

        [Test]
        public void Append_WhenIndentSet_GeneratesIndentBeforeAppendedText()
        {
            const string testText = "This should be indented";
            _codeBuilder.Append(testText);
            Assert.AreEqual("\t" + testText, _codeBuilder.ToString());
        }

        [Test]
        public void CurlyScope_InsideScope_IncreasesIndent()
        {
            using (_codeBuilder.CurlyScope())
            {
                _codeBuilder.AppendLine("Test");
                string result = _codeBuilder.ToString()
                    .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)[1];
                Assert.AreEqual("\t\tTest", result);
            }
        }

        [Test]
        public void CurlyScope_AfterScopeEnded_DecreasesIndent()
        {
            using (_codeBuilder.CurlyScope())
            {
                _codeBuilder.AppendLine("In Scope");
            }

            _codeBuilder.AppendLine("Test");
            string result = _codeBuilder.ToString()
                .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)[3];
            Assert.AreEqual("\tTest", result);
        }

        [Test]
        public void AppendLine_AddsEmptyNewLine()
        {
            _codeBuilder.AppendLine();
            var result = _codeBuilder.ToString();
            Assert.IsTrue(result.Contains(Environment.NewLine));
        }

        [Test]
        public void AppendLine_WithText_AddsIndentedLineWithText()
        {
            _codeBuilder.AppendLine("Test");
            var result = _codeBuilder.ToString();
            Assert.AreEqual($"\tTest{Environment.NewLine}", result);
        }

        [Test]
        public void AppendFormat_WithArgument_AddsArgumentToTheString()
        {
            _codeBuilder.AppendFormat("Test {0}", 2);
            var result = _codeBuilder.ToString();
            Assert.AreEqual($"\tTest 2", result);
        }

        [Test]
        public void Clear_RemovesAllContent()
        {
            _codeBuilder.AppendLine("Test 123")
                .AppendLine("Another Test 123");
            
            Assert.IsNotEmpty(_codeBuilder.ToString());
            _codeBuilder.Clear();
            Assert.IsEmpty(_codeBuilder.ToString());
        }
    }
}