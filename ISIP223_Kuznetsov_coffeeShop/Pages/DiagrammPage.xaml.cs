using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting; 
using Word = Microsoft.Office.Interop.Word;       
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms.Integration;

namespace ISIP223_Kuznetsov_coffeeShop.Pages
{
    public partial class DiagrammPage : Page
    {
        public DiagrammPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ChartPayments.ChartAreas.Add(new ChartArea("Main"));
            var currentSeries = new Series("Выручка")
            {
                IsValueShownAsLabel = true
            };
            ChartPayments.Series.Add(currentSeries);

            UpdateChart();
        }

        private void CmbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            if (ChartPayments == null || ChartPayments.Series.Count == 0)
                return;

            var currentSeries = ChartPayments.Series[0];
            currentSeries.Points.Clear();

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                if (CmbReportType.SelectedIndex == 0) 
                {
                    currentSeries.ChartType = SeriesChartType.Pie;

            
                    var data = db.Orders
                        .GroupBy(o => o.Cafes.Name)
                        .Select(g => new {
                            CafeName = g.Key,
                            Total = g.Sum(o => o.Quantity * o.Products.Cost)
                        }).ToList();

                    foreach (var item in data)
                    {
                        currentSeries.Points.AddXY(item.CafeName, item.Total);
                    }
                }
                else 
                {
                    currentSeries.ChartType = SeriesChartType.Column;

                    var data = db.Orders
                        .GroupBy(o => o.Products.Name)
                        .Select(g => new {
                            ProductName = g.Key,
                            Total = g.Sum(o => o.Quantity * o.Products.Cost)
                        }).ToList();

                    foreach (var item in data)
                    {
                        currentSeries.Points.AddXY(item.ProductName, item.Total);
                    }
                }
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = true; 
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            worksheet.Cells[1, 1] = "Название";
            worksheet.Cells[1, 2] = "Сумма (руб.)";
            worksheet.Cells[1, 1].Font.Bold = true;
            worksheet.Cells[1, 2].Font.Bold = true;

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                var data = db.Orders
                    .GroupBy(o => o.Cafes.Name)
                    .Select(g => new { Name = g.Key, Total = g.Sum(o => o.Quantity * o.Products.Cost) })
                    .ToList();

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1] = item.Name;
                    worksheet.Cells[row, 2] = item.Total;
                    row++;
                }
            }

            worksheet.Columns.AutoFit();
            MessageBox.Show("Отчет успешно выгружен в Excel!", "Успех");
        }

        private void BtnExportWord_Click(object sender, RoutedEventArgs e)
        {
            var application = new Word.Application();
            Word.Document document = application.Documents.Add();
            application.Visible = true;

            Word.Paragraph titleParagraph = document.Paragraphs.Add();
            Word.Range titleRange = titleParagraph.Range;
            titleRange.Text = "Отчет по выручке сети кофеен";
            titleParagraph.set_Style("Заголовок 1"); 
            titleRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            titleRange.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                var data = db.Orders
                    .GroupBy(o => o.Cafes.Name)
                    .Select(g => new { Name = g.Key, Total = g.Sum(o => o.Quantity * o.Products.Cost) })
                    .ToList();

                Word.Table table = document.Tables.Add(tableRange, data.Count + 1, 2);
                table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

                table.Cell(1, 1).Range.Text = "Кофейня";
                table.Cell(1, 2).Range.Text = "Выручка (руб.)";
                table.Rows[1].Range.Font.Bold = 1;

                for (int i = 0; i < data.Count; i++)
                {
                    table.Cell(i + 2, 1).Range.Text = data[i].Name;
                    table.Cell(i + 2, 2).Range.Text = data[i].Total.ToString("N2");
                }
            }

            string docPath = @"C:\Users\bosss\OneDrive\Desktop";
            string pdfPath = @"C:\Users\bosss\OneDrive\Desktop";

            try
            {
                document.SaveAs2(docPath);
                document.SaveAs2(pdfPath, Word.WdExportFormat.wdExportFormatPDF);
                MessageBox.Show($"Файлы сохранены на диск C:\n{docPath}\n{pdfPath}", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
    }
}