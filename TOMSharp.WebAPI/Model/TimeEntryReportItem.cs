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
    public class TimeEntryReportItem
    {
        #nullable enable
        public int? BudgetLinesId { get; set; }
        public string? BudgetLinesName { get; set; }
        public string? BudgetLinesFinanceYear { get; set; }
        public decimal? BudgetLinesBudget { get; set; }
        public int? PeopleId { get; set; }
        public string? PeopleName { get; set; }
        public string? PeopleEmailAddress { get; set; }
        [Column(TypeName = "date")]
        public DateTime? PeopleStartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? PeopleEndDate { get; set; }
        public string? PeopleResourceType { get; set; }
        public string? PeopleTeam { get; set; }
        public string? PeopleEmployer { get; set; }
        public int? ProjectsId { get; set; }
        public string? ProjectsName { get; set; }
        public string? ProjectsTimeEntryName { get; set; }
        public int? ProjectsBudgetLineId { get; set; }
        public string? ProjectsFinanceWIPCode { get; set; }
        public string? ProjectsType { get; set; }
        public string? ProjectsFinanceName { get; set; }
        public string? ProjectsActivity { get; set; }
        public int? SprintsId { get; set; }
        public string? SprintsName { get; set; }
        [Column(TypeName = "date")]
        public DateTime? SprintsStartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? SprintsEndDate { get; set; }
        public string? SprintsFinanceMonth { get; set; }
        public string? SprintsFinanceYear { get; set; }
        
        #nullable disable
        
        public long TimeEntriesId { get; set; }
        public long TimeEntriesPid { get; set; }
        public long TimeEntriesTid { get; set; }
        public long TimeEntriesUid { get; set; }
        public string TimeEntriesDescription { get; set; }
        [Column(TypeName = "date")]
        public DateTime TimeEntriesStart { get; set; }
        [Column(TypeName = "date")]
        public DateTime TimeEntriesEnd { get; set; }
        [Column(TypeName = "date")]
        public DateTime TimeEntriesUpdated { get; set; }
        public long TimeEntriesDur { get; set; }
        public string TimeEntriesUser { get; set; }
        public bool TimeEntriesUseStop { get; set; }
        public string TimeEntriesClient { get; set; }
        public string TimeEntriesProject { get; set; }
        public string TimeEntriesProjectColor { get; set; }
        public string TimeEntriesProjectHexColor { get; set; }
        public string TimeEntriesTask { get; set; }
        public decimal TimeEntriesBillable { get; set; }
        public bool TimeEntriesIsBillable { get; set; }
        public string TimeEntriesCur { get; set; }
        public decimal Hours { get; set; }
        public string Capitalised { get; set; }
        [NotMapped]
        public decimal Charge {
            get {
                return Utilities.CalculateCharge(Hours);
            }
        }
    }
}