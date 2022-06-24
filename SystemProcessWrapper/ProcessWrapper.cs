using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SystemProcessWrapper
{
    public class ProcessWrapper
    {
        private ProcessStartInfo procStartInfo;
        private Process process;
        private Processes _pro;
        private Dictionary<Processes, string> _DicProcesses = new Dictionary<Processes, string>
            {
                { Processes.AccessibilityOptions, "access.cpl" },
                { Processes.AddNewHardware, "sysdm.cpl add new hardware" },
                { Processes.AddRemovePrograms, "appwiz.cpl" },
                { Processes.DateTimeProperties, "timedate.cpl" },
                { Processes.DisplayProperties, "desk.cpl" },
                { Processes.FindFast, "findfast.cpl" },
                { Processes.FontsFolder, "fonts" },
                { Processes.InternetProperties, "inetcpl.cpl" },
                { Processes.JoystickProperties, "joy.cpl" },
                { Processes.KeyboardProperties, "main.cpl keyboard" },
                { Processes.MicrosoftExchange, "mlcfg32.cpl" },
                { Processes.MicrosoftMailPostOffice, "wgpocpl.cpl" },
                { Processes.ModemProperties, "modem.cpl" },
                { Processes.MouseProperties, "main.cpl" },
                { Processes.MultimediaProperties, "mmsys.cpl" },
                { Processes.NetworkProperties, "netcpl.cpl" },
                { Processes.PasswordProperties, "password.cpl" },
                { Processes.PCCard, "main.cpl pc card (PCMCIA)" },
                { Processes.PowerManagement_Windows95, "main.cpl power" },
                { Processes.PowerManagement_Windows98, "powercfg.cpl" },
                { Processes.PrintersFolder, "printers" },
                { Processes.RegionalSettings, "intl.cpl" },
                { Processes.ScannersAndCameras, "sticpl.cpl" },
                { Processes.SoundProperties, "mmsys.cpl sounds" },
                { Processes.SystemProperties, "sysdm.cpl" }
            };
        private string _currentProcess;
        public ProcessWrapper(Processes pro)
        {
            _pro = pro;
            if (_DicProcesses.TryGetValue(pro, out _currentProcess))
            {
                procStartInfo = new ProcessStartInfo("control", _currentProcess);
                process = new Process();
                process.Exited += Process_Exited;
                process.Disposed += Process_Disposed;

                process.StartInfo = procStartInfo;
                process.Start();
            }
        }

        private void Process_Disposed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (process != null)
            {
                process.Dispose();
            }
        }
    }
}
