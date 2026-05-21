namespace OnboardingSIGDB1.Domain.Base;

public abstract class BaseEntity<T> : BaseElement<T> where T : BaseEntity<T>
{
    public int Id { get; protected set; }
}