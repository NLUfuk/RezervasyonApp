using System.ComponentModel.DataAnnotations;

namespace RezervasyonApp.Models;

public class Vehicle
{
    public int Id { get; set; }

    [Required]
    public int DriverId { get; set; }      // FK: aracı kim kullanıyor
    public Driver? Driver { get; set; }    // navigation

    [Required, StringLength(20)]
    public string Plate { get; set; } = default!;   // 20 ABC 123

    [Range(1, 100)]
    public int Capacity { get; set; } = 20;         // ileride koltuklarla eşleriz
}
