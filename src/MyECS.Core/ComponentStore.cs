using System.Runtime.CompilerServices;

namespace MyECS.Core;

public interface IComponentStore
{
    void Remove(int entityId);
}

public class ComponentStore<T> : IComponentStore where T : struct
{
    private readonly int[] sparse;
    private readonly T[] dense;
    private readonly int[] entities;
    private int count;

    public ComponentStore(int maxEntities)
    {
        sparse = new int[maxEntities];
        Array.Fill(sparse, -1);
        dense = new T[maxEntities];
        entities = new int[maxEntities];
        count = 0;
    }

    public int Count => count;
    public ReadOnlySpan<T> AsSpan => dense.AsSpan(0, count);
    public int GetEntityAtDenseIndex(int index) => entities[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(int entityId)
    {
        int index = sparse[entityId];
        return ref dense[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Has(int entityId)
    {
        return entityId < sparse.Length && sparse[entityId] != -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int entityId, T component)
    {
        int index = sparse[entityId];
        if (index != -1)
            dense[index] = component;
        else
            Add(entityId, component);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int entityId, T component)
    {
        if (sparse[entityId] != -1) return; // TODO: Выдавать предупреждение

        sparse[entityId] = count;
        dense[count] = component;
        entities[count] = entityId; // Запоминаем айди сущности, к которой принадлежит компонент
        count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove(int entityId)
    {
        int indexToRemove = sparse[entityId];
        if (indexToRemove == -1) return;

        int lastIndex = count - 1;
        int lastEntityId = entities[lastIndex];

        // SWAP: Переносим последний элемент на место удаляемого
        dense[indexToRemove] = dense[lastIndex];
        entities[indexToRemove] = lastEntityId;

        // Обновляем указатель в Sparse для перенесенного элемента
        sparse[lastEntityId] = indexToRemove;

        // POP: Очищаем данные
        sparse[entityId] = -1;
        count--;
    }
}
