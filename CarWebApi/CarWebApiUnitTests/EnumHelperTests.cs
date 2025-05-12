using CarWebApi.Utils;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace CarWebApiUnitTests
{
    public class EnumHelperTests
    {
        [Test]
        public void GetEnumIdentifiers()
        {
            var data = EnumHelper.GetEnumIdentifiers<TestEnum>();

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Count, Is.EqualTo(4));
            Assert.That(data[0].Id, Is.EqualTo(0));
            Assert.That(data[0].Description, Is.EqualTo("None"));
            Assert.That(data[1].Id, Is.EqualTo(1));
            Assert.That(data[1].Description, Is.EqualTo("First"));
            Assert.That(data[2].Id, Is.EqualTo(2));
            Assert.That(data[2].Description, Is.EqualTo("Second"));
            Assert.That(data[3].Id, Is.EqualTo(3));
            Assert.That(data[3].Description, Is.EqualTo("Third"));
        }

        [Test]
        public void GetEnumDescription()
        {
            var data = EnumHelper.GetEnumDescription(TestEnum.Second);

            Assert.That(data, Is.Not.Null);
            Assert.That(data, Is.EqualTo("Second"));
        }

        [Test]
        public void GetEnumIdentifierById()
        {
            var data = EnumHelper.GetEnumIdentifierById<TestEnum>(3);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Id, Is.EqualTo(3));
            Assert.That(data.Description, Is.EqualTo("Third"));
        }

        [Test]
        public void GetEnumValueFromDescription()
        {
            var data = EnumHelper.GetEnumValueFromDescription<TestEnum>("First");

            Assert.That(data, Is.EqualTo(TestEnum.First));
        }
    }

    public enum TestEnum
    {
        [Description("None")]
        None,
        [Description("First")]
        First,
        [Description("Second")]
        Second,
        [Description("Third")]
        Third
    }
}
