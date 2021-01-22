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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TOMSharp_Loader.Model
{
    public class TimeEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public long Pid { get; set; }
        public long Tid { get; set; }
        public long Uid { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime Updated { get; set; }
        public long Dur { get; set; }
        public string User { get; set; }
        public bool UseStop { get; set; }
        public string Client { get; set; }
        public string Project { get; set; }
        public string ProjectColor { get; set; }
        public string ProjectHexColor { get; set; }
        public string Task { get; set; }
        public decimal Billable { get; set; }
        public bool IsBillable { get; set; }
        public string Cur { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Pid)}: {Pid}, {nameof(Tid)}: {Tid}, {nameof(Uid)}: {Uid}, {nameof(Description)}: {Description}, {nameof(Start)}: {Start}, {nameof(End)}: {End}, {nameof(Updated)}: {Updated}, {nameof(Dur)}: {Dur}, {nameof(User)}: {User}, {nameof(UseStop)}: {UseStop}, {nameof(Client)}: {Client}, {nameof(Project)}: {Project}, {nameof(ProjectColor)}: {ProjectColor}, {nameof(ProjectHexColor)}: {ProjectHexColor}, {nameof(Task)}: {Task}, {nameof(Billable)}: {Billable}, {nameof(IsBillable)}: {IsBillable}, {nameof(Cur)}: {Cur}";
        }

        public void UpdateFrom(TimeEntry timeEntry)
        {
            Pid = timeEntry.Pid;
            Tid = timeEntry.Tid;
            Uid = timeEntry.Uid;
            Description = timeEntry.Description;
            Start = timeEntry.Start;
            End = timeEntry.End;
            Updated = timeEntry.Updated;
            Dur = timeEntry.Dur;
            User = timeEntry.User;
            UseStop = timeEntry.UseStop;
            Client = timeEntry.Client;
            Project = timeEntry.Project;
            ProjectColor = timeEntry.ProjectColor;
            ProjectHexColor = timeEntry.ProjectHexColor;
            Task = timeEntry.Task;
            Billable = timeEntry.Billable;
            IsBillable = timeEntry.IsBillable;
            Cur = timeEntry.Cur;
        }
    }
}
