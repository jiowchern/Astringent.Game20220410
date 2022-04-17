using System;
using System.Linq;



namespace Astringent.Game20220410.Sources
{
    public class IdDispenser
    {
        
        int _IdProvider;
        
        class Renter
        {
            public WeakReference Target;
            public int Id;
        }

        readonly System.Collections.Generic.Queue<int> _Reserves;
        readonly System.Collections.Generic.List<Renter> _Renters;
        public IdDispenser()
        {
            _Reserves = new System.Collections.Generic.Queue<int>();
            _Renters = new System.Collections.Generic.List<Renter>();
        }

        public int Dispatch<T>(T applicant) where T : class
        {
            
            _Recycling();
            int id;
            if(_TryGetReserves(out id))
            {                
                return _GiveOut(applicant , id);
            }

            return _GiveOut(applicant, ++_IdProvider);
        }

        private int _GiveOut<T>(T applicant, int id) where T : class
        {
            _Renters.Add(new Renter() { Target = new WeakReference(applicant), Id = id });
            return id;
        }

        private bool _TryGetReserves(out int id)
        {
            id = 0;
            if(_Reserves.Any() == false)
            {
                return false;
            }

            id = _Reserves.Dequeue();
            return true;
        }

        private void _Recycling()
        {
            var renters = from r in _Renters where r.Target.IsAlive == false select r;

            foreach (var r in renters.ToArray())
            {
                _Renters.Remove(r);
                _Reserves.Enqueue(r.Id);
            }
        }
    }
}
