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

namespace TOMSharp_Loader.Model
{
    public class TimeBookingSummary
    {
        public int SprintId { get; set; }
        public string Sprint { get; set; }
        public string Team { get; set; }
        public string Person { get; set; }
        public decimal Hours { get; set; }
        public string FinanceYear { get; set; }
    }
}
