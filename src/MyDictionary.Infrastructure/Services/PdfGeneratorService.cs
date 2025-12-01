using MyDictionary.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MyDictionary.Infrastructure.Services;

public class PdfGeneratorService
{
    public byte[] GenerateWordListPdf(IEnumerable<Word> words, string title)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header()
                    .Background(Color.FromHex("#bfdbfe"))
                    .Padding(20)
                    .Column(column =>
                    {
                        column.Item().Text("myDictionary - قاموسي")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Color.FromHex("#1e40af"));
                        
                        column.Item().Text(title)
                            .FontSize(14)
                            .FontColor(Color.FromHex("#374151"));
                        
                        column.Item().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}")
                            .FontSize(9)
                            .FontColor(Color.FromHex("#9ca3af"));
                        
                        column.Item().Text($"Total Words: {words.Count()}")
                            .FontSize(11)
                            .Bold();
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Table(table =>
                    {
                        // Define columns
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // #
                            columns.RelativeColumn(2);   // Word
                            columns.RelativeColumn(2);   // Translation
                            columns.RelativeColumn(1);   // POS
                            columns.RelativeColumn(1);   // Difficulty
                            columns.RelativeColumn(1.5f); // Next Review
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Color.FromHex("#2563eb")).Padding(5).Text("#").Bold().FontColor(Colors.White);
                            header.Cell().Background(Color.FromHex("#2563eb")).Padding(5).Text("Word").Bold().FontColor(Colors.White);
                            header.Cell().Background(Color.FromHex("#2563eb")).Padding(5).Text("Translation").Bold().FontColor(Colors.White);
                            header.Cell().Background(Color.FromHex("#2563eb")).Padding(5).Text("Type").Bold().FontColor(Colors.White);
                            header.Cell().Background(Color.FromHex("#2563eb")).Padding(5).Text("Difficulty").Bold().FontColor(Colors.White);
                            header.Cell().Background(Color.FromHex("#2563eb")).Padding(5).Text("Next Review").Bold().FontColor(Colors.White);
                        });

                        // Rows
                        int index = 1;
                        foreach (var word in words)
                        {
                            var bgColor = index % 2 == 0 ? Color.FromHex("#f3f4f6") : Colors.White;
                            
                            table.Cell().Background(bgColor).Padding(5).Text(index.ToString());
                            table.Cell().Background(bgColor).Padding(5).Text(word.WordText);
                            table.Cell().Background(bgColor).Padding(5).Text(word.Translation);
                            table.Cell().Background(bgColor).Padding(5).Text(GetPartOfSpeechShort(word.PartOfSpeech));
                            table.Cell().Background(bgColor).Padding(5).Text(word.Difficulty.ToString());
                            table.Cell().Background(bgColor).Padding(5).Text(word.NextReviewDate.ToString("yyyy-MM-dd"));
                            
                            index++;
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    private string GetPartOfSpeechShort(PartOfSpeech pos)
    {
        return pos switch
        {
            PartOfSpeech.Noun => "N",
            PartOfSpeech.Verb => "V",
            PartOfSpeech.Adjective => "Adj",
            PartOfSpeech.Adverb => "Adv",
            PartOfSpeech.Pronoun => "Pron",
            PartOfSpeech.Preposition => "Prep",
            PartOfSpeech.Conjunction => "Conj",
            PartOfSpeech.Interjection => "Interj",
            _ => "Other"
        };
    }
}
