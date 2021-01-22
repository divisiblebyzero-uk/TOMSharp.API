// Copyright (C) 2021 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using TOMSharp_Loader.Service;
using Xunit;

namespace Test.TOMSharp.WebAPI
{
    public class TestDetermineActivityName
    {
        private readonly ProjectActivityDeterminer _pad = new ProjectActivityDeterminer();
        [Theory]
        [InlineData("FY21/Some Project Or Other/Research", "Research")]
        [InlineData("FY21/Project/Production Support", "Prod Support")]
        [InlineData("Research the thing", "Research")]
        [InlineData("Development of the thing", "Development")]
        [InlineData("", "Other")]
        [InlineData(null, "Other")]
        [InlineData("//", "Other")]
        [InlineData("FY21/Do something but not sure what", "Other")]
        public void SimpleTest(string projectName, string expectedActivity)
        {
            var determinedActivity = _pad.DetermineProjectActivity(projectName);
            Assert.Equal(expectedActivity, determinedActivity);
        }
    }
}