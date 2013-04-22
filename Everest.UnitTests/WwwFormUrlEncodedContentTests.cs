using System.Collections.Generic;
using System.IO;
using Everest.Content;
using NUnit.Framework;

namespace Everest.UnitTests
{
    [TestFixture]
    public class WwwFormUrlEncodedContentTests
    {
        [Test]
        public void CanConstructContentFromDictionary()
        {
            var dictionary = new Dictionary<string, string>
                {
                    {"Value1", "Alpha"},
                    {"Value2", "Bravo"},
                    {"Value3", "Charlie"},
                };

            var content = new WwwFormUrlEncodedContent(dictionary);

            Assert.That(content.MediaType, Is.EqualTo("application/x-www-form-urlencoded"));

            using (var reader = new StreamReader(content.AsStream()))
            {
                var contentString = reader.ReadToEnd();
                Assert.That(contentString, Is.EqualTo("Value1=Alpha&Value2=Bravo&Value3=Charlie"));
            }
        }

        [Test]
        public void CanConstructContentFromDictionaryWithCharactersNeedingEscaping()
        {
            var dictionary = new Dictionary<string, string>
                {
                    {"Value1", "Al&pha"},
                    {"Value2", "Bra<vo Beta"},
                    {"Value3", "Char\"lie"},
                };

            var content = new WwwFormUrlEncodedContent(dictionary);

            Assert.That(content.MediaType, Is.EqualTo("application/x-www-form-urlencoded"));

            using (var reader = new StreamReader(content.AsStream()))
            {
                var contentString = reader.ReadToEnd();
                Assert.That(contentString, Is.EqualTo("Value1=Al%26pha&Value2=Bra%3cvo+Beta&Value3=Char%22lie"));
            }
        }

        [Test]
        public void CanConstructContentFromObjectProperties()
        {
            var objectSource = new
                {
                    Value1="Alpha",
                    Value2="Bravo",
                    Value3="Charlie",
                };

            var content = new WwwFormUrlEncodedContent(objectSource);

            Assert.That(content.MediaType, Is.EqualTo("application/x-www-form-urlencoded"));

            using (var reader = new StreamReader(content.AsStream()))
            {
                var contentString = reader.ReadToEnd();
                Assert.That(contentString, Is.EqualTo("Value1=Alpha&Value2=Bravo&Value3=Charlie"));
            }
        }
    }
}
