using System.ComponentModel.DataAnnotations.Schema;

public class PropertyAgent
{
    public string PropertyId { get; set; } = string.Empty;
    [ForeignKey("PropertyId")]
    public Property Property { get; set; } = null!;

    public string AgentId { get; set; } = string.Empty;
    [ForeignKey("AgentId")]
    public Agent Agent { get; set; } = null!;
}