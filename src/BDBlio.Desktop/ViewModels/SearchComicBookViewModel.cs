using System.Collections.ObjectModel;
using BDBlio.Core.Models;
using BDBlio.Data;
using BDBlio.Data.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BDBlio.Desktop.ViewModels;

public partial class SearchComicBookViewModel : ObservableObject
{
    private readonly ComicBookRepository _repository;

    [ObservableProperty]
    private string? searchQuery;

    [ObservableProperty]
    private string? barcode;

    [ObservableProperty]
    private ObservableCollection<ComicBook> searchResults = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? statusMessage;

    public SearchComicBookViewModel()
    {
        var context = new BDBlioDatabaseContext();
        _repository = new ComicBookRepository(context);
    }

    [RelayCommand]
    public async Task SearchByBarcode()
    {
        if (string.IsNullOrWhiteSpace(Barcode))
            return;

        IsLoading = true;
        StatusMessage = null;

        try
        {
            ComicBook? book = null;

            if (Barcode.Length == 13)
                book = await _repository.GetByEANAsync(Barcode);
            else if (Barcode.Length == 10 || Barcode.Length == 13)
                book = await _repository.GetByISBNAsync(Barcode);

            SearchResults.Clear();
            if (book != null)
            {
                SearchResults.Add(book);
                StatusMessage = "✓ BD trouvée dans votre bibliothèque";
            }
            else
            {
                StatusMessage = "✗ BD non trouvée";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erreur : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SearchByQuery()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        IsLoading = true;
        StatusMessage = null;

        try
        {
            var results = await _repository.SearchAsync(SearchQuery, null, null);
            SearchResults.Clear();

            foreach (var result in results)
                SearchResults.Add(result);

            StatusMessage = results.Count > 0
                ? $"✓ {results.Count} BD trouvée(s)"
                : "✗ Aucun résultat";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erreur : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
