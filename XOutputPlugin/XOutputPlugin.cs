using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreePIE.Core.Contracts;
using XOutputPlugin.Globals;
using SlimDX.XInput;

namespace XOutputPlugin
{

    [GlobalType(Type = typeof(XOutputGlobal), IsIndexed = true)]
    public class XOutputPlugin : IPlugin
    {
        private List<XOutputGlobalHolder> holders = new List<XOutputGlobalHolder>();

        public event EventHandler Started;

        public object CreateGlobal()
        {
            holders = new List<XOutputGlobalHolder>();
            return new GlobalIndexer<XOutputGlobal, uint>(Create);
        }

        private XOutputGlobal Create(uint index)
        {
            var holder = new XOutputGlobalHolder(index + 1);
            holders.Add(holder);
            return holder.Global;
        }

        public void Stop()
        {
            holders.ForEach(h => h.Dispose());
        }

        public void DoBeforeNextExecute()
        {
            holders.ForEach(h => h.SendPressed());
        }

        public Action Start()
        {
            return null;
        }

        public bool GetProperty(int index, IPluginProperty property)
        {
            return false;
        }

        public bool SetProperties(Dictionary<string, object> properties)
        {
            return false;
        }

        public string FriendlyName
        {
            get { return "xOutput"; }
        }
    }

    public class XOutputGlobalHolder : IDisposable
    {
        private DS3Device device;

        public XOutputGlobalHolder(uint index)
        {
            Global = new XOutputGlobal(this);
            device = new DS3Device(index);
            device.Open();
            device.Plugin();
        }

        public void SetTrigger(XOutputTrigger trigger, int value)
        {
            device.SetTrigger(trigger, (Byte)value);
        }

        public void SetButton(XOutputButton button, bool pressed)
        {
            device.SetButton(button, pressed);
        }

        public void SetPressed(XOutputButton button)
        {
            device.SetButton(button, true);
        }

        public void SendPressed()
        {
            device.SendPressed();
        }

        public void SetAxis(XOutputAxis axis, int value)
        {
            device.SetAxis(axis, value);
        }

        public int GetAxis(XOutputAxis axis)
        {
            return device.GetAxis(axis);
        }

        public int GetTrigger(XOutputTrigger trigger)
        {
            return device.GetTrigger(trigger);
        }

        public bool GetButton(XOutputButton button)
        {
            return device.GetButton(button);
        }

        public void SetReset(bool value)
        {
            device.ResetAxisOnExecute = value;
        }

        public bool GetReset()
        {
            return device.ResetAxisOnExecute;
        }

        public void Dispose()
        {
            device.Unplug();
        }

        public XOutputGlobal Global { get; private set; }
    }

    [Global(Name = "xoutput")]
    public class XOutputGlobal
    {
        private readonly XOutputGlobalHolder holder;

        public XOutputGlobal(XOutputGlobalHolder holder)
        {
            this.holder = holder;
        }

        #region Axis
        public int lx
        {
            get { return holder.GetAxis(XOutputAxis.LX); }
            set { holder.SetAxis(XOutputAxis.LX, value); }
        }

        public int ly
        {
            get { return holder.GetAxis(XOutputAxis.LY); }
            set { holder.SetAxis(XOutputAxis.LY, value); }
        }

        public int rx
        {
            get { return holder.GetAxis(XOutputAxis.RX); }
            set { holder.SetAxis(XOutputAxis.RX, value); }
        }

        public int ry
        {
            get { return holder.GetAxis(XOutputAxis.RY); }
            set { holder.SetAxis(XOutputAxis.RY, value); }
        }
        #endregion

        #region Triggers
        public int R2
        {
            get { return holder.GetTrigger(XOutputTrigger.R2); }
            set { holder.SetTrigger(XOutputTrigger.R2, value); }
        }

        public int L2
        {
            get { return holder.GetAxis(XOutputAxis.RY); }
            set { holder.SetAxis(XOutputAxis.RY, value); }
        }
        #endregion

        #region Buttons
        public bool Up
        {
            get { return holder.GetButton(XOutputButton.Up); }
            set { holder.SetButton(XOutputButton.Up, value); }
        }

        public bool Down
        {
            get { return holder.GetButton(XOutputButton.Down); }
            set { holder.SetButton(XOutputButton.Down, value); }
        }

        public bool Left
        {
            get { return holder.GetButton(XOutputButton.Left); }
            set { holder.SetButton(XOutputButton.Left, value); }
        }

        public bool Right
        {
            get { return holder.GetButton(XOutputButton.Right); }
            set { holder.SetButton(XOutputButton.Right, value); }
        }

        public bool Start
        {
            get { return holder.GetButton(XOutputButton.Start); }
            set { holder.SetButton(XOutputButton.Start, value); }
        }

        public bool Back
        {
            get { return holder.GetButton(XOutputButton.Back); }
            set { holder.SetButton(XOutputButton.Back, value); }
        }

        public bool L3
        {
            get { return holder.GetButton(XOutputButton.L3); }
            set { holder.SetButton(XOutputButton.L3, value); }
        }

        public bool R3
        {
            get { return holder.GetButton(XOutputButton.R3); }
            set { holder.SetButton(XOutputButton.R3, value); }
        }

        public bool L1
        {
            get { return holder.GetButton(XOutputButton.L1); }
            set { holder.SetButton(XOutputButton.L1, value); }
        }

        public bool R1
        {
            get { return holder.GetButton(XOutputButton.R1); }
            set { holder.SetButton(XOutputButton.R1, value); }
        }

        public bool Guide
        {
            get { return holder.GetButton(XOutputButton.Guide); }
            set { holder.SetButton(XOutputButton.Guide, value); }
        }

        public bool Y
        {
            get { return holder.GetButton(XOutputButton.Y); }
            set { holder.SetButton(XOutputButton.Y, value); }
        }

        public bool B
        {
            get { return holder.GetButton(XOutputButton.B); }
            set { holder.SetButton(XOutputButton.B, value); }
        }

        public bool A
        {
            get { return holder.GetButton(XOutputButton.A); }
            set { holder.SetButton(XOutputButton.A, value); }
        }

        public bool X
        {
            get { return holder.GetButton(XOutputButton.X); }
            set { holder.SetButton(XOutputButton.X, value); }
        }
        #endregion

        public bool ResetAxisOnExecute {
            get { return holder.GetReset(); }
            set { holder.SetReset(value); }
        }
        public int AxisMax { get { return Int16.MaxValue; } }
        public int AxisMin { get { return Int16.MinValue; } }
        public int TriggerMax { get { return Byte.MaxValue; } }
        public int TriggerMin { get { return Byte.MaxValue; } }

        public void setButton(XOutputButton button, bool pressed)
        {
            holder.SetButton(button, pressed);
        }

    }
    
}
