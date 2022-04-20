using Regulus.Remote.Ghost;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Astringent.Game20220410
{
    public class Agent : MonoBehaviour
    {

        static event System.Action<IAgent> _ActiveEvent;

        public static System.IObservable<IAgent> Observer {
            get {
                return UniRx.Observable.FromEvent<Action<IAgent>, IAgent>(h => (gpi) => h(gpi), h => ActiveEvent += h, h => ActiveEvent -= h);
            }
        }
        
        public static event System.Action<IAgent> ActiveEvent { 
            add { 
                var agent = GameObject.FindObjectOfType<Agent>();
                if(agent != null)
                {
                    if(agent.GetAgent() !=null)
                    {
                        value(agent.GetAgent());
                    }
                }

                _ActiveEvent += value;
            }
            remove {

                _ActiveEvent -= value;
            } 
        }

        public int Port;
        public Connecter Connecter;
        public UnityEngine.UI.Text Ip;
        private IAgent _Agent;


        System.Action _UpdateAction;
        System.Action _DestroyAction;

        public Agent()
        {
            _UpdateAction = _UpdateEmpty;
            _DestroyAction = _UpdateEmpty;
        }

        private void _UpdateEmpty()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            //_Agent = Regulus.Remote.Client.Provider.CreateAgent(Astringent.Game20220410.Protocol.Provider.Create(), Connecter);
        }

        internal static IAgent FindAgent()
        {
            var obj = GameObject.FindObjectOfType<Agent>();
            if(obj == null)
                return null;
            return obj.GetAgent();
        }

        private IAgent GetAgent()
        {
            return _Agent;
        }

        // Update is called once per frame
        void Update()
        {
            _UpdateAction();
            
        }

        public void Connect()
        {
            Disconnect();

            _Agent = Regulus.Remote.Client.Provider.CreateAgent(Astringent.Game20220410.Protocol.Provider.Create(), Connecter);
            _UpdateAction = _Agent.Update;
            _DestroyAction = () => {
                Connecter.Disconnect();
                _UpdateAction = _UpdateEmpty;
                _Agent.Dispose();
                _Agent = null;
            };
            if(_ActiveEvent !=null)
                _ActiveEvent(_Agent);
            Connecter.Connect($"{Ip.text}:{Port}");            
        }


        public void Disconnect()
        {
            _DestroyAction();
            _DestroyAction = _UpdateEmpty;
            
            
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

    }

}
