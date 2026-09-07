using BDBlio.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BDBlio.Data.Repositories;

public class ComicBookRepository
{
    private readonly BDBlioDatabaseContext _context;

    public ComicBookRepository(BDBlioDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<ComicBook>> GetAllAsync()
    {
        return await _context.ComicBooks
            .Include(c => c.Ratings)
            .ToListAsync();
    }

    public async Task<ComicBook?> GetByIdAsync(int id)
    {
        return await _context.ComicBooks
            .Include(c => c.Ratings)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ComicBook?> GetByISBNAsync(string isbn)
    {
        return await _context.ComicBooks
            .FirstOrDefaultAsync(c => c.ISBN == isbn);
    }

    public async Task<ComicBook?> GetByEANAsync(string ean)
    {
        return await _context.ComicBooks
            .FirstOrDefaultAsync(c => c.EAN == ean);
    }

    public async Task<List<ComicBook>> SearchAsync(string? title, string? author, string? collection)
    {
        var query = _context.ComicBooks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(c => c.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(c => c.Author != null && c.Author.Contains(author));

        if (!string.IsNullOrWhiteSpace(collection))
            query = query.Where(c => c.Collection != null && c.Collection.Contains(collection));

        return await query.ToListAsync();
    }

    public async Task<ComicBook> AddAsync(ComicBook comicBook)
    {
        _context.ComicBooks.Add(comicBook);
        await _context.SaveChangesAsync();
        return comicBook;
    }

    public async Task UpdateAsync(ComicBook comicBook)
    {
        comicBook.UpdatedAt = DateTime.UtcNow;
        _context.ComicBooks.Update(comicBook);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var comicBook = await GetByIdAsync(id);
        if (comicBook != null)
        {
            _context.ComicBooks.Remove(comicBook);
            await _context.SaveChangesAsync();
        }
    }
}
