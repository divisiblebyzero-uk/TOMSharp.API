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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

namespace TOMSharp_Loader.Service
{

    public class PivotOptions
    {
        public string PivotName { get; set; }
        public string[] PivotRows { get; set; }
        public string[] PivotColumns { get; set; }
        public string[] PivotValues { get; set; }
    }
    
    public class SpreadsheetService
    {
        private readonly ILogger<SpreadsheetService> _logger;
        private readonly TogglContext _context;
        public const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public SpreadsheetService(ILogger<SpreadsheetService> logger, TogglContext context)
        {
            _logger = logger;
            _context = context;
        }

        private void AddPivot(PivotOptions pivotOptions, 
            XLWorkbook wb, IXLTable tbsTable)
        {
            if (pivotOptions.PivotRows != null || pivotOptions.PivotColumns != null || pivotOptions.PivotValues != null)
            {
                var ptSheet = wb.Worksheets.Add(pivotOptions.PivotName);

                var pt = ptSheet.PivotTables.Add(pivotOptions.PivotName, ptSheet.Cell(1, 1), tbsTable.AsRange());

                pt.SetShowEmptyItemsOnRows(false);
                
                if (pivotOptions.PivotRows != null)
                {
                    foreach (string row in pivotOptions.PivotRows)
                    {
                        pt.RowLabels.Add(row);
                    }
                }
                if (pivotOptions.PivotColumns != null)
                {
                    foreach (string col in pivotOptions.PivotColumns)
                    {
                        pt.ColumnLabels.Add(col);
                    }
                }

                if (pivotOptions.PivotValues != null)
                {
                    foreach (string val in pivotOptions.PivotValues)
                    {
                        if (val == "Charge")
                        {
                            pt.Values.Add(val).NumberFormat.Format = "£ #,##0";
                        }
                        else
                        {
                            pt.Values.Add(val).NumberFormat.Format = "#,##0";

                        }
                    }
                }

                ptSheet.Columns().Width = 12;
                ptSheet.Column(1).Width = 60;
                ptSheet.SetTabActive();
            }
        }
        
        public async Task<MemoryStream> GetGenericTableExcel(IEnumerable<object> rows,
            PivotOptions[] pivots = null)
        {
            return await Task.Run(() =>
            {
                var wb = new XLWorkbook();
                var ws = wb.AddWorksheet("Data");

                var tbsTable = ws.Cell(1, 1).InsertTable(rows.AsEnumerable(), "Data", true);
                ws.Columns().AdjustToContents();

                if (pivots != null)
                {
                    foreach (var pivot in pivots)
                    {
                        AddPivot(pivot, wb, tbsTable);
                    }
                }

                return GetWorkbookAsMemoryStream(wb);
            });
        }
       
        private MemoryStream GetWorkbookAsMemoryStream(XLWorkbook workbook)
        {
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public string GetFilename(Object controller, string parameters)
        {
            var dateString = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var controllerName = controller.GetType().Name.Replace("Controller", "");
            return parameters == null ? $"TOM-{controllerName}-{dateString}.xlsx" : $"TOM-{controllerName}-{parameters}-{dateString}.xlsx";
        }
    }

}
