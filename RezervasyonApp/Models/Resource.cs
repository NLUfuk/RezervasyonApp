using System.ComponentModel.DataAnnotations;

namespace RezervasyonApp.Models;

public class Resource
{
    public int Id { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = default!;
}
