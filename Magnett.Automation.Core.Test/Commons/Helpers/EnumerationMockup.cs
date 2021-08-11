using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.Test.Commons.Helpers
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