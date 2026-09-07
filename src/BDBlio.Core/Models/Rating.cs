using System.ComponentModel.DataAnnotations;

namespace BDBlio.Core.Models;

public class Rating
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ComicBookId { get; set; }

    public ComicBook? ComicBook { get; set; }

    [Range(0, 5)]
    public decimal Score { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
