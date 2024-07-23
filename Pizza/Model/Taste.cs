using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizza.Models;

public class Taste
{

    public int Id { get; set; }


    public string? taste { get; set; }

    public double Price { get; set; }

}




