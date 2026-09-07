using System.Collections.ObjectModel;
using BDBlio.Core.Models;
using BDBlio.Data;
using BDBlio.Data.Repositories;
using BDBlio.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace BDBlio.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ComicBookRepository _repository;
    private readonly IImportExportService _importExportService;

    [ObservableProperty]
    private string? statusMessage;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private ObservableCollection<string> availableColumns = new();

    [ObservableProperty]
    private Dictionary<string, string> columnMapping = new();

    [ObservableProperty]
    private string? selectedFileType = "CSV";

    public SettingsViewModel()
    {
        var context = new BDBlioDatabaseContext();
        _repository = new ComicBookRepository(context);
        _importExportService = new ImportExportService();
    }

    [RelayCommand]
    public async Task ExportLibrary()
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV Files|*.csv|Excel Files|*.xlsx",
            DefaultExt = ".csv"
        };

        if (saveDialog.ShowDialog() != true)
            return;

        IsLoading = true;
        StatusMessage = "Exportation en cours...";

        try
        {
            var books = await _repository.GetAllAsync();

            if (saveDialog.FileName.EndsWith(".xlsx"))
                await _importExportService.ExportToExcelAsync(books, saveDialog.FileName);
            else
                await _importExportService.ExportToCsvAsync(books, saveDialog.FileName);

            StatusMessage = "✓ Exportation réussie";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Erreur : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ImportLibrary()
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "CSV Files|*.csv|Excel Files|*.xlsx",
            DefaultExt = ".csv"
        };

        if (openDialog.ShowDialog() != true)
            return;

        IsLoading = true;
        StatusMessage = "Préparation de l'importation...";

        try
        {
            var filePath = openDialog.FileName;
            var columns = openDialog.FileName.EndsWith(".xlsx")
                ? _importExportService.GetExcelColumns(filePath)
                : _importExportService.GetCsvColumns(filePath);

            AvailableColumns.Clear();
            foreach (var col in columns)
                AvailableColumns.Add(col);

            StatusMessage = "Configurez le mappage des colonnes";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Erreur : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ConfirmImport(string filePath)
    {
        if (ColumnMapping.Count == 0)
        {
            StatusMessage = "Veuillez configurer le mappage des colonnes";
            return;
        }

        IsLoading = true;
        StatusMessage = "Importation en cours...";

        try
        {
            List<ComicBook> books;

            if (filePath.EndsWith(".xlsx"))
                books = await _importExportService.ImportFromExcelAsync(filePath, ColumnMapping);
            else
                books = await _importExportService.ImportFromCsvAsync(filePath, ColumnMapping);

            int addedCount = 0;
            foreach (var book in books)
            {
                var existing = await _repository.GetByISBNAsync(book.ISBN ?? "");
                if (existing == null)
                {
                    await _repository.AddAsync(book);
                    addedCount++;
                }
            }

            StatusMessage = $"✓ {addedCount} BD importée(s)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Erreur : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
