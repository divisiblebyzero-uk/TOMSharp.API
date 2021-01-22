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
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using TOMSharp_Loader.Model;

namespace TOMSharp_Loader.Service
{
    public class PeopleSheetImporter
    {
        private readonly TogglContext _togglContext;
        private readonly ILogger _logger;
        public PeopleSheetImporter(TogglContext togglContext, ILogger<PeopleSheetImporter> logger)
        {
            _togglContext = togglContext;
            _logger = logger;
        }
        private void AddOrUpdatePerson(Person p)
        {
            Person savedPerson = _togglContext.People.FirstOrDefault(person => person.Name == p.Name);
            if (savedPerson == null)
            {
                _togglContext.People.Add(p);
            }
            else
            {
                savedPerson.Name = p.Name;
                savedPerson.EmailAddress = p.EmailAddress;
                savedPerson.EndDate = p.EndDate;
                savedPerson.ResourceType = p.ResourceType;
                savedPerson.StartDate = p.StartDate;
                savedPerson.Team = p.Team;
                _togglContext.Update(savedPerson);
            }
            _togglContext.SaveChanges();
        }

        public int ImportPeopleSheet()
        {
            _logger.LogInformation("Starting to import people");
            using var reader = new TextFieldParser(@".\SamplePeople.txt");
            reader.TextFieldType = FieldType.Delimited;
            reader.Delimiters = new string[] { "\t" };
            reader.ReadLine(); // Skip over header row
            while (!reader.EndOfData)
            {
                try
                {
                    var r = reader.ReadFields();
                    _logger.LogInformation($"Building person: {r[0]}");
                    Person p = new Person();
                    p.Name = r[0];
                    p.Team = r[1];
                    p.ResourceType = r[2];
                    p.EmailAddress = r[3];
                    if (r[4] != null && r[4] != "")
                        p.StartDate = DateTime.Parse(r[4]);
                    if (r[5] != null && r[5] != "")
                        p.EndDate = DateTime.Parse(r[5]);
                    AddOrUpdatePerson(p);

                }
                catch (MalformedLineException ex)
                {
                    Console.WriteLine($"Error reading data: {ex.Message}");
                    return -1;
                }
            }

            return 0;
        }
        
    }
}
