//using ClosedXML.Excel;
//using DocumentFormat.OpenXml.Packaging;
//using Microsoft.AspNetCore.Mvc;
//using ReportAutomationApp.Models;
//using System.Data;

//namespace ReportAutomationApp.Controllers
//{
//    public class DashboardController : Controller
//    {
//        private static DataTable _excelData;

//        public IActionResult Index()
//        {
//            return View(new ExcelUploadViewModel());
//        }

//        [HttpPost]
//        public IActionResult UploadExcel(ExcelUploadViewModel model)
//        {
//            if (model.ExcelFile != null)
//            {
//                using (var stream = new MemoryStream())
//                {
//                    model.ExcelFile.CopyTo(stream);

//                    using (var workbook = new XLWorkbook(stream))
//                    {
//                        var worksheet = workbook.Worksheet(1);
//                        _excelData = new DataTable();

//                        foreach (var row in worksheet.RowsUsed())
//                        {
//                            if (row.RowNumber() == 1)
//                            {
//                                foreach (var cell in row.Cells())
//                                {
//                                    _excelData.Columns.Add(cell.Value.ToString());
//                                }
//                            }
//                            else
//                            {
//                                var dataRow = _excelData.NewRow();
//                                for (int i = 0; i < _excelData.Columns.Count; i++)
//                                {
//                                    dataRow[i] = row.Cell(i + 1).Value.ToString();
//                                }
//                                _excelData.Rows.Add(dataRow);
//                            }
//                        }
//                    }
//                }

//                model.Columns = _excelData.Columns
//                    .Cast<DataColumn>()
//                    .Select(c => c.ColumnName)
//                    .ToList();
//            }

//            return View("Index", model);
//        }

//        [HttpPost]
//        public IActionResult GenerateChart(ExcelUploadViewModel model)
//        {
//            if (_excelData == null)
//                return RedirectToAction("Index");

//            if (string.IsNullOrEmpty(model.SelectedXColumn) ||
//                model.SelectedYColumns == null ||
//                !model.SelectedYColumns.Any())
//                return RedirectToAction("Index");

//            var groupColumn = model.SelectedXColumn;
//            var valueColumn = model.SelectedYColumns.First();

//            var filteredData = _excelData.AsEnumerable();

//            // Apply filter
//            if (model.SelectedGroupNames != null && model.SelectedGroupNames.Any())
//            {
//                filteredData = filteredData
//                    .Where(r => model.SelectedGroupNames.Contains(r[groupColumn].ToString()));
//            }

//            var groupedData = filteredData
//                .GroupBy(r => r[groupColumn].ToString())
//                .Select(g => new
//                {
//                    Name = g.Key,
//                    Total = g.Sum(r =>
//                    {
//                        double val;
//                        return double.TryParse(r[valueColumn].ToString(), out val) ? val : 0;
//                    })
//                })
//                .ToList();

//            var labels = groupedData.Select(x => x.Name).ToList();
//            var totals = groupedData.Select(x => x.Total).ToList();

//            ViewBag.XValues = labels;
//            ViewBag.YValues = totals;
//            ViewBag.ChartType = model.ChartType;

//            // Restore Columns
//            model.Columns = _excelData.Columns
//                .Cast<DataColumn>()
//                .Select(c => c.ColumnName)
//                .ToList();

//            // Restore Group Names
//            model.GroupNames = _excelData.AsEnumerable()
//                .Select(r => r[groupColumn].ToString())
//                .Distinct()
//                .ToList();

//            return View("Index", model);
//        }



//        [HttpPost]
//        public IActionResult ExportToPPT(string chartImageBase64)
//        {
//            if (string.IsNullOrEmpty(chartImageBase64))
//                return RedirectToAction("Index");

//            var base64Data = chartImageBase64.Replace("data:image/png;base64,", "");
//            byte[] imageBytes = Convert.FromBase64String(base64Data);

//            using (var memoryStream = new MemoryStream())
//            {
//                using (var presentation =
//                    PresentationDocument.Create(memoryStream,
//                        DocumentFormat.OpenXml.PresentationDocumentType.Presentation))
//                {
//                    var presentationPart = presentation.AddPresentationPart();
//                    presentationPart.Presentation = new DocumentFormat.OpenXml.Presentation.Presentation();

//                    // 🔥 Add SlideMaster
//                    var slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>();
//                    slideMasterPart.SlideMaster =
//                        new DocumentFormat.OpenXml.Presentation.SlideMaster(
//                            new DocumentFormat.OpenXml.Presentation.CommonSlideData(
//                                new DocumentFormat.OpenXml.Presentation.ShapeTree()));

//                    var slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>();
//                    slideLayoutPart.SlideLayout =
//                        new DocumentFormat.OpenXml.Presentation.SlideLayout(
//                            new DocumentFormat.OpenXml.Presentation.CommonSlideData(
//                                new DocumentFormat.OpenXml.Presentation.ShapeTree()));

//                    slideMasterPart.SlideMaster.AppendChild(
//                        new DocumentFormat.OpenXml.Presentation.SlideLayoutIdList(
//                            new DocumentFormat.OpenXml.Presentation.SlideLayoutId()
//                            {
//                                Id = 1U,
//                                RelationshipId = slideMasterPart.GetIdOfPart(slideLayoutPart)
//                            }));

//                    presentationPart.Presentation.AppendChild(
//                        new DocumentFormat.OpenXml.Presentation.SlideMasterIdList(
//                            new DocumentFormat.OpenXml.Presentation.SlideMasterId()
//                            {
//                                Id = 2147483648U,
//                                RelationshipId = presentationPart.GetIdOfPart(slideMasterPart)
//                            }));

//                    // 🔥 Create Slide
//                    var slidePart = presentationPart.AddNewPart<SlidePart>();
//                    slidePart.Slide =
//                        new DocumentFormat.OpenXml.Presentation.Slide(
//                            new DocumentFormat.OpenXml.Presentation.CommonSlideData(
//                                new DocumentFormat.OpenXml.Presentation.ShapeTree()));

//                    slidePart.AddPart(slideLayoutPart);

//                    // 🔥 Add Image
//                    var imagePart = slidePart.AddImagePart(ImagePartType.Png);
//                    using (var imgStream = new MemoryStream(imageBytes))
//                    {
//                        imagePart.FeedData(imgStream);
//                    }

//                    var tree = slidePart.Slide.CommonSlideData.ShapeTree;

//                    var picture = new DocumentFormat.OpenXml.Presentation.Picture(
//                        new DocumentFormat.OpenXml.Presentation.NonVisualPictureProperties(
//                            new DocumentFormat.OpenXml.Presentation.NonVisualDrawingProperties()
//                            {
//                                Id = 4U,
//                                Name = "Chart Image"
//                            },
//                            new DocumentFormat.OpenXml.Presentation.NonVisualPictureDrawingProperties(),
//                            new DocumentFormat.OpenXml.Presentation.ApplicationNonVisualDrawingProperties()),
//                        new DocumentFormat.OpenXml.Presentation.BlipFill(
//                            new DocumentFormat.OpenXml.Drawing.Blip()
//                            {
//                                Embed = slidePart.GetIdOfPart(imagePart)
//                            },
//                            new DocumentFormat.OpenXml.Drawing.Stretch(
//                                new DocumentFormat.OpenXml.Drawing.FillRectangle())),
//                        new DocumentFormat.OpenXml.Presentation.ShapeProperties(
//                            new DocumentFormat.OpenXml.Drawing.Transform2D(
//                                new DocumentFormat.OpenXml.Drawing.Offset() { X = 0, Y = 0 },
//                                new DocumentFormat.OpenXml.Drawing.Extents()
//                                {
//                                    Cx = 8000000,
//                                    Cy = 5000000
//                                }),
//                            new DocumentFormat.OpenXml.Drawing.PresetGeometry(
//                                new DocumentFormat.OpenXml.Drawing.AdjustValueList())
//                            { Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle }));

//                    tree.AppendChild(picture);

//                    presentationPart.Presentation.AppendChild(
//                        new DocumentFormat.OpenXml.Presentation.SlideIdList(
//                            new DocumentFormat.OpenXml.Presentation.SlideId()
//                            {
//                                Id = 256U,
//                                RelationshipId = presentationPart.GetIdOfPart(slidePart)
//                            }));

//                    presentationPart.Presentation.Save();
//                }

//                return File(memoryStream.ToArray(),
//                    "application/vnd.openxmlformats-officedocument.presentationml.presentation",
//                    "ChartReport.pptx");
//            }
//        }
//    }
//}