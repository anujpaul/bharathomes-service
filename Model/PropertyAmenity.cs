using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PropertyAmenity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string PropertyId { get; set; } = string.Empty;
    [ForeignKey("PropertyId")]
    public Property Property { get; set; } = null!;
}