using Common.DeviceDataModel;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.ControllerDataModel.DataModelInterfaces
{
    public interface IController
    {
        ConcurrentDictionary<int, IList<InnerDeviceData>> LocalBuffer { get; }

        string LocalDatabasePath { get; }
        string DataEndpoint { get; }
        string AliveEndpoint { get; }
        int ControllerCode { get; }
        uint RetryTime { get; }
        uint ControllerCycle { get; }
    }
}
