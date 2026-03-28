using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeRxKEYs
{
    public interface IXRModule
    {
        string DisplayName { get; set; }
        TrackedObject Head { get; set; }
        TrackedObject L_Hand { get; set; }
        TrackedObject R_Hand { get; set; }

        void Setup();
        void Shutdown();

        void SendCommand(string cmd);
        string RequestData(string request);
        void TriggersComplete();

        TrackedObject GetTrackedObject(SerializableTrackedObject serializedTrackedObj);
    }
}
