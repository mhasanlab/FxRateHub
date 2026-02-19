using System;

namespace FxRateHub.Domain.Common;

/// <summary>
/// Base entity class that provides common audit properties for all entities.
/// </summary>
public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Generic base entity class that provides an ID property in addition to audit properties.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public abstract class BaseEntity<TId> : BaseEntity
{
    public TId Id { get; set; } = default!;
}
