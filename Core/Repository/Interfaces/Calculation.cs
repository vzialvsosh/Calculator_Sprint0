namespace Core.Repository.Interfaces;

public class Calculation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Expression { get; set; } = string.Empty;
    public double Result { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}