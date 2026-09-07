using BDBlio.Core.Models;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using System.Globalization;

namespace BDBlio.Services;

public interface IImportExportService
{
    Task<List<ComicBook>> ImportFromCsvAsync(string filePath, Dictionary<string, string> columnMapping);
    Task<List<ComicBook>> ImportFromExcelAsync(string filePath, Dictionary<string, string> columnMapping);
    Task ExportToCsvAsync(List<ComicBook> comicBooks, string filePath);
    Task ExportToExcelAsync(List<ComicBook> comicBooks, string filePath);
    List<string> GetCsvColumns(string filePath);
    List<string> GetExcelColumns(string filePath);
}

public class ImportExportService : IImportExportService
{
    public async Task<List<ComicBook>> ImportFromCsvAsync(string filePath, Dictionary<string, string> columnMapping)
    {
        var comicBooks = new List<ComicBook>();

        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                var comicBook = MapRowToComicBook(csv, columnMapping);
                comicBooks.Add(comicBook);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur import CSV: {ex.Message}");
        }

        return await Task.FromResult(comicBooks);
    }

    public async Task<List<ComicBook>> ImportFromExcelAsync(string filePath, Dictionary<string, string> columnMapping)
    {
        var comicBooks = new List<ComicBook>();
        EPPlus.LicenseContext.LicenseType = EPPlus.LicenseType.Community;

        try
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var comicBook = new ComicBook();

                foreach (var mapping in columnMapping)
                {
                    var excelColumn = int.Parse(mapping.Key.Replace("Column", ""));
                    var value = worksheet.Cells[row, excelColumn].Value?.ToString();

                    if (string.IsNullOrEmpty(value))
                        continue;

                    MapPropertyValue(comicBook, mapping.Value, value);
                }

                comicBooks.Add(comicBook);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur import Excel: {ex.Message}");
        }

        return await Task.FromResult(comicBooks);
    }

    public async Task ExportToCsvAsync(List<ComicBook> comicBooks, string filePath)
    {
        try
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<ComicBook>();
            await csv.NextRecordAsync();

            foreach (var book in comicBooks)
            {
                csv.WriteRecord(book);
                await csv.NextRecordAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur export CSV: {ex.Message}");
        }
    }

    public async Task ExportToExcelAsync(List<ComicBook> comicBooks, string filePath)
    {
        EPPlus.LicenseContext.LicenseType = EPPlus.LicenseType.Community;

        try
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Bibliothèque");

            // Headers
            var headers = new[] { "Titre", "Auteur", "Éditeur", "Série", "Collection", "Volume", "ISBN", "EAN", "Date Publication", "Description", "Prix", "Langue", "Pages" };
            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cells[1, col].Value = headers[col - 1];
                worksheet.Cells[1, col].Style.Font.Bold = true;
                worksheet.Cells[1, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Data
            int row = 2;
            foreach (var book in comicBooks)
            {
                worksheet.Cells[row, 1].Value = book.Title;
                worksheet.Cells[row, 2].Value = book.Author;
                worksheet.Cells[row, 3].Value = book.Publisher;
                worksheet.Cells[row, 4].Value = book.Series;
                worksheet.Cells[row, 5].Value = book.Collection;
                worksheet.Cells[row, 6].Value = book.Volume;
                worksheet.Cells[row, 7].Value = book.ISBN;
                worksheet.Cells[row, 8].Value = book.EAN;
                worksheet.Cells[row, 9].Value = book.PublicationDate?.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 10].Value = book.Description;
                worksheet.Cells[row, 11].Value = book.Price;
                worksheet.Cells[row, 12].Value = book.Language;
                worksheet.Cells[row, 13].Value = book.PageCount;
                row++;
            }

            worksheet.Columns.AutoFit();

            await package.SaveAsAsync(new FileInfo(filePath));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur export Excel: {ex.Message}");
        }
    }

    public List<string> GetCsvColumns(string filePath)
    {
        var columns = new List<string>();
        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();
            columns = csv.HeaderRecord?.ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lecture colonnes CSV: {ex.Message}");
        }
        return columns;
    }

    public List<string> GetExcelColumns(string filePath)
    {
        var columns = new List<string>();
        EPPlus.LicenseContext.LicenseType = EPPlus.LicenseType.Community;

        try
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            for (int col = 1; col <= worksheet.Dimension?.End.Column; col++)
            {
                var value = worksheet.Cells[1, col].Value?.ToString() ?? $"Colonne {col}";
                columns.Add(value);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur lecture colonnes Excel: {ex.Message}");
        }
        return columns;
    }

    private ComicBook MapRowToComicBook(IReaderRow row, Dictionary<string, string> columnMapping)
    {
        var comicBook = new ComicBook();

        foreach (var mapping in columnMapping)
        {
            try
            {
                var value = row[mapping.Key];
                if (string.IsNullOrEmpty(value))
                    continue;

                MapPropertyValue(comicBook, mapping.Value, value);
            }
            catch
            {
                // Ignorer les colonnes non trouvées
            }
        }

        return comicBook;
    }

    private void MapPropertyValue(ComicBook comicBook, string propertyName, string value)
    {
        propertyName = propertyName.ToLower();

        switch (propertyName)
        {
            case "title" or "titre":
                comicBook.Title = value;
                break;
            case "author" or "auteur":
                comicBook.Author = value;
                break;
            case "publisher" or "éditeur":
                comicBook.Publisher = value;
                break;
            case "series" or "série":
                comicBook.Series = value;
                break;
            case "collection":
                comicBook.Collection = value;
                break;
            case "volume":
                if (int.TryParse(value, out var volume))
                    comicBook.Volume = volume;
                break;
            case "isbn":
                comicBook.ISBN = value;
                break;
            case "ean":
                comicBook.EAN = value;
                break;
            case "publicationdate" or "date publication":
                if (DateTime.TryParse(value, out var pubDate))
                    comicBook.PublicationDate = pubDate;
                break;
            case "description":
                comicBook.Description = value;
                break;
            case "price" or "prix":
                if (decimal.TryParse(value.Replace("€", "").Replace(",", "."), out var price))
                    comicBook.Price = price;
                break;
            case "language" or "langue":
                comicBook.Language = value;
                break;
            case "pagecount" or "pages":
                if (int.TryParse(value, out var pages))
                    comicBook.PageCount = pages;
                break;
            case "notes":
                comicBook.Notes = value;
                break;
        }
    }
}
