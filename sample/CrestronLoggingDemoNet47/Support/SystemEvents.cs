using System;
using Crestron.SimplSharp;

namespace CrestronLoggingDemo
{
    public partial class ControlSystem
    {
        private void ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {
                case eEthernetEventType.LinkDown:
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                    }
                    break;
                case eEthernetEventType.LinkUp:
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case eProgramStatusEventType.Paused:
                    break;
                case eProgramStatusEventType.Resumed:
                    break;
                case eProgramStatusEventType.Stopping:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(programStatusEventType), programStatusEventType, null);
            }
        }
        private void ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case eSystemEventType.DiskInserted:
                    break;
                case eSystemEventType.DiskRemoved:
                    break;
                case eSystemEventType.Rebooting:
                    break;
                case eSystemEventType.TimeChange:
                    break;
                case eSystemEventType.AuthenticationStateChange:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(systemEventType), systemEventType, null);
            }
        }
    }
}