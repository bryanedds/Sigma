using Xunit;

namespace Sigma.Tests
{
    public class TestRecord : Record<int, string>
    {
        public TestRecord(int i, string s) : base(i, s) { }
    }

    public class TestRecordRecord : Record<int, TestRecord>
    {
        public TestRecordRecord(int i, TestRecord t) : base(i, t) { }
    }

    public class TestUnion : Union<TestUnionTag, object>
    {
        public TestUnion(int i) : base(TestUnionTag.Int, i) { }
        public TestUnion(string s) : base(TestUnionTag.String, s) { }
        public TestUnion(TestUnionTag t, object o) : base(t, o) { }
    }

    public enum TestUnionTag
    {
        [Union(typeof(int))] Int = 0,
        [Union(typeof(string))] String
    }

    public class SymbolTests
    {
        [Fact]
        public void PrimitiveSerializationWorks()
        {
            Assert.Equal(Conversion.ValueToString(0), "0");
            Assert.Equal(Conversion.ValueToString(0.0), "0");
            Assert.Equal(Conversion.ValueToString(""), "\"\"");
            Assert.Equal(Conversion.ValueToString(" "), "\" \"");
            Assert.Equal(Conversion.ValueToString("String"), "String");
            Assert.Equal(Conversion.ValueToString(new Ref<string>("String")), "String");
        }

        [Fact]
        public void PrimitiveDeserializationWorks()
        {
            Assert.Equal(Conversion.StringToValue<int>("0"), 0);
            Assert.Equal(Conversion.StringToValue<double>("0"), 0.0);
            Assert.Equal(Conversion.StringToValue<string>(""), "");
            Assert.Equal(Conversion.StringToValue<string>(" "), " ");
            Assert.Equal(Conversion.StringToValue<string>("String"), "String");
            Assert.Equal(Conversion.StringToValue<Ref<string>>("String"), new Ref<string>("String"));
        }

        [Fact]
        public void RecordSerializationWorks()
        {
            Assert.Equal(Conversion.ValueToString(new TestRecord(0, "")), "[0 \"\"]");
            Assert.Equal(Conversion.ValueToString(new TestRecordRecord(0, new TestRecord(0, ""))), "[0 [0 \"\"]]");
        }

        [Fact]
        public void RecordDeserializationWorks()
        {
            Assert.Equal(Conversion.StringToValue<TestRecord>("[0 \"\"]"), new TestRecord(0, ""));
            Assert.Equal(Conversion.StringToValue<TestRecordRecord>("[0 [0 \"\"]]"), new TestRecordRecord(0, new TestRecord(0, "")));
        }

        [Fact]
        public void UnionSerializationWorks()
        {
            Assert.Equal(Conversion.ValueToString(new TestUnion(0)), "[Int 0]");
            Assert.Equal(Conversion.ValueToString(new TestUnion("")), "[String \"\"]");
        }

        [Fact]
        public void UnionDeserializationWorks()
        {
            Assert.Equal(Conversion.StringToValue<TestUnion>("[Int 0]"), new TestUnion(0));
            Assert.Equal(Conversion.StringToValue<TestUnion>("[String \"\"]"), new TestUnion(""));
        }
    }
}
