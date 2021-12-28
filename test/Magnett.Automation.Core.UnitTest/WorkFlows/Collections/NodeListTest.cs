using Magnett.Automation.Core.WorkFlows.Collections;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Collections
{
    public class NodeListTest
    {
        [Fact]
        public void Create_OnInvoke_ReturnNodeListInstance()
        {
            var instance = NodeList.Create();

            Assert.IsType<NodeList>(instance);
        }
    }
}