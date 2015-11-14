using FreePIE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutputPlugin
{
    [GlobalEnum]
    public enum XOutputButton
    {
        A, B, X, Y,
        Up, Down, Left, Right,
        L3, R3, L1, R1,
        Guide, Start, Back
    }

    [GlobalEnum]
    public enum XOutputTrigger
    {
        L2, R2
    }

    [GlobalEnum]
    public enum XOutputAxis
    {
        LX, LY, RX, RY
    }

    class DeviceState
    {
        // 0x1 or 0x0
        public Boolean[] ButtonState { get; set; }
        public Byte[] TriggerState { get; set; }
        public Int16[] AxisState { get; set; }
    }

    class DS3Device : ScpDevice
    {
        public const String DS3_BUS_CLASS_GUID = "{F679F562-3164-42CE-A4DB-E7DDBE723909}";
        public uint Index { get; private set; }
        private DeviceState m_state;

        public DS3Device(uint index) : base(DS3_BUS_CLASS_GUID)
        {
            Index = index;
            ResetState();
        }

        private void ResetState()
        {
            m_state = new DeviceState();
            m_state.ButtonState = new Boolean[Enum.GetNames(typeof(XOutputButton)).Length];
            m_state.TriggerState = new Byte[Enum.GetNames(typeof(XOutputTrigger)).Length];
            m_state.AxisState = new Int16[Enum.GetNames(typeof(XOutputAxis)).Length];
        }

        private Int32 Scale(Int32 Value, Boolean Flip)
        {
            Value -= 0x80;

            if (Value == -128) Value = -127;
            if (Flip) Value *= -1;

            return (Int32)((float)Value * 258.00787401574803149606299212599f);
        }

        public override Boolean Open(int Instance = 0)
        {
            return base.Open(Instance);
        }

        public override Boolean Open(String DevicePath)
        {
            m_Path = DevicePath;
            m_WinUsbHandle = (IntPtr)INVALID_HANDLE_VALUE;

            if (GetDeviceHandle(m_Path))
            {

                m_IsActive = true;

            }
            return true;
        }

        public Boolean Plugin()
        {
            if (IsActive)
            {
                Int32 Transfered = 0;
                Byte[] Buffer = new Byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                Buffer[4] = (Byte)((Index >> 0) & 0xFF);
                Buffer[5] = (Byte)((Index >> 8) & 0xFF);
                Buffer[6] = (Byte)((Index >> 16) & 0xFF);
                Buffer[7] = (Byte)((Index >> 24) & 0xFF);

                return DeviceIoControl(m_FileHandle, 0x2A4000, Buffer, Buffer.Length, null, 0, ref Transfered, IntPtr.Zero);
            }

            return false;
        }

        public Boolean Unplug()
        {
            if (IsActive)
            {
                Int32 Transfered = 0;
                Byte[] Buffer = new Byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                Buffer[4] = (Byte)((Index >> 0) & 0xFF);
                Buffer[5] = (Byte)((Index >> 8) & 0xFF);
                Buffer[6] = (Byte)((Index >> 16) & 0xFF);
                Buffer[7] = (Byte)((Index >> 24) & 0xFF);

                return DeviceIoControl(m_FileHandle, 0x2A4004, Buffer, Buffer.Length, null, 0, ref Transfered, IntPtr.Zero);
            }
            return false;
        }

        public void SetButton(XOutputButton button, Boolean pressed)
        {
            m_state.ButtonState[(int)button] = pressed;
        }

        public void SetTrigger(XOutputTrigger trigger, Int32 value)
        {
            m_state.TriggerState[(int)trigger] = Convert.ToByte(InRange(value, Byte.MinValue, Byte.MaxValue));
        }

        public void SetAxis(XOutputAxis axis, Int32 value)
        {
            m_state.AxisState[(int)axis] = Convert.ToInt16(InRange(value, Int16.MinValue, Int16.MaxValue));
        }

        public Int32 InRange(Int32 value, Int32 min, Int32 max)
        {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        private int IntValue(XOutputButton button)
        {
            return m_state.ButtonState[(int)button] ? 1 : 0;
        }

        private Int32 Scale(XOutputAxis trigger)
        {
            Int32 Value = m_state.AxisState[(int)trigger];
            return Value;
        }

        private byte[] BuildInput()
        {
            byte[] Input = new byte[28];
            Input[0] = 0x1c;
            Input[4] = (Byte)(Index);
            Input[9] = 0x14;

            Input[10] |= (Byte)(IntValue(XOutputButton.Up) << 0); // Up
            Input[10] |= (Byte)(IntValue(XOutputButton.Down) << 1); // Down
            Input[10] |= (Byte)(IntValue(XOutputButton.Left) << 2); // Left
            Input[10] |= (Byte)(IntValue(XOutputButton.Right) << 3); // Right
            Input[10] |= (Byte)(IntValue(XOutputButton.Start) << 4); // Start
            Input[10] |= (Byte)(IntValue(XOutputButton.Back) << 5); // Back
            Input[10] |= (Byte)(IntValue(XOutputButton.L3) << 6); // Left  Thumb
            Input[10] |= (Byte)(IntValue(XOutputButton.R3) << 7); // Right Thumb

            Input[11] |= (Byte)(IntValue(XOutputButton.L1) << 0); // Left  Shoulder
            Input[11] |= (Byte)(IntValue(XOutputButton.R1) << 1); // Right Shoulder
            Input[11] |= (Byte)(IntValue(XOutputButton.Guide) << 2); // Guide

            Input[11] |= (Byte)(IntValue(XOutputButton.Y) << 7); // Y
            Input[11] |= (Byte)(IntValue(XOutputButton.B) << 5); // B
            Input[11] |= (Byte)(IntValue(XOutputButton.A) << 4); // A
            Input[11] |= (Byte)(IntValue(XOutputButton.X) << 6); // X

            Input[12] = m_state.TriggerState[(int)XOutputTrigger.L2]; // Left Trigger
            Input[13] = m_state.TriggerState[(int)XOutputTrigger.R2]; // Right Trigger

            Int32 LX = Scale(XOutputAxis.LX);
            Int32 LY = Scale(XOutputAxis.LY);
            Int32 RX = Scale(XOutputAxis.RX);
            Int32 RY = Scale(XOutputAxis.RY);

            Input[14] = (Byte)((LX >> 0) & 0xFF); // LX
            Input[15] = (Byte)((LX >> 8) & 0xFF);

            Input[16] = (Byte)((LY >> 0) & 0xFF); // LY
            Input[17] = (Byte)((LY >> 8) & 0xFF);

            Input[18] = (Byte)((RX >> 0) & 0xFF); // RX
            Input[19] = (Byte)((RX >> 8) & 0xFF);

            Input[20] = (Byte)((RY >> 0) & 0xFF); // RY
            Input[21] = (Byte)((RY >> 8) & 0xFF);
            
            return Input;
        }

        public Boolean SendPressed()
        {
            if(IsActive)
            {
                Int32 Transfered = 0;
                byte[] Input = BuildInput();
                byte[] Output = new byte[8];
                bool value = DeviceIoControl(m_FileHandle, 0x2A400C, Input, Input.Length, Output, Output.Length, ref Transfered, IntPtr.Zero) && Transfered > 0;
                ResetState();
                return value;
            }
            return false;
        }

    }
}
