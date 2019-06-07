using System.Collections.Generic;

namespace Imp.BroadPhase
{
    public interface IBoxTree<T> where T : IBoxable
    {
        void Insert(T obj);

        void Retrieve(List<T> neighbours, T targetObj);

        void Clear();
    }
}