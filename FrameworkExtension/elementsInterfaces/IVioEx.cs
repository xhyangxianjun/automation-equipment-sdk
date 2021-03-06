﻿using Automation.FrameworkExtension.elements;
using Automation.FrameworkExtension.motionDriver;

namespace Automation.FrameworkExtension.elementsInterfaces
{
    public interface IVioEx : IElement
    {
        VioType Type { set; get; }
        string Description { set; get; }
        bool Enable { set; get; }


        string DriverName { set; get; }
        int Port { set; get; }


        MotionCardWrapper Driver { get; }
    }
}