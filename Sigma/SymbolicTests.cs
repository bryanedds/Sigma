using Xunit;

namespace Sigma.Tests
{
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
        public void UnionSerializationWorks()
        {
            Assert.Equal(Conversion.ValueToString(new TestUnion(0)), "[Int 0]");
            Assert.Equal(Conversion.ValueToString(new TestUnion("")), "[String \"\"]");
        }

        [Fact]
        public void UnionDeserializationWorks()
        {
            Assert.Equal(Conversion.StringToValue<TestUnion>("[Int 0]"), new TestUnion(0));
            Assert.Equal(Conversion.StringToValue<TestUnion>("[String \"\"]"), new TestUnion("\"\""));
        }
    }
}
