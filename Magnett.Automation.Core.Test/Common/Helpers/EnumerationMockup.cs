using Magnett.Automation.Core.Common;

namespace Magnett.Automation.Core.Test.Common.Helpers
{
    public class EnumerationMockup : Enumeration
    {
        public int GetId()
        {
            return Id;
        }
        public EnumerationMockup(int id, string name) :
            base(id, name)
        {
        }
    }
}