﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Common.ControllerDataModel
{
    [ServiceContract]
    public interface ISendControllerData
    {
        [OperationContract]
        bool SendControllerData(ControllerData data);
    }
}
