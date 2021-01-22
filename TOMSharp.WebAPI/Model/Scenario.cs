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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TOMSharp_Loader.Model
{
    public class Scenario
    {
        [Key]
        public int? Id { get; set; }
        public string FinanceYear { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Created { get; set; }
        [Column(TypeName = "date")]
        public DateTime? Updated { get; set; }
        public List<ScenarioForecast> ScenarioForecasts { get; set; }
        public List<ScenarioProject> ScenarioProjects { get; set; }
    }

    public class ScenarioDto
    {
        public int? Id { get; set; }
        public string FinanceYear { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public List<Forecast> Forecasts { get; set; }
        public List<string> Projects { get; set; }
    }
    public class ScenarioForecast
    {
        public int? ScenarioId { get; set; }
        public Scenario Scenario { get; set; }
        public int? ForecastId { get; set; }
        public Forecast Forecast { get; set; }
        public int Priority { get; set; }
        
    }

    public class ScenarioProject
    {
        [Key]
        public int Id { get; set; }
        public int? ScenarioId { get; set; }
        public Scenario Scenario { get; set; }
        public string Project { get; set; }
    }
    
}