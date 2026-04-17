namespace MyECS.Core;

public class World(int maxEntities)
{
    private readonly int maxEntities = maxEntities;
    private readonly Dictionary<Type, IComponentStore> stores = [];

    private readonly int[] versions = new int[maxEntities];
    private readonly Stack<int> freeIds = [];
    private int nextId = 0;

    public bool IsAlive(Entity entity)
    {
        return versions[entity.Id] == entity.Version;
    }

    public Entity Create()
    {
        int id = freeIds.Count > 0 ? freeIds.Pop() : nextId++;
        return new Entity(id, versions[id]);
    }

    public void Destroy(Entity entity)
    {
        if (!IsAlive(entity)) return;
        foreach (var store in stores.Values) store.Remove(entity.Id);
        versions[entity.Id]++;
        freeIds.Push(entity.Id);
    }

    public ComponentStore<T> GetStore<T>() where T : struct
    {
        var type = typeof(T);
        if (!stores.TryGetValue(type, out var store))
        {
            store = new ComponentStore<T>(maxEntities);
            stores[type] = store;
        }
        return (ComponentStore<T>)store;
    }
}
