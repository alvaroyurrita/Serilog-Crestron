using Crestron.SimplSharp;

namespace CrestronLoggingDemo_SIMPL
{
    public partial class CrestronLoggingDemoSimpl
    {
        private void CrestronEnvironmentOnProgramStatusEventHandler(eProgramStatusEventType programeventtype)
        {
            switch (programeventtype)
            {
                case eProgramStatusEventType.Stopping:
                    break;
                case eProgramStatusEventType.Paused:
                    break;
                case eProgramStatusEventType.Resumed:
                    break;
                default:
                    break;
            }
        }
        private void CrestronEnvironmentOnSystemEventHandler(eSystemEventType systemeventtype)
        {
            switch (systemeventtype)
            {
                case eSystemEventType.Rebooting:
                    break;
                case eSystemEventType.DiskInserted:
                    break;
                case eSystemEventType.DiskRemoved:
                    break;
                case eSystemEventType.TimeChange:
                    break;
                case eSystemEventType.AuthenticationStateChange:
                    break;
                default:
                    break;
            }
        }
        private void CrestronEnvironmentOnEthernetEventHandler(EthernetEventArgs etherneteventargs)
        {
            switch (etherneteventargs.EthernetEventType)
            {
                case eEthernetEventType.LinkDown:
                    break;
                case eEthernetEventType.LinkUp:
                    break;
                default:
                    break;
            }
        }
    }
}
