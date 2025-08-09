using System.ComponentModel.DataAnnotations;

namespace RezervasyonApp.Models;

public class Shift
{
    public int Id { get; set; }

    [Required]
    public int DriverId { get; set; }
    public Driver? Driver { get; set; }

    [Required]
    public int VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    [Required, DataType(DataType.DateTime)]
    public DateTime DepartureTime { get; set; }
}
