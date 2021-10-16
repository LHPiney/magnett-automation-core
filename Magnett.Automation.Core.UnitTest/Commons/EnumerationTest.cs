using Magnett.Automation.Core.UnitTest.Commons.Helpers;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Commons
{
    public class EnumerationTest
    {
        private const int Id = 1;
        private const string Name = "Name";

        [Fact]
        public void Create_When_Data_Is_Valid_Return_Instance()
        {
            var enumeration = new EnumerationMockup(Id, Name);
            
            Assert.NotNull(enumeration);
        }

        [Fact]
        public void Create_When_Instance_Created_Id_Is_Stored()
        {
            var enumeration = new EnumerationMockup(Id, Name);
            
            Assert.Equal(Id, enumeration.GetId());
        }
        
        [Fact]
        public void Create_When_Instance_Created_Name_Is_Stored()
        {
            var enumeration = new EnumerationMockup(Id, Name);
            
            Assert.Equal(Name, enumeration.Name);
        }

    }
}