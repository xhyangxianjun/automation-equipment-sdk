﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Automation.FrameworkExtension.common;
using Automation.FrameworkExtension.deviceDriver;
using Automation.FrameworkExtension.elementsInterfaces;
using Automation.FrameworkExtension.motionDriver;
using Automation.FrameworkExtension.stateMachine;

namespace Automation.FrameworkExtension.frameworkManage
{
    /// <summary>
    /// 
    /// </summary>
    public class FrameworkManager : UserSettings<FrameworkManager>
    {
        #region singleton

        private FrameworkManager()
        {
        }

        public static FrameworkManager Ins { get; private set; } = new FrameworkManager();

        #endregion


        #region  framework manger

        public bool Reboot { get; set; } = false;

        public bool IsSimulate { get; set; } = false;

        public bool IsDebug { get; set; } = false;

        public new void Load(string env)
        {
            if (File.Exists(env))
            {
                var ins = UserSettings<FrameworkManager>.Load(env);
                if (ins != null)
                {
                    Ins = ins;
                }
            }
            else
            {
                Ins = new FrameworkManager();
                Ins.Save(env);
            }

            _debugForm = new FrameworkDebugForm();

            LoadMotionDriverTypes(Directory.GetCurrentDirectory());
            LoadStationTaskTypes(Directory.GetCurrentDirectory());
            LoadDeviceTypes(Directory.GetCurrentDirectory());
        }


        /// <summary>
        /// 初始化PrimsFactory
        /// </summary>
        public void Initialize()
        {
            if (IsDebug)
            {
                _debugForm?.Show();
            }
        }


        public void Terminate()
        {
            if (IsDebug)
            {
                _debugForm?.Hide();
                _debugForm?.Close();
            }
        }


        public override bool Verify()
        {
            return true;
        }



        private static FrameworkDebugForm _debugForm;


        public void Debug(string log)
        {
            if (!Ins.IsDebug)
            {
                return;
            }

            if (_debugForm != null)
            {
                _debugForm.UpdateLog(log);
            }
        }
        public void Error(string msg)
        {
            if (Ins.IsDebug)
            {
                Debug(msg);
            }

            MessageBox.Show(msg, FrameworkExceptionHead, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        public static string FrameworkExceptionHead = "FRAMEWORK EXCEPTION";


        #region framework reflection methods




        public static Dictionary<string, Type> TaskTypes = new Dictionary<string, Type>();
        public static Dictionary<string, Type> MotionCardTypes = new Dictionary<string, Type>();


        public static void LoadMotionDriverTypes(string folder)
        {
            var files = Directory.GetFiles(folder).Select(f => new FileInfo(f)).ToList();
            files = files.FindAll(f => f.Name.EndsWith(".dll") && f.Name.StartsWith("Automation"));

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var motionCardTypes = assembly.GetExportedTypes().Where(t => t.GetInterface(nameof(IMotionCard)) != null);
                foreach (var t in motionCardTypes)
                {
                    MotionCardTypes.Add(t.Name, t);
                }
            }
            FrameworkManager.Ins.Debug($"加载运动驱动类型：\n{string.Join("\n", MotionCardTypes.Select(t => t.Key))}");
        }


        public static void LoadStationTaskTypes(string folder)
        {
            var files = Directory.GetFiles(folder).Select(f => new FileInfo(f)).ToList();
            files = files.FindAll(f => (f.Name.EndsWith(".dll") && f.Name.StartsWith("Automation")) || f.Name.EndsWith(".exe"));

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var taskTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(StationTask)));
                foreach (var t in taskTypes)
                {
                    TaskTypes.Add(t.Name, t);
                }
            }
            FrameworkManager.Ins.Debug($"加载用户定义任务类型：\n{string.Join("\n", TaskTypes.Select(t => t.Key))}");
        }

        public static void LoadDeviceTypes(string folder)
        {
            var files = Directory.GetFiles(folder).Select(f => new FileInfo(f)).ToList();
            files = files.FindAll(f => (f.Name.EndsWith(".dll") && f.Name.StartsWith("Automation")) || f.Name.EndsWith(".exe"));

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var deviceTypes = assembly.GetTypes().Where(t => t.GetInterface(nameof(IDevice)) != null);
                foreach (var t in deviceTypes)
                {
                    DeviceManager.Ins.DeviceTypes.Add(t.Name, t);
                }
            }
            FrameworkManager.Ins.Debug($"加载用户定义设备类型：\n{string.Join("\n", DeviceManager.Ins.DeviceTypes.Select(t => t.Key))}");
        }

        #endregion


        public class DeviceManager
        {
            private DeviceManager()
            {

            }

            public static DeviceManager Ins { get; } = new DeviceManager();


            public Dictionary<string, Type> DeviceTypes = new Dictionary<string, Type>();


            public void Import(string line, StateMachine machine)
            {
                var data = line.Split(' ');

                var i = 0;
                var index = data[i++];
                var name = data[i++];
                var id = data[i++];
                var type = data[i++];

                if (DeviceTypes.ContainsKey(type))
                {
                    var dev = (IDevice)Activator.CreateInstance(DeviceTypes[type]);
                    dev.Import(line, machine);
                }
            }
        }

    }
}