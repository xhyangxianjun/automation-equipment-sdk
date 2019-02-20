﻿using Automation.FrameworkExtension.common;

namespace DemoMachine.Machine
{
    public class DemoMachineSettings : UserSettings<DemoMachineSettings>
    {
        public string Name { get; set; }

        public string Version { get; set; }





        public override bool Verify()
        {
            return true;
        }
    }
}