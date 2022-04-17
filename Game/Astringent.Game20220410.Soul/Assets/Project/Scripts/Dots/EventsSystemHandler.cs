using System;

namespace Astringent.Game20220410.Dots.Systems
{
    public class EventsSystemHandler<T>
    {
        public struct Data
        {
            public int Id;
            public T State;
        }
        public event System.Action<int, T> StateEvent;

        readonly Unity.Collections.NativeQueue<Data> _Queue;
        public EventsSystemHandler()
        {
            StateEvent += _Empty;
            _Queue  =new Unity.Collections.NativeQueue<Data>(Unity.Collections.Allocator.Persistent);
        }

        private void _Empty(int arg1, T arg2)
        {
            
        }

     

        public void Update(System.Action<Unity.Collections.NativeQueue<Data>.ParallelWriter> update)
        {
            Unity.Collections.NativeQueue<Data>.ParallelWriter writter = _Queue.AsParallelWriter();

            update(writter);

            while (_Queue.TryDequeue(out var data))
            {
                StateEvent.Invoke(data.Id, data.State);
            }
        }

        internal void Dispose()
        {
            _Queue.Dispose();
        }
    }
}
