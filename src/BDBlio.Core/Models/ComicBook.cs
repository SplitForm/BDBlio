using System.ComponentModel.DataAnnotations;

namespace BDBlio.Core.Models;

public class ComicBook
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Author { get; set; }

    [MaxLength(255)]
    public string? Publisher { get; set; }

    [MaxLength(50)]
    public string? Series { get; set; }

    [MaxLength(50)]
    public string? Collection { get; set; }

    public int? Volume { get; set; }

    [MaxLength(13)]
    public string? ISBN { get; set; }

    [MaxLength(13)]
    public string? EAN { get; set; }

    public DateTime? PublicationDate { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string? CoverImageUrl { get; set; }

    public decimal? Price { get; set; }

    [MaxLength(50)]
    public string? Language { get; set; }

    public int? PageCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation property
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
