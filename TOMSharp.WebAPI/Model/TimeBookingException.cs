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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TOMSharp_Loader.Model
{
    public class TimeBookingException
    {
        private const decimal LowThreshold = 0.8m;
        private const decimal HighThreshold = 1.2m;
        public string Sprint { get; set; }
        public string Person { get; set; }
        public decimal ExpectedHours { get; set; }
        public decimal? ActualHours { get; set; }
        [NotMapped]
        public List<string> Errors
        {
            get
            {
                var errors = new List<string>();
                if (ActualHours == null)
                {
                    errors.Add("No time booked.");
                }

                if (ActualHours > ExpectedHours*HighThreshold)
                {
                    errors.Add("Too many hours booked.");
                }

                if (ActualHours < ExpectedHours*LowThreshold)
                {
                    errors.Add("Not enough hours booked");
                }

                return errors;
            }
        }

        [NotMapped] public bool IsCorrect => !Errors.Any();
    }
}
