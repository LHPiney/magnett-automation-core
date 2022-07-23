using System.Collections.Generic;
using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.UnitTest.Commons.Helpers;

public class DictionaryWrapperMockup : DictionaryWrapper<string>
{
    private DictionaryWrapperMockup(IDictionary<CommonNamedKey, string> dictionary)
        : base(dictionary)
    {
    }

    private DictionaryWrapperMockup()
        : base()
    {
    }
        
    public static DictionaryWrapperMockup Create(IDictionary<CommonNamedKey, string> dictionary)
    {
        return new(dictionary);
    }
        
    public static DictionaryWrapperMockup Create()
    {
        return new();
    }
}