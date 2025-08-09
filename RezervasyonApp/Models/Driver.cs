using System.ComponentModel.DataAnnotations;

namespace RezervasyonApp.Models;

public class Driver
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string FullName { get; set; } = default!;

    [Phone, StringLength(20)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;
}
