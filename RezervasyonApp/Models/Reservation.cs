using System.ComponentModel.DataAnnotations;

namespace RezervasyonApp.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Resource { get; set; } = "GENEL"; // oda-1, arac-2 vs.

    [Required, StringLength(80)]
    public string FullName { get; set; } = default!;

    [Phone, StringLength(20)]
    public string? Phone { get; set; }

    [Required, DataType(DataType.DateTime)]
    public DateTime StartAt { get; set; }

    [Range(15, 480)] // dakika cinsinden
    public int DurationMinutes { get; set; } = 60;

    [StringLength(200)]
    public string? Notes { get; set; }
}
