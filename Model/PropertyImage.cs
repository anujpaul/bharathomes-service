using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PropertyImage
{
    [Key]
    public int Id { get; set; } // auto increment int is fine here
    public string Url { get; set; } = string.Empty;
    public int Order { get; set; } // to preserve image order

    public string PropertyId { get; set; } = string.Empty;
    [ForeignKey("PropertyId")]
    public Property Property { get; set; } = null!;
}