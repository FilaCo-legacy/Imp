using System.Collections.Generic;

namespace ImpLite.BroadPhase
{
    public interface IBoxTree<T> where T : IBoxable
    {
        void Insert(T obj);

        void Retrieve(List<T> neighbours, T targetObj);

        void Clear();
    }
}