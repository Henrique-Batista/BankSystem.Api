using System.ComponentModel.DataAnnotations;

namespace BankSystem.Domain.Models;

public abstract class Entity
{
    [Key]
    public Guid Id { get; private init; }
    
    protected Entity()
    {
        Id = Guid.NewGuid();
    }
}