using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using my_shop.App.Models;
using System;
using System.IO;
using System.Linq;

namespace my_shop.App.Services.PdfService
{
    public class OrderPdfService
    {
        public byte[] GenerateOrderInvoice(Order order)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .PaddingBottom(10)
                        .Column(column =>
                        {
                            // Header Section
                            column.Item()
                                .Background(Colors.Blue.Lighten3)
                                .Padding(15)
                                .Column(col =>
                                {
                                    col.Item().Text("INVOICE")
                                        .FontSize(28)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken2);

                                    col.Item().PaddingTop(5).Text($"Order #{order.Id}")
                                        .FontSize(14)
                                        .SemiBold();
                                });

                            // Order Info Section
                            column.Item().PaddingTop(15).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Order Information").Bold().FontSize(12);
                                    col.Item().PaddingTop(3).Text($"Date: {order.FormattedDate}").FontSize(10);
                                    col.Item().Text($"Status: {order.StatusDisplay}").FontSize(10);
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Customer Information").Bold().FontSize(12);
                                    col.Item().PaddingTop(3).Text($"Name: {order.Customer?.Name ?? "N/A"}").FontSize(10);
                                    col.Item().Text($"Phone: {order.Customer?.Phone ?? "N/A"}").FontSize(10);
                                });
                            });

                            // Divider
                            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        });

                    page.Content()
                        .PaddingTop(10)
                        .Column(column =>
                        {
                            // Items Table
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);  // #
                                    columns.ConstantColumn(70);  // SKU
                                    columns.RelativeColumn(3);   // Product Name
                                    columns.RelativeColumn(1.5f);   // Unit Price
                                    columns.ConstantColumn(40);   // Qty
                                    columns.RelativeColumn(1.5f);   // Total
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Lighten3)
                                        .Padding(5).Text("#").Bold().FontSize(10);
                                    header.Cell().Background(Colors.Blue.Lighten3)
                                        .Padding(5).Text("SKU").Bold().FontSize(10);
                                    header.Cell().Background(Colors.Blue.Lighten3)
                                        .Padding(5).Text("Product").Bold().FontSize(10);
                                    header.Cell().Background(Colors.Blue.Lighten3)
                                        .Padding(5).Text("Price").Bold().FontSize(10).AlignRight();
                                    header.Cell().Background(Colors.Blue.Lighten3)
                                        .Padding(5).Text("Qty").Bold().FontSize(10).AlignCenter();
                                    header.Cell().Background(Colors.Blue.Lighten3)
                                        .Padding(5).Text("Total").Bold().FontSize(10).AlignRight();
                                });

                                // Items
                                int index = 1;
                                if (order.OrderItems != null)
                                {
                                    foreach (var item in order.OrderItems)
                                    {
                                        var bgColor = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                                        table.Cell().Background(bgColor).Padding(5)
                                            .Text(index.ToString()).FontSize(10);
                                        table.Cell().Background(bgColor).Padding(5)
                                            .Text(item.Product?.Sku ?? "N/A").FontSize(9);
                                        table.Cell().Background(bgColor).Padding(5)
                                            .Text(item.Product?.Name ?? "N/A").FontSize(9);
                                        table.Cell().Background(bgColor).Padding(5)
                                            .Text(item.FormattedPrice).FontSize(9).AlignRight();
                                        table.Cell().Background(bgColor).Padding(5)
                                            .Text($"× {item.Quantity}").FontSize(9).AlignCenter();
                                        table.Cell().Background(bgColor).Padding(5)
                                            .Text(item.FormattedSubtotal).FontSize(9).AlignRight();

                                        index++;
                                    }
                                }
                            });

                            // Total Section
                            column.Item().PaddingTop(15).AlignRight().Column(col =>
                            {
                                col.Item()
                                    .Background(Colors.Blue.Lighten4)
                                    .Padding(12)
                                    .Width(200)
                                    .Row(row =>
                                    {
                                        row.RelativeItem().Text("TOTAL:").Bold().FontSize(12);
                                        row.RelativeItem().Text(order.FormattedPrice)
                                            .Bold().FontSize(12).FontColor(Colors.Blue.Darken2).AlignRight();
                                    });
                            });

                            // Footer note
                            column.Item().PaddingTop(20).Text("Thank you for your purchase!")
                                .FontSize(12).Italic().FontColor(Colors.Grey.Darken1);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ").FontSize(9);
                            text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold().FontSize(9);
                            text.Span(" | Page ").FontSize(9);
                            text.CurrentPageNumber().FontSize(9);
                        });
                });
            });

            return document.GeneratePdf();
        }

        public string SavePdfToTemp(byte[] pdfBytes, int orderId)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), "MyShop_Invoices");
            Directory.CreateDirectory(tempFolder);

            var fileName = $"Invoice_Order_{orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(tempFolder, fileName);

            File.WriteAllBytes(filePath, pdfBytes);
            return filePath;
        }
    }
}