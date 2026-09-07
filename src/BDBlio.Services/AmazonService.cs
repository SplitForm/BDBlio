using BDBlio.Core.Models;
using HtmlAgilityPack;
using RestSharp;

namespace BDBlio.Services;

public interface IAmazonService
{
    Task<ComicBook?> SearchByISBNAsync(string isbn);
    Task<ComicBook?> SearchByEANAsync(string ean);
    Task<List<ComicBook>> SearchByTitleAsync(string title);
    Task<List<ComicBook>> SearchByAuthorAsync(string author);
}

public class AmazonService : IAmazonService
{
    private readonly string _baseUrl = "https://www.amazon.fr";
    private readonly RestClient _client;

    public AmazonService()
    {
        var options = new RestClientOptions(_baseUrl)
        {
            ThrowOnAnyError = false,
            ThrowOnDeserializationError = false
        };
        _client = new RestClient(options);
    }

    public async Task<ComicBook?> SearchByISBNAsync(string isbn)
    {
        return await SearchOnAmazonAsync($"ISBN {isbn}");
    }

    public async Task<ComicBook?> SearchByEANAsync(string ean)
    {
        return await SearchOnAmazonAsync($"EAN {ean}");
    }

    public async Task<List<ComicBook>> SearchByTitleAsync(string title)
    {
        return await SearchMultipleAsync(title);
    }

    public async Task<List<ComicBook>> SearchByAuthorAsync(string author)
    {
        return await SearchMultipleAsync(author);
    }

    private async Task<ComicBook?> SearchOnAmazonAsync(string query)
    {
        try
        {
            var request = new RestRequest($"/s", Method.Get);
            request.AddParameter("k", query);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful || response.Content == null)
                return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(response.Content);

            // Parsing du premier résultat
            var firstResult = doc.DocumentNode.SelectSingleNode("//div[@data-component-type='s-search-result']");
            if (firstResult == null)
                return null;

            var title = firstResult.SelectSingleNode(".//h2//a//span")?.InnerText?.Trim() ?? "";
            var authorNode = firstResult.SelectSingleNode(".//span[contains(@class, 'a-color-secondary')]//a");
            var author = authorNode?.InnerText?.Trim();
            var priceNode = firstResult.SelectSingleNode(".//span[@class='a-price-whole']");
            var priceText = priceNode?.InnerText?.Trim();
            decimal.TryParse(priceText?.Replace("€", "").Replace(",", "."), out var price);

            return new ComicBook
            {
                Title = title,
                Author = author,
                Price = price > 0 ? price : null,
                Language = "Français"
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur Amazon Service: {ex.Message}");
            return null;
        }
    }

    private async Task<List<ComicBook>> SearchMultipleAsync(string query)
    {
        var results = new List<ComicBook>();
        try
        {
            var request = new RestRequest($"/s", Method.Get);
            request.AddParameter("k", query);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful || response.Content == null)
                return results;

            var doc = new HtmlDocument();
            doc.LoadHtml(response.Content);

            var searchResults = doc.DocumentNode.SelectNodes("//div[@data-component-type='s-search-result']");
            if (searchResults == null)
                return results;

            foreach (var result in searchResults.Take(10))
            {
                var title = result.SelectSingleNode(".//h2//a//span")?.InnerText?.Trim() ?? "";
                if (string.IsNullOrEmpty(title)) continue;

                var authorNode = result.SelectSingleNode(".//span[contains(@class, 'a-color-secondary')]//a");
                var author = authorNode?.InnerText?.Trim();
                var priceNode = result.SelectSingleNode(".//span[@class='a-price-whole']");
                var priceText = priceNode?.InnerText?.Trim();
                decimal.TryParse(priceText?.Replace("€", "").Replace(",", "."), out var price);

                results.Add(new ComicBook
                {
                    Title = title,
                    Author = author,
                    Price = price > 0 ? price : null,
                    Language = "Français"
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur recherche multiple: {ex.Message}");
        }

        return results;
    }
}
