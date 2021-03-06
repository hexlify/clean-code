﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Markdown.Tokenizing;
using NUnit.Framework;
using FluentAssertions;

namespace MarkdownTests
{
    [TestFixture]
    public class MarkdownTokenizerShould
    {
        [TestCase("", TestName = "OnEmptyString")]
        [TestCase(null, TestName = "OnNullString")]
        public void ThrowExpetionOnInvalidString(string source)
        {
            Action act = () => MarkdownTokenizer.Tokenize(source);

            act.Should().ThrowExactly<ArgumentException>().And.Message.Should().Contain("can't be null or empty");
        }

        #region Tag.Emphasize

        [Test, TestCaseSource(nameof(WrapInEmphasizeTagTestCases))]
        public void WrapInEmphasizeTag(string source, params Token[] expected)
        {
            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        private static IEnumerable WrapInEmphasizeTagTestCases()
        {
            yield return new TestCaseData("_people_ hello", new[]
            {
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "people"),
                new Token(Tag.Emphasize, false),
                new Token(Tag.Raw, false, " hello"),
            }).SetName("when tag is at the start");

            yield return new TestCaseData("hello _people_", new[]
            {
                new Token(Tag.Raw, false, "hello "),
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "people"),
                new Token(Tag.Emphasize, false),
            }).SetName("when tag is at the end");

            yield return new TestCaseData("start _middle_ end!", new[]
            {
                new Token(Tag.Raw, false, "start "),
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "middle"),
                new Token(Tag.Emphasize, false),
                new Token(Tag.Raw, false, " end!"),
            }).SetName("when tag is in the middle");

            yield return new TestCaseData("Word _another_ pretty _word_ !", new[]
            {
                new Token(Tag.Raw, false, "Word "),
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "another"),
                new Token(Tag.Emphasize, false),
                new Token(Tag.Raw, false, " pretty "),
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "word"),
                new Token(Tag.Emphasize, false),
                new Token(Tag.Raw, false, " !"),
            }).SetName("with multiple tags");
        }

        [Test, TestCaseSource(nameof(NotWrapInEmphasizeTagTestCases))]
        public void NotWrapInEmphasizeTag(string source, params Token[] expected)
        {
            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        private static IEnumerable NotWrapInEmphasizeTagTestCases()
        {
            yield return new TestCaseData(@"hello \_people_", new[]
            {
                new Token(Tag.Raw, false, "hello _people_"),
            }).SetName("when first underscore is escaped");

            yield return new TestCaseData(@"hello \_people", new[]
            {
                new Token(Tag.Raw, false, "hello _people"),
            }).SetName("when first underscore is escaped without second underscore");

            yield return new TestCaseData(@"hello _people\_", new[]
            {
                new Token(Tag.Raw, false, @"hello _people_"),
            }).SetName("when second underscore is escaped");

            yield return new TestCaseData(@"hello people\_", new[]
            {
                new Token(Tag.Raw, false, @"hello people_"),
            }).SetName("when second underscore is escaped without first underscore");

            yield return new TestCaseData(@"hello \_people\_", new[]
            {
                new Token(Tag.Raw, false, @"hello _people_"),
            }).SetName("when both underscores are escaped");

            yield return new TestCaseData("start _end", new[]
            {
                new Token(Tag.Raw, false, "start _end"),
            }).SetName("when first underscore is unpaired");

            yield return new TestCaseData("start_ end", new[]
            {
                new Token(Tag.Raw, false, "start_ end"),
            }).SetName("when second underscore is unpaired");

            yield return new TestCaseData("text_another_text", new[]
            {
                new Token(Tag.Raw, false, "text_another_text"),
            }).SetName("underscores inside text");

            yield return new TestCaseData(" _text_text ", new[]
            {
                new Token(Tag.Raw, false, " _text_text "),
            }).SetName("when second underscore inside text");

            yield return new TestCaseData("text_text_ ", new[]
            {
                new Token(Tag.Raw, false, "text_text_ "),
            }).SetName("when first underscore inside text");

            yield return new TestCaseData("digits_12_3", new[]
            {
                new Token(Tag.Raw, false, "digits_12_3"),
            }).SetName("underscores with digits");

            yield return new TestCaseData(" _text2_3text ", new[]
            {
                new Token(Tag.Raw, false, " _text2_3text "),
            }).SetName("when second underscore near digits");

            yield return new TestCaseData("text1_text_ ", new[]
            {
                new Token(Tag.Raw, false, "text1_text_ "),
            }).SetName("when first underscore near digits");
        }

        #endregion

        #region Tag.Strong

        [Test, TestCaseSource(nameof(WrapInStrongTagTestCases))]
        public void WrapInStrongTag(string source, params Token[] expected)
        {
            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        private static IEnumerable WrapInStrongTagTestCases()
        {
            yield return new TestCaseData("__people__ hello", new[]
            {
                new Token(Tag.Strong, true),
                new Token(Tag.Raw, false, "people"),
                new Token(Tag.Strong, false),
                new Token(Tag.Raw, false, " hello"),
            }).SetName("when tag is at the start");

            yield return new TestCaseData("hello __people__", new[]
            {
                new Token(Tag.Raw, false, "hello "),
                new Token(Tag.Strong, true),
                new Token(Tag.Raw, false, "people"),
                new Token(Tag.Strong, false),
            }).SetName("when tag is at the end");

            yield return new TestCaseData("start __middle__ end!", new[]
            {
                new Token(Tag.Raw, false, "start "),
                new Token(Tag.Strong, true),
                new Token(Tag.Raw, false, "middle"),
                new Token(Tag.Strong, false),
                new Token(Tag.Raw, false, " end!"),
            }).SetName("when tag is in the middle");

            yield return new TestCaseData("Word __another__ pretty __word__ !", new[]
            {
                new Token(Tag.Raw, false, "Word "),
                new Token(Tag.Strong, true),
                new Token(Tag.Raw, false, "another"),
                new Token(Tag.Strong, false),
                new Token(Tag.Raw, false, " pretty "),
                new Token(Tag.Strong, true),
                new Token(Tag.Raw, false, "word"),
                new Token(Tag.Strong, false),
                new Token(Tag.Raw, false, " !"),
            }).SetName("with multiple tags");
        }

        [Test, TestCaseSource(nameof(NotWrapInStrongTagTestCases))]
        public void NotWrapInStrongTag(string source, params Token[] expected)
        {
            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        private static IEnumerable NotWrapInStrongTagTestCases()
        {
            yield return new TestCaseData(@"hello \__people__", new[]
            {
                new Token(Tag.Raw, false, @"hello __people__"),

            }).SetName("when first double underscore is escaped");

            yield return new TestCaseData(@"hello \__people", new[]
            {
                new Token(Tag.Raw, false, "hello __people"),
            }).SetName("when first double underscore is escaped without second double underscore");

            yield return new TestCaseData(@"hello __people\__", new[]
            {
                new Token(Tag.Raw, false, @"hello __people__"),
            }).SetName("when second double underscore is escaped");

            yield return new TestCaseData(@"hello people\__", new[]
            {
                new Token(Tag.Raw, false, @"hello people__"),
            }).SetName("when second double underscore is escaped without first double underscore");

            yield return new TestCaseData(@"hello \__people\__", new[]
            {
                new Token(Tag.Raw, false, @"hello __people__"),
            }).SetName("when both double underscores are escaped");

            yield return new TestCaseData("start __end", new[]
            {
                new Token(Tag.Raw, false, "start __end"),
            }).SetName("when first double underscore is unpaired");

            yield return new TestCaseData("start__ end", new[]
            {
                new Token(Tag.Raw, false, "start__ end"),
            }).SetName("when second double underscore is unpaired");

            yield return new TestCaseData("text__another__text", new[]
            {
                new Token(Tag.Raw, false, "text__another__text"),
            }).SetName("double underscores in the middle of text");

            yield return new TestCaseData(" __text__text ", new[]
            {
                new Token(Tag.Raw, false, " __text__text "),
            }).SetName("when second double underscore inside text");

            yield return new TestCaseData("text__text__ ", new[]
            {
                new Token(Tag.Raw, false, "text__text__ "),
            }).SetName("when first double underscore inside text");

            yield return new TestCaseData("digits__12__3", new[]
            {
                new Token(Tag.Raw, false, "digits__12__3"),
            }).SetName("double underscores with digits");

            yield return new TestCaseData(" __text2__3text ", new[]
            {
                new Token(Tag.Raw, false, " __text2__3text "),
            }).SetName("when second double underscore near digits");

            yield return new TestCaseData("text1__text__ ", new[]
            {
                new Token(Tag.Raw, false, "text1__text__ "),
            }).SetName("when first double underscore near digits");
        }

        #endregion

        #region Tag.Pre

        [Test, TestCaseSource(nameof(WrapInPreTagTestCases))]
        public void WrapInPreTag(string source, params Token[] expected)
        {
            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        private static IEnumerable WrapInPreTagTestCases()
        {
            yield return new TestCaseData("`people` hello", new[]
            {
                new Token(Tag.Pre, true),
                new Token(Tag.Raw, false, "people"),
                new Token(Tag.Pre, false),
                new Token(Tag.Raw, false, " hello"),
            }).SetName("when tag is at the start");

            yield return new TestCaseData("hello `people`", new[]
            {
                new Token(Tag.Raw, false, "hello "),
                new Token(Tag.Pre, true),
                new Token(Tag.Raw, false, "people"),
                new Token(Tag.Pre, false),
            }).SetName("when tag is at the end");

            yield return new TestCaseData("start `middle` end!", new[]
            {
                new Token(Tag.Raw, false, "start "),
                new Token(Tag.Pre, true),
                new Token(Tag.Raw, false, "middle"),
                new Token(Tag.Pre, false),
                new Token(Tag.Raw, false, " end!"),
            }).SetName("when tag is in the middle");

            yield return new TestCaseData("Word `another` pretty `word` !", new[]
            {
                new Token(Tag.Raw, false, "Word "),
                new Token(Tag.Pre, true),
                new Token(Tag.Raw, false, "another"),
                new Token(Tag.Pre, false),
                new Token(Tag.Raw, false, " pretty "),
                new Token(Tag.Pre, true),
                new Token(Tag.Raw, false, "word"),
                new Token(Tag.Pre, false),
                new Token(Tag.Raw, false, " !"),
            }).SetName("with multiple tags");
        }

        [Test, TestCaseSource(nameof(NotWrapInPreTagTestCases))]
        public void NotWrapInPreTag(string source, params Token[] expected)
        {
            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        private static IEnumerable NotWrapInPreTagTestCases()
        {
            yield return new TestCaseData(@"hello \`people`", new[]
            {
                new Token(Tag.Raw, false, "hello `people`"),
            }).SetName("when first ` is escaped");

            yield return new TestCaseData(@"hello \`people", new[]
            {
                new Token(Tag.Raw, false, "hello `people"),
            }).SetName("when first ` is escaped without second underscore");

            yield return new TestCaseData(@"hello `people\`", new[]
            {
                new Token(Tag.Raw, false, @"hello `people`"),
            }).SetName("when second ` is escaped");

            yield return new TestCaseData(@"hello people\`", new[]
            {
                new Token(Tag.Raw, false, @"hello people`"),
            }).SetName("when second ` is escaped without first underscore");

            yield return new TestCaseData(@"hello \`people\`", new[]
            {
                new Token(Tag.Raw, false, @"hello `people`"),
            }).SetName("when both ` are escaped");

            yield return new TestCaseData("start `end", new[]
            {
                new Token(Tag.Raw, false, "start `end"),
            }).SetName("when first ` is unpaired");

            yield return new TestCaseData("start` end", new[]
            {
                new Token(Tag.Raw, false, "start` end"),
            }).SetName("when second ` is unpaired");

            yield return new TestCaseData("text`another`text", new[]
            {
                new Token(Tag.Raw, false, "text`another`text"),
            }).SetName("` inside text");

            yield return new TestCaseData(" `text`text ", new[]
            {
                new Token(Tag.Raw, false, " `text`text "),
            }).SetName("when second ` inside text");

            yield return new TestCaseData("text`text` ", new[]
            {
                new Token(Tag.Raw, false, "text`text` "),
            }).SetName("when first ` inside text");

            yield return new TestCaseData("digits`12`3", new[]
            {
                new Token(Tag.Raw, false, "digits`12`3"),
            }).SetName("` with digits");

            yield return new TestCaseData(" `text2`3text ", new[]
            {
                new Token(Tag.Raw, false, " `text2`3text "),
            }).SetName("when ` underscore near digits");

            yield return new TestCaseData("text1`text` ", new[]
            {
                new Token(Tag.Raw, false, "text1`text` "),
            }).SetName("when first ` near digits");
        }

        #endregion

        #region Tags Combination

        [Test]
        public void WrapInEmpasizeTagInsideStrong()
        {
            var source = "__strong _em em_ strong__";
            var expected = new[]
            {
                new Token(Tag.Strong, true),
                new Token(Tag.Raw, false, "strong "),
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "em em"),
                new Token(Tag.Emphasize, false),
                new Token(Tag.Raw, false, " strong"),
                new Token(Tag.Strong, false),
            };

            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Test]
        public void NotWrapInStrongTagInsideEmphasize()
        {
            var source = "_em __strong strong__ em_";
            var expected = new[]
            {
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "em __strong strong__ em"),
                new Token(Tag.Emphasize, false),
            };

            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        [Test, TestCaseSource(nameof(CombinationTestCases))]
        public void NotWrapUnpairedTagsInsideEachOther(string source, params Token[] expected)
        {
            MarkdownTokenizer.Tokenize(source).Should()
                .BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }

        private static IEnumerable CombinationTestCases()
        {
            yield return new TestCaseData("__strong _strong__ text", new[]
            {
                new Token(Tag.Strong, true),
                new Token(Tag.Raw, false, "strong _strong"),
                new Token(Tag.Strong, false),
                new Token(Tag.Raw, false, " text")
            }).SetName("unpaired starting underscore inside paired double underscores");

            yield return new TestCaseData("__strong_ strong__ text", new[]
            {
                new Token(Tag.Strong, true),
                new Token(Tag.Raw, false, "strong_ strong"),
                new Token(Tag.Strong, false),
                new Token(Tag.Raw, false, " text")
            }).SetName("unpaired ending underscore inside paired double underscores");

            yield return new TestCaseData("__strong _strong__ text_", new[]
            {
                new Token(Tag.Raw, false, "__strong "),
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "strong__ text"),
                new Token(Tag.Emphasize, false),
            }).SetName("starting underscore inside paired double underscores");

            yield return new TestCaseData("_em __em_ text", new[]
            {
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "em __em"),
                new Token(Tag.Emphasize, false),
                new Token(Tag.Raw, false, " text")
            }).SetName("unpaired starting double underscore inside paired underscores");

            yield return new TestCaseData("_em__ em_ text", new[]
            {
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "em__ em"),
                new Token(Tag.Emphasize, false),
                new Token(Tag.Raw, false, " text")
            }).SetName("unpaired ending double underscore inside paired underscores");

            yield return new TestCaseData("_em __em_ text__", new[]
            {
                new Token(Tag.Emphasize, true),
                new Token(Tag.Raw, false, "em __em"),
                new Token(Tag.Emphasize, false),
                new Token(Tag.Raw, false, " text__")
            }).SetName("starting double underscore inside paired underscores");
        }

        #endregion

        #region Execution Time

        [Test, TestCaseSource(nameof(ExecutionTimeTestCases))]
        public void ExecuteInLinearTime(string source, string tripleLengthSource)
        {
            Action tokenize = () => MarkdownTokenizer.Tokenize(source);
            Action tokenizeTriple = () => MarkdownTokenizer.Tokenize(tripleLengthSource);

            MarkdownTokenizer.Tokenize("__test _t_ test__");  // warmup

            var watch = Stopwatch.StartNew();
            tokenize();
            var tokenizeTimeElapsed = watch.ElapsedMilliseconds;
            watch.Restart();
            tokenizeTriple();
            var tokenizeTripleTimeElapsed = watch.ElapsedMilliseconds;
            watch.Stop();

            tokenizeTripleTimeElapsed.Should().BeLessOrEqualTo(5 * tokenizeTimeElapsed);
        }

        private static IEnumerable ExecutionTimeTestCases()
        {
            var sourceLength = 1000000;

            yield return new TestCaseData(
                GenerateSimpleMarkdownString(sourceLength),
                GenerateSimpleMarkdownString(sourceLength * 3)).SetName("OnSimpleString");

            yield return new TestCaseData(
                GenerateMarkdownStringWithNestedTags(sourceLength),
                GenerateMarkdownStringWithNestedTags(sourceLength * 3)).SetName("OnStringWithNestedTags");

            yield return new TestCaseData(
                GenerateMarkdownStringWithALotOfRawTags(sourceLength),
                GenerateMarkdownStringWithALotOfRawTags(sourceLength * 3)).SetName("OnStringWithALotOfRawTags");
        }

        private static string GenerateSimpleMarkdownString(int length)
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < (length - 4) / 5; i++)
                stringBuilder.Append("word ");
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return "__" + stringBuilder + "__";
        }

        private static string GenerateMarkdownStringWithNestedTags(int length)
        {
            var prefix = "__c ";
            var suffix = " c__";
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < (length - 5) / 8; i++)
                stringBuilder.Append(prefix);
            stringBuilder.Append("__c__");
            for (var i = 0; i < (length - 5) / 8; i++)
                stringBuilder.Append(suffix);

            return stringBuilder.ToString();
        }

        private static string GenerateMarkdownStringWithALotOfRawTags(int length)
        {
            var prefix = "__c ";
            var suffix = " c_";
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < (length - 4) / 7; i++)
                stringBuilder.Append(prefix);
            stringBuilder.Append("__c_");
            for (var i = 0; i < (length - 4) / 7; i++)
                stringBuilder.Append(suffix);

            return stringBuilder.ToString();
        }

        #endregion
    }
}