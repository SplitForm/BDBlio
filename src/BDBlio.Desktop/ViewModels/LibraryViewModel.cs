using System.Collections.ObjectModel;
using BDBlio.Core.Models;
using BDBlio.Data;
using BDBlio.Data.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BDBlio.Desktop.ViewModels;

public enum SortBy
{
    Title,
    Author,
    Collection
}

public partial class LibraryViewModel : ObservableObject
{
    private readonly ComicBookRepository _repository;
    private List<ComicBook> _allBooks = new();

    [ObservableProperty]
    private ObservableCollection<ComicBook> comicBooks = new();

    [ObservableProperty]
    private ComicBook? selectedBook;

    [ObservableProperty]
    private SortBy sortBy = SortBy.Title;

    [ObservableProperty]
    private int totalBooks;

    [ObservableProperty]
    private bool isLoading;

    public LibraryViewModel()
    {
        var context = new BDBlioDatabaseContext();
        _repository = new ComicBookRepository(context);
    }

    [RelayCommand]
    public async Task LoadBooks()
    {
        IsLoading = true;

        try
        {
            _allBooks = await _repository.GetAllAsync();
            RefreshDisplay();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void SortByTitle()
    {
        SortBy = SortBy.Title;
        RefreshDisplay();
    }

    [RelayCommand]
    public void SortByAuthor()
    {
        SortBy = SortBy.Author;
        RefreshDisplay();
    }

    [RelayCommand]
    public void SortByCollection()
    {
        SortBy = SortBy.Collection;
        RefreshDisplay();
    }

    [RelayCommand]
    public async Task DeleteBook(ComicBook book)
    {
        try
        {
            await _repository.DeleteAsync(book.Id);
            await LoadBooksCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur suppression : {ex.Message}");
        }
    }

    private void RefreshDisplay()
    {
        var sortedBooks = SortBy switch
        {
            SortBy.Title => _allBooks.OrderBy(b => b.Title).ToList(),
            SortBy.Author => _allBooks.OrderBy(b => b.Author ?? "").ToList(),
            SortBy.Collection => _allBooks.OrderBy(b => b.Collection ?? "").ToList(),
            _ => _allBooks
        };

        ComicBooks.Clear();
        foreach (var book in sortedBooks)
            ComicBooks.Add(book);

        TotalBooks = ComicBooks.Count;
    }
}
