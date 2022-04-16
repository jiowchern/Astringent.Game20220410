using Regulus.Remote.Ghost;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Astringent.Game20220410
{
    public class Agent : MonoBehaviour
    {
        public static IObservable<IAgent> Observer { get {
                return AgentRx.GetObservable();
            } }

        public Connecter Connecter;
        public UnityEngine.UI.Text Message;
        private IAgent _Agent;

        // Start is called before the first frame update
        void Start()
        {
            _Agent = Regulus.Remote.Client.Provider.CreateAgent(Astringent.Game20220410.Protocol.Provider.Create(), Connecter);
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
            
            _Agent.Update();
        }

        public void Connect()
        {
             Connecter.Connect("127.0.0.1:53100");
            Message.text = "done";
        }

    }

}
