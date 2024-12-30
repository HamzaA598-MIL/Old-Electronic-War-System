using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    namespace ElecWarSystem.Enums
{
    [Flags]
    enum Connectivity : byte
    {
        AllDisconnected = 0b_0000_0000,
        CommandorStratigic = 0b_0000_0001,
        OperationsStratigic = 0b_0000_0010,
        ExtStratigic = 0b_0000_0100,
        CommandorCentral = 0b_0000_1000,
        OperationsCentral = 0b_0001_0000,
        WirelessConnections = 0b_0010_0000,
        DataTranfer = 0b_0100_0000,
        HotLine = 0b_1000_0000,
        AllConnected = 0b_1111_1111,
    }
}