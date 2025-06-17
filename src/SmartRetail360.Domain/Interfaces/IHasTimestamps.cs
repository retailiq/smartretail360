namespace SmartRetail360.Domain.Interfaces;

public interface IHasCreatedAt
{
    DateTime CreatedAt { get; set; }
}

public interface IHasUpdatedAt
{
    DateTime UpdatedAt { get; set; }
}