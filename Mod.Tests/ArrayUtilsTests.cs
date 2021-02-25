using System.Collections.Generic;
using NUnit.Framework;
using ValheimEdits.Utils;

namespace ValheimEdits.Tests
{
    [TestFixture]
    public class ArrayUtilsTests
    {
        [Test]
        public void TestParseObjectToArray()
        {
            object test = new List<CommandParser.CommandSegment>
            {
                new(CommandParser.SegmentType.String, "my string"),
                new(CommandParser.SegmentType.Int, "42"),
                null,
                new(CommandParser.SegmentType.Identifier, "player")
            };

            Assert.AreEqual($"my string, 42, {ArrayUtils.NullString}, player",
                string.Join(", ", ArrayUtils.ToStringArray(test)));
        }
    }
}
