using NUnit.Framework;
using ValheimEdits.Serialization;

namespace ValheimEdits.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void TestLoadAndSave()
        {
            Config.Delete();
            try
            {
                Config.Load();
                Assert.IsTrue(Config.Instance.WorkbenchRequiresRoof);
                Config.Instance.WorkbenchRequiresRoof = false;
                Assert.IsFalse(Config.Instance.WorkbenchRequiresRoof);
                Config.Save();
                Assert.IsFalse(Config.Instance.WorkbenchRequiresRoof);
                Config.Instance.WorkbenchRequiresRoof = true;
                Config.Load();
                Assert.IsFalse(Config.Instance.WorkbenchRequiresRoof);
            }
            finally
            {
                Config.Delete();
            }
        }
    }
}
