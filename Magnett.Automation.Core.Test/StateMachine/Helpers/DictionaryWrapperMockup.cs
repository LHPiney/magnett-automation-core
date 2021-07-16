using System.Collections.Generic;
using Magnett.Automation.Core.StateMachine.Collections;

namespace Magnett.Automation.Core.Test.StateMachine.Helpers
{
    public class DictionaryWrapperMockup : DictionaryWrapper<string>
    {
        private DictionaryWrapperMockup(IDictionary<string, string> values)
            : base(values)
        {}
        
        public static DictionaryWrapperMockup Create(IDictionary<string, string> values)
        {
            return new(values);
        }
    }
}