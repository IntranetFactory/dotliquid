using System;
using System.Globalization;
using System.Threading;
using System.Linq;
using NUnit.Framework;

namespace DotLiquid.Tests
{
    [TestFixture]
    public class StandardFilterTests
    {
        [Test]
        public void TestSize()
        {
            Assert.AreEqual(3, StandardFilters.size(new[] { 1, 2, 3 }));
            Assert.AreEqual(0, StandardFilters.size(new object[] { }));
            Assert.AreEqual(0, StandardFilters.size(null));
        }

        [Test]
        public void TestDowncase()
        {
            Assert.AreEqual("testing", StandardFilters.downcase("Testing"));
            Assert.AreEqual(null, StandardFilters.downcase(null));
        }

        [Test]
        public void TestUpcase()
        {
            Assert.AreEqual("TESTING", StandardFilters.upcase("Testing"));
            Assert.AreEqual(null, StandardFilters.upcase(null));
        }

        [Test]
        public void TestTruncate()
        {
            Assert.AreEqual(null, StandardFilters.truncate(null));
            Assert.AreEqual("", StandardFilters.truncate(""));
            Assert.AreEqual("1234...", StandardFilters.truncate("1234567890", 7));
            Assert.AreEqual("1234567890", StandardFilters.truncate("1234567890", 20));
            Assert.AreEqual("...", StandardFilters.truncate("1234567890", 0));
            Assert.AreEqual("1234567890", StandardFilters.truncate("1234567890"));
        }

        [Test]
        public void TestEscape()
        {
            Assert.AreEqual(null, StandardFilters.escape(null));
            Assert.AreEqual("", StandardFilters.escape(""));
            Assert.AreEqual("&lt;strong&gt;", StandardFilters.escape("<strong>"));
            Assert.AreEqual("&lt;strong&gt;", StandardFilters.h("<strong>"));
        }

        [Test]
        public void TestEscape_CSharpNamingConvention()
        {
            Template.NamingConvention = new NamingConventions.CSharpNamingConvention();
            Helper.AssertTemplateResult("a\\r\\nb\\r\\nc",
                "{{ source | escape }}",
                Hash.FromAnonymousObject(new { source = "a\r\nb\r\nc" }));
        }

        [Test]
        public void TestJsonEscape_RubyNamingConvention()
        {
            Template.NamingConvention = new NamingConventions.RubyNamingConvention();
            Helper.AssertTemplateResult("a\\r\\nb\\r\\nc",
                "{{ source | json_escape }}",
                Hash.FromAnonymousObject(new { source = "a\r\nb\r\nc" }));
        }

        [Test]
        public void TestJsonEscape_CSharpNamingConvention()
        {
            Template.NamingConvention = new NamingConventions.CSharpNamingConvention();
            Helper.AssertTemplateResult("a\\r\\nb\\r\\nc",
                "{{ source | json_escape }}",
                Hash.FromAnonymousObject(new { source = "a\r\nb\r\nc" }));
        }

        [Test]
        public void Testescape()
        {
            Helper.AssertTemplateResult("&lt;strong&gt;",
                "{{ source | escape }}",
                Hash.FromAnonymousObject(new { source = "<strong>" }));
        }

        [Test]
        public void TestCSharpNamingConventionProblem()
        {

            //Template.NamingConvention = new NamingConventions.RubyNamingConvention();
            //Template.NamingConvention = new NamingConventions.CSharpNamingConvention();
            Template template = Template.Parse("1{{testValue}}2");
            Assert.AreEqual("1abc2", template.Render(Hash.FromAnonymousObject(new { testValue = "abc" })));

        }

        //[Test]
        //public void TestCSharpNamingConventionProblemWithJsonEscape()
        //{//this don't add filters, so json_escape cannot be tested in 'Render' call such this one

        //    Template.NamingConvention = new NamingConventions.CSharpNamingConvention();
        //    Template template = Template.Parse("1{{testValue}}2 | json_escape");
        //    Assert.AreEqual("1abc2", template.Render(Hash.FromAnonymousObject(new { testValue = @"a""bc" })));

        //}

        [Test]
        public void TestTruncateWords()
        {
            Assert.AreEqual(null, StandardFilters.truncate_words(null));
            Assert.AreEqual("", StandardFilters.truncate_words(""));
            Assert.AreEqual("one two three", StandardFilters.truncate_words("one two three", 4));
            Assert.AreEqual("one two...", StandardFilters.truncate_words("one two three", 2));
            Assert.AreEqual("one two three", StandardFilters.truncate_words("one two three"));
            Assert.AreEqual("Two small (13&#8221; x 5.5&#8221; x 10&#8221; high) baskets fit inside one large basket (13&#8221;...", StandardFilters.truncate_words("Two small (13&#8221; x 5.5&#8221; x 10&#8221; high) baskets fit inside one large basket (13&#8221; x 16&#8221; x 10.5&#8221; high) with cover.", 15));
        }

        [Test]
        public void TestSplit()
        {
            Assert.AreEqual(new[] { "This", "is", "a", "sentence" }, StandardFilters.split("This is a sentence", " "));
            Assert.AreEqual(new string[] { null }, StandardFilters.split(null, null));
        }

        [Test]
        public void TestStripHtml()
        {
            Assert.AreEqual("test", StandardFilters.strip_html("<div>test</div>"));
            Assert.AreEqual("test", StandardFilters.strip_html("<div id='test'>test</div>"));
            Assert.AreEqual(null, StandardFilters.strip_html(null));
        }

        [Test]
        public void TestStrip()
        {
            Assert.AreEqual("test", StandardFilters.strip("  test  "));
            Assert.AreEqual("test", StandardFilters.strip("   test"));
            Assert.AreEqual("test", StandardFilters.strip("test   "));
            Assert.AreEqual("test", StandardFilters.strip("test"));
            Assert.AreEqual(null, StandardFilters.strip(null));
        }

        [Test]
        public void TestLStrip()
        {
            Assert.AreEqual("test  ", StandardFilters.lstrip("  test  "));
            Assert.AreEqual("test", StandardFilters.lstrip("   test"));
            Assert.AreEqual("test   ", StandardFilters.lstrip("test   "));
            Assert.AreEqual("test", StandardFilters.lstrip("test"));
            Assert.AreEqual(null, StandardFilters.lstrip(null));
        }

        [Test]
        public void TestRStrip()
        {
            Assert.AreEqual("  test", StandardFilters.rstrip("  test  "));
            Assert.AreEqual("   test", StandardFilters.rstrip("   test"));
            Assert.AreEqual("test", StandardFilters.rstrip("test   "));
            Assert.AreEqual("test", StandardFilters.rstrip("test"));
            Assert.AreEqual(null, StandardFilters.rstrip(null));
        }

        [Test]
        public void TestJoin()
        {
            Assert.AreEqual(null, StandardFilters.join(null));
            Assert.AreEqual("", StandardFilters.join(""));
            Assert.AreEqual("1 2 3 4", StandardFilters.join(new[] { 1, 2, 3, 4 }));
            Assert.AreEqual("1 - 2 - 3 - 4", StandardFilters.join(new[] { 1, 2, 3, 4 }, " - "));
        }

        [Test]
        public void TestSort()
        {
            Assert.AreEqual(null, StandardFilters.sort(null));
            CollectionAssert.AreEqual(new string[] { }, StandardFilters.sort(new string[] { }));
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, StandardFilters.sort(new[] { 4, 3, 2, 1 }));
            CollectionAssert.AreEqual(new[] { new { a = 1 }, new { a = 2 }, new { a = 3 }, new { a = 4 } },
                StandardFilters.sort(new[] { new { a = 4 }, new { a = 3 }, new { a = 1 }, new { a = 2 } }, "a"));
        }

        [Test]
        public void TestSort_OnHashList_WithProperty_DoesNotFlattenList()
        {
            var list = new System.Collections.Generic.List<Hash>();
            var hash1 = CreateHash("1", "Text1");
            var hash2 = CreateHash("2", "Text2");
            var hash3 = CreateHash("3", "Text3");
            list.Add(hash3);
            list.Add(hash1);
            list.Add(hash2);

            var result = StandardFilters.sort(list, "sortby").Cast<Hash>().ToArray();
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(hash1["content"], result[0]["content"]);
            Assert.AreEqual(hash2["content"], result[1]["content"]);
            Assert.AreEqual(hash3["content"], result[2]["content"]);
        }

        private static Hash CreateHash(string sortby, string content)
        {
            var hash = new Hash();
            hash.Add("sortby", sortby);
            hash.Add("content", content);
            return hash;
        }

        [Test]
        public void TestMap()
        {
            CollectionAssert.AreEqual(new string[] { }, StandardFilters.map(new string[] { }, "a"));
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 },
                StandardFilters.map(new[] { new { a = 1 }, new { a = 2 }, new { a = 3 }, new { a = 4 } }, "a"));
            Helper.AssertTemplateResult("abc", "{{ ary | map:'foo' | map:'bar' }}",
                Hash.FromAnonymousObject(
                    new
                    {
                        ary =
                            new[]
                    {
                        Hash.FromAnonymousObject(new { foo = Hash.FromAnonymousObject(new { bar = "a" }) }), Hash.FromAnonymousObject(new { foo = Hash.FromAnonymousObject(new { bar = "b" }) }),
                        Hash.FromAnonymousObject(new { foo = Hash.FromAnonymousObject(new { bar = "c" }) })
                    }
                    }));
            CollectionAssert.AreEqual(new[] { new { a = 1 }, new { a = 2 }, new { a = 3 }, new { a = 4 } },
                StandardFilters.map(new[] { new { a = 1 }, new { a = 2 }, new { a = 3 }, new { a = 4 } }, "b"));

            Assert.AreEqual(null, StandardFilters.map(null, "a"));
            CollectionAssert.AreEqual(new object[] { null }, StandardFilters.map(new object[] { null }, "a"));

            var hash = Hash.FromAnonymousObject(new {
                ary = new[] {
                    new Helper.DataObject { PropAllowed = "a", PropDisallowed = "x" },
                    new Helper.DataObject { PropAllowed = "b", PropDisallowed = "y" },
                    new Helper.DataObject { PropAllowed = "c", PropDisallowed = "z" },
                }
            });

            Helper.AssertTemplateResult("abc", "{{ ary | map:'prop_allowed' | join:'' }}", hash);
            Helper.AssertTemplateResult("", "{{ ary | map:'prop_disallowed' | join:'' }}", hash);

            hash = Hash.FromAnonymousObject(new
            {
                ary = new[] {
                    new Helper.DataObjectDrop { Prop = "a" },
                    new Helper.DataObjectDrop { Prop = "b" },
                    new Helper.DataObjectDrop { Prop = "c" },
                }
            });

            Helper.AssertTemplateResult("abc", "{{ ary | map:'prop' | join:'' }}", hash);
            Helper.AssertTemplateResult("", "{{ ary | map:'no_prop' | join:'' }}", hash);
        }

        [TestCase("6.72", "$6.72")]
        [TestCase("6000", "$6,000.00")]
        [TestCase("6000000", "$6,000,000.00")]
        [TestCase("6000.4", "$6,000.40")]
        [TestCase("6000000.4", "$6,000,000.40")]
        [TestCase("6.8458", "$6.85")]
        public void TestAmericanCurrencyFromString(string input, string expected)
        {
#if CORE
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
#else
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
#endif
            Assert.AreEqual(expected, StandardFilters.currency(input));
        }

        [TestCase("6.72", "6,72 Ã¢â€šÂ¬")]
        [TestCase("6000", "6.000,00 Ã¢â€šÂ¬")]
        [TestCase("6000000", "6.000.000,00 Ã¢â€šÂ¬")]
        [TestCase("6000.4", "6.000,40 Ã¢â€šÂ¬")]
        [TestCase("6000000.4", "6.000.000,40 Ã¢â€šÂ¬")]
        [TestCase("6.8458", "6,85 Ã¢â€šÂ¬")]
        public void TestEuroCurrencyFromString(string input, string expected)
        {//it is known that test fails for now due 'Ã¢â€šÂ¬'
#if CORE
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
#else
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
#endif
            Assert.AreEqual(expected, StandardFilters.currency(input, "de-DE"));
        }

        [Test]
        public void TestMalformedCurrency()
        {
            Assert.AreEqual("teststring", StandardFilters.currency("teststring", "de-DE"));
        }

        [Test]
        public void TestCurrencyWithinTemplateRender()
        {//it is known that test fails for now due 'Ã¢â€šÂ¬'
#if CORE
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
#else
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
#endif

            Template dollarTemplate = Template.Parse(@"{{ amount | currency }}");
            Template euroTemplate = Template.Parse(@"{{ amount | currency: ""de-DE"" }}");

            Assert.AreEqual("$7,000.00", dollarTemplate.Render(Hash.FromAnonymousObject(new { amount = "7000" })));
            Assert.AreEqual("7.000,00 Ã¢â€šÂ¬", euroTemplate.Render(Hash.FromAnonymousObject(new { amount = 7000 })));
        }

        [Test]
        public void TestCurrencyFromDoubleInput()
        {//it is known that test fails for now due 'Ã¢â€šÂ¬'
            Assert.AreEqual("$6.85", StandardFilters.currency(6.8458, "en-US"));
            Assert.AreEqual("$6.72", StandardFilters.currency(6.72, "en-CA"));
            Assert.AreEqual("6.000.000,00 Ã¢â€šÂ¬", StandardFilters.currency(6000000, "de-DE"));
            Assert.AreEqual("6.000.000,78 Ã¢â€šÂ¬", StandardFilters.currency(6000000.78, "de-DE"));
        }

        [Test]
        public void TestDate()
        {
            Liquid.UseRubyDateFormat = false;
            DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;

            Assert.AreEqual(dateTimeFormat.GetMonthName(5), StandardFilters.date(DateTime.Parse("2006-05-05 10:00:00"), "MMMM"));
            Assert.AreEqual(dateTimeFormat.GetMonthName(6), StandardFilters.date(DateTime.Parse("2006-06-05 10:00:00"), "MMMM"));
            Assert.AreEqual(dateTimeFormat.GetMonthName(7), StandardFilters.date(DateTime.Parse("2006-07-05 10:00:00"), "MMMM"));

            Assert.AreEqual(dateTimeFormat.GetMonthName(5), StandardFilters.date("2006-05-05 10:00:00", "MMMM"));
            Assert.AreEqual(dateTimeFormat.GetMonthName(6), StandardFilters.date("2006-06-05 10:00:00", "MMMM"));
            Assert.AreEqual(dateTimeFormat.GetMonthName(7), StandardFilters.date("2006-07-05 10:00:00", "MMMM"));

            Assert.AreEqual("08/01/2006 10:00:00", StandardFilters.date("08/01/2006 10:00:00", string.Empty));
            Assert.AreEqual("08/02/2006 10:00:00", StandardFilters.date("08/02/2006 10:00:00", null));
            Assert.AreEqual(new DateTime(2006, 8, 3, 10, 0, 0).ToString(), StandardFilters.date(new DateTime(2006, 8, 3, 10, 0, 0), string.Empty));
            Assert.AreEqual(new DateTime(2006, 8, 4, 10, 0, 0).ToString(), StandardFilters.date(new DateTime(2006, 8, 4, 10, 0, 0), null));

            Assert.AreEqual(new DateTime(2006, 7, 5).ToString("MM/dd/yyyy"), StandardFilters.date("2006-07-05 10:00:00", "MM/dd/yyyy"));

            Assert.AreEqual(new DateTime(2004, 7, 16).ToString("MM/dd/yyyy"), StandardFilters.date("Fri Jul 16 2004 01:00:00", "MM/dd/yyyy"));

            Assert.AreEqual(null, StandardFilters.date(null, "MMMM"));

            Assert.AreEqual("hi", StandardFilters.date("hi", "MMMM"));

            Assert.AreEqual(DateTime.Now.ToString("MM/dd/yyyy"), StandardFilters.date("now", "MM/dd/yyyy"));
            Assert.AreEqual(DateTime.Now.ToString("MM/dd/yyyy"), StandardFilters.date("today", "MM/dd/yyyy"));
            Assert.AreEqual(DateTime.Now.ToString("MM/dd/yyyy"), StandardFilters.date("Now", "MM/dd/yyyy"));
            Assert.AreEqual(DateTime.Now.ToString("MM/dd/yyyy"), StandardFilters.date("Today", "MM/dd/yyyy"));

            Assert.AreEqual(DateTime.Now.ToString(), StandardFilters.date("now", null));
            Assert.AreEqual(DateTime.Now.ToString(), StandardFilters.date("today", null));
            Assert.AreEqual(DateTime.Now.ToString(), StandardFilters.date("now", string.Empty));
            Assert.AreEqual(DateTime.Now.ToString(), StandardFilters.date("today", string.Empty));

            Assert.AreEqual("345000", StandardFilters.date(DateTime.Parse("2006-05-05 10:00:00.345"), "ffffff"));

            Template template = Template.Parse(@"{{ hi | date:""MMMM"" }}");
            Assert.AreEqual("hi", template.Render(Hash.FromAnonymousObject(new { hi = "hi" })));
        }

        [Test]
        public void TestStrFTime()
        {
            Liquid.UseRubyDateFormat = true;
            DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;

            Assert.AreEqual(dateTimeFormat.GetMonthName(5), StandardFilters.date(DateTime.Parse("2006-05-05 10:00:00"), "%B"));
            Assert.AreEqual(dateTimeFormat.GetMonthName(6), StandardFilters.date(DateTime.Parse("2006-06-05 10:00:00"), "%B"));
            Assert.AreEqual(dateTimeFormat.GetMonthName(7), StandardFilters.date(DateTime.Parse("2006-07-05 10:00:00"), "%B"));

            Assert.AreEqual(dateTimeFormat.GetMonthName(5), StandardFilters.date("2006-05-05 10:00:00", "%B"));
            Assert.AreEqual(dateTimeFormat.GetMonthName(6), StandardFilters.date("2006-06-05 10:00:00", "%B"));
            Assert.AreEqual(dateTimeFormat.GetMonthName(7), StandardFilters.date("2006-07-05 10:00:00", "%B"));

            Assert.AreEqual("05/07/2006 10:00:00", StandardFilters.date("05/07/2006 10:00:00", string.Empty));
            Assert.AreEqual("05/07/2006 10:00:00", StandardFilters.date("05/07/2006 10:00:00", null));
            Assert.AreEqual(new DateTime(2006, 8, 3, 10, 0, 0).ToString(), StandardFilters.date(new DateTime(2006, 8, 3, 10, 0, 0), string.Empty));
            Assert.AreEqual(new DateTime(2006, 8, 4, 10, 0, 0).ToString(), StandardFilters.date(new DateTime(2006, 8, 4, 10, 0, 0), null));

            Assert.AreEqual("07/05/2006", StandardFilters.date("2006-07-05 10:00:00", "%m/%d/%Y"));

            Assert.AreEqual("07/16/2004", StandardFilters.date("Fri Jul 16 2004 01:00:00", "%m/%d/%Y"));

            Assert.AreEqual(null, StandardFilters.date(null, "%M"));

            Assert.AreEqual("hi", StandardFilters.date("hi", "%M"));

            Template template = Template.Parse(@"{{ hi | date:""%M"" }}");
            Assert.AreEqual("hi", template.Render(Hash.FromAnonymousObject(new { hi = "hi" })));
        }

        [Test]
        public void TestFirstLast()
        {
            Assert.Null(StandardFilters.first(null));
            Assert.Null(StandardFilters.last(null));
            Assert.AreEqual(1, StandardFilters.first(new[] { 1, 2, 3 }));
            Assert.AreEqual(3, StandardFilters.last(new[] { 1, 2, 3 }));
            Assert.Null(StandardFilters.first(new object[] { }));
            Assert.Null(StandardFilters.last(new object[] { }));
        }

        [Test]
        public void TestReplace()
        {
            Assert.Null(StandardFilters.replace(null, "a", "b"));
            Assert.AreEqual("", StandardFilters.replace("", "a", "b"));
            Assert.AreEqual("a a a a", StandardFilters.replace("a a a a", null, "b"));
            Assert.AreEqual("a a a a", StandardFilters.replace("a a a a", "", "b"));
            Assert.AreEqual("b b b b", StandardFilters.replace("a a a a", "a", "b"));
        }

        [Test]
        public void TestReplaceFirst()
        {
            Assert.Null(StandardFilters.replace_first(null, "a", "b"));
            Assert.AreEqual("", StandardFilters.replace_first("", "a", "b"));
            Assert.AreEqual("a a a a", StandardFilters.replace_first("a a a a", null, "b"));
            Assert.AreEqual("a a a a", StandardFilters.replace_first("a a a a", "", "b"));
            Assert.AreEqual("b a a a", StandardFilters.replace_first("a a a a", "a", "b"));
            Helper.AssertTemplateResult("b a a a", "{{ 'a a a a' | replace_first: 'a', 'b' }}");
        }

        [Test]
        public void TestRemove()
        {
            Assert.AreEqual("   ", StandardFilters.remove("a a a a", "a"));
            Assert.AreEqual("a a a", StandardFilters.remove_first("a a a a", "a "));
            Helper.AssertTemplateResult("a a a", "{{ 'a a a a' | remove_first: 'a ' }}");
        }

        [Test]
        public void TestPipesInStringArguments()
        {
            Helper.AssertTemplateResult("foobar", "{{ 'foo|bar' | remove: '|' }}");
        }

        [Test]
        public void TestStripWindowsNewlines()
        {
            Helper.AssertTemplateResult("abc", "{{ source | strip_newlines }}", Hash.FromAnonymousObject(new { source = "a\r\nb\r\nc" }));
            Helper.AssertTemplateResult("ab", "{{ source | strip_newlines }}", Hash.FromAnonymousObject(new { source = "a\r\n\r\n\r\nb" }));
        }

        [Test]
        public void TestStripUnixNewlines()
        {
            Helper.AssertTemplateResult("abc", "{{ source | strip_newlines }}", Hash.FromAnonymousObject(new { source = "a\nb\nc" }));
            Helper.AssertTemplateResult("ab", "{{ source | strip_newlines }}", Hash.FromAnonymousObject(new { source = "a\n\n\nb" }));
        }

        [Test]
        public void TestWindowsNewlinesToBr()
        {
            Helper.AssertTemplateResult("a<br />\r\nb<br />\r\nc",
                "{{ source | newline_to_br }}",
                Hash.FromAnonymousObject(new { source = "a\r\nb\r\nc" }));
        }

        [Test]
        public void TestUnixNewlinesToBr()
        {
            Helper.AssertTemplateResult("a<br />\nb<br />\nc",
                "{{ source | newline_to_br }}",
                Hash.FromAnonymousObject(new { source = "a\nb\nc" }));
        }

        [Test]
        public void TestPlus()
        {
            using (CultureHelper.SetCulture("en-GB"))
            {
                Helper.AssertTemplateResult("2", "{{ 1 | plus:1 }}");
                Helper.AssertTemplateResult("5.5", "{{ 2  | plus:3.5 }}");
                Helper.AssertTemplateResult("5.5", "{{ 3.5 | plus:2 }}");
                Helper.AssertTemplateResult("11", "{{ '1' | plus:'1' }}");
            }
        }

        [Test]
        public void TestMinus()
        {
            using (CultureHelper.SetCulture("en-GB"))
            {
                Helper.AssertTemplateResult("4", "{{ input | minus:operand }}", Hash.FromAnonymousObject(new { input = 5, operand = 1 }));
                Helper.AssertTemplateResult("-1.5", "{{ 2  | minus:3.5 }}");
                Helper.AssertTemplateResult("1.5", "{{ 3.5 | minus:2 }}");
            }
        }

        [Test]
        public void TestMinusWithFrenchDecimalSeparator()
        {
            using (CultureHelper.SetCulture("fr-FR"))
            {
                Helper.AssertTemplateResult(string.Format("1{0}2", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator),
                    "{{ 3,2 | minus:2 | round:1 }}");
            }
        }

        [Test]
        public void TestRound()
        {
            using (CultureHelper.SetCulture("en-GB"))
            {
                Helper.AssertTemplateResult("1.235", "{{ 1.234678 | round:3 }}");
                Helper.AssertTemplateResult("1", "{{ 1 | round }}");

                Assert.Null(StandardFilters.round("1.2345678", "two"));
            }
        }

        [Test]
        public void TestTimes()
        {
            using (CultureHelper.SetCulture("en-GB"))
            { 
                Helper.AssertTemplateResult("12", "{{ 3 | times:4 }}");
                Helper.AssertTemplateResult("125", "{{ 10 | times:12.5 }}");
                Helper.AssertTemplateResult("125", "{{ 10.0 | times:12.5 }}");
                Helper.AssertTemplateResult("125", "{{ 12.5 | times:10 }}");
                Helper.AssertTemplateResult("125", "{{ 12.5 | times:10.0 }}");
                Helper.AssertTemplateResult("foofoofoofoo", "{{ 'foo' | times:4 }}");
            }
        }

        [Test]
        public void TestAppend()
        {
            Hash assigns = Hash.FromAnonymousObject(new { a = "bc", b = "d" });
            Helper.AssertTemplateResult("bcd", "{{ a | append: 'd'}}", assigns);
            Helper.AssertTemplateResult("bcd", "{{ a | append: b}}", assigns);
        }

        [Test]
        public void TestPrepend()
        {
            Hash assigns = Hash.FromAnonymousObject(new { a = "bc", b = "a" });
            Helper.AssertTemplateResult("abc", "{{ a | prepend: 'a'}}", assigns);
            Helper.AssertTemplateResult("abc", "{{ a | prepend: b}}", assigns);
        }

        [Test]
        public void TestDividedBy()
        {
            Helper.AssertTemplateResult("4", "{{ 12 | divided_by:3 }}");
            Helper.AssertTemplateResult("4", "{{ 14 | divided_by:3 }}");
            Helper.AssertTemplateResult("5", "{{ 15 | divided_by:3 }}");
            Assert.Null(StandardFilters.divided_by(null, 3));
            Assert.Null(StandardFilters.divided_by(4, null));
        }

        [Test]
        public void TestInt32DividedByInt64 ()
        {
            int a = 20;
            long b = 5;
            var c = a / b;
            Assert.AreEqual( c, (long)4 );


            Hash assigns = Hash.FromAnonymousObject(new { a = a, b = b});
            Helper.AssertTemplateResult("4", "{{ a | divided_by:b }}", assigns);
        }

        [Test]
        public void TestModulo()
        {
            Helper.AssertTemplateResult("1", "{{ 3 | modulo:2 }}");
            Assert.Null(StandardFilters.modulo(null, 3));
            Assert.Null(StandardFilters.modulo(4, null));
        }

        [Test]
        public void TestUrlencode()
        {
            Assert.AreEqual("http%3A%2F%2Fdotliquidmarkup.org%2F", StandardFilters.url_encode("http://dotliquidmarkup.org/"));
            Assert.AreEqual(null, StandardFilters.url_encode(null));
        }

        [Test]
        public void TestDefault()
        {
            Hash assigns = Hash.FromAnonymousObject(new { var1 = "foo", var2 = "bar" });
            Helper.AssertTemplateResult("foo", "{{ var1 | default: 'foobar' }}", assigns);
            Helper.AssertTemplateResult("bar", "{{ var2 | default: 'foobar' }}", assigns);
            Helper.AssertTemplateResult("foobar", "{{ unknownvariable | default: 'foobar' }}", assigns);
        }

        [Test]
        public void TestCapitalize()
        {
            Assert.AreEqual(null, StandardFilters.capitalize(null));
            Assert.AreEqual("", StandardFilters.capitalize(""));
            Assert.AreEqual(" ", StandardFilters.capitalize(" "));
            Assert.AreEqual("That Is One Sentence.", StandardFilters.capitalize("That is one sentence."));
        }
    }
}
