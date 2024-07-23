using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public class Flavor
{
    public int Id { get; set; }
    public string? flavor { get; set; }
    public double Price { get; set; }
}
