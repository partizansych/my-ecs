namespace MyECS.Core;

public readonly record struct Entity
{
    public readonly int Id;
    public readonly int Version;

    public Entity(int id, int version)
    {
        Id = id;
        Version = version;
    }

    public static readonly Entity Null = new(-1, 0);
}