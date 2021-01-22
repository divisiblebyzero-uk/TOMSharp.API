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
using System.Linq;

namespace TOMSharp_Loader.Service
{
    public class ProjectActivityDeterminer
    {
        public static readonly string[] AllowableProjectActivities = 
        {
            "Research", "Development", "Prod Support", "Small Change", "Other", "Data Migration"
        };

        private const string DefaultProjectActivity = "Other";

        private static readonly string[] Delimiters =
        {
            "/", "-", ":", "_"
        };
        
        public string DetermineProjectActivity(string projectName)
        {
            if (projectName == null)
            {
                return DefaultProjectActivity;
            }

            var split = projectName.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 0)
            {
                return DefaultProjectActivity;
            }

            var lastEntry = split.Last();
            if (AllowableProjectActivities.Contains(lastEntry))
            {
                return lastEntry;
            }

            foreach (var activity in AllowableProjectActivities)
            {
                if (projectName.Contains(activity))
                {
                    return activity;
                }
            }

            if (projectName.Contains("Production Support"))
            {
                return "Prod Support";
            }
            return DefaultProjectActivity;
        }
    }
}