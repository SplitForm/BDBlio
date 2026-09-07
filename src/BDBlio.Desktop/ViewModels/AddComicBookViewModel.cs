using System.Collections.ObjectModel;
using System.Windows.Input;
using BDBlio.Core.Models;
using BDBlio.Data;
using BDBlio.Data.Repositories;
using BDBlio.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BDBlio.Desktop.ViewModels;

public partial class AddComicBookViewModel : ObservableObject
{
    private readonly ComicBookRepository _repository;
    private readonly IAmazonService _amazonService;
    private readonly IBarcodeService _barcodeService;

    [ObservableProperty]
    private string? searchQuery;

    [ObservableProperty]
    private string? barcode;

    [ObservableProperty]
    private ObservableCollection<ComicBook> searchResults = new();

    [ObservableProperty]
    private ComicBook? selectedComicBook;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public AddComicBookViewModel()
    {
        var context = new BDBlioDatabaseContext();
        _repository = new ComicBookRepository(context);
        _amazonService = new AmazonService();
        _barcodeService = new BarcodeService();
    }

    [RelayCommand]
    public async Task SearchByBarcode()
    {
        if (string.IsNullOrWhiteSpace(Barcode))
            return;

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            ComicBook? book = null;

            if (Barcode.Length == 13)
                book = await _amazonService.SearchByEANAsync(Barcode);
            else if (Barcode.Length == 10 || Barcode.Length == 13)
                book = await _amazonService.SearchByISBNAsync(Barcode);

            if (book != null)
            {
                SelectedComicBook = book;
                SearchResults.Clear();
                SearchResults.Add(book);
            }
            else
            {
                ErrorMessage = "Aucun résultat trouvé pour ce code-barres";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de la recherche : {ex.Message}";
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
        ErrorMessage = null;

        try
        {
            var results = await _amazonService.SearchByTitleAsync(SearchQuery);
            SearchResults.Clear();

            foreach (var result in results)
                SearchResults.Add(result);

            if (results.Count == 0)
                ErrorMessage = "Aucun résultat trouvé";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de la recherche : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task AddSelectedBook()
    {
        if (SelectedComicBook == null)
            return;

        try
        {
            await _repository.AddAsync(SelectedComicBook);
            ErrorMessage = null;
            SearchQuery = null;
            Barcode = null;
            SearchResults.Clear();
            SelectedComicBook = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de l'ajout : {ex.Message}";
        }
    }
}
