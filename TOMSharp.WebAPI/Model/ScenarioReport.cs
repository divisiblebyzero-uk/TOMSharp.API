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

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TOMSharp_Loader.Model
{
    public class ScenarioReport
    {
        public int? ScenarioId { get; set; }
        public string ForecastName { get; set; }
        public string Project { get; set; }
        public string FinanceMonth { get; set; }
        public string FinanceYear { get; set; }
        public decimal Hours { get; set; }
        [Column(TypeName = "date")]
        public DateTime MonthStartDate { get; set; }

        public string Sprint { get; set; }
        public int SprintId { get; set; }

    }
}