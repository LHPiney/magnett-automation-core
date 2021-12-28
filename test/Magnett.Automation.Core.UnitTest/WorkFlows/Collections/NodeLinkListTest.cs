using Magnett.Automation.Core.WorkFlows.Collections;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Collections
{
    public class NodeLinkListTest
    {
        [Fact]
        public void Create_OnInvoke_ReturnNodeLinkListInstance()
        {
            var instance = NodeLinkList.Create();

            Assert.IsType<NodeLinkList>(instance);
        }
    }
}