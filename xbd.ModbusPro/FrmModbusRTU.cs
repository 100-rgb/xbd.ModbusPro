using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xbd.DataConvertLib;
using xbd.ModbusLib;

namespace xbd.ModbusPro
{
    public enum StoreArea
    {
        输出线圈0x,
        输入线圈1x,
        输入寄存器3x,
        保持寄存器4x
    }

    public partial class FrmModbusRTU : Form
    {
        public FrmModbusRTU()
        {
            InitializeComponent();
            InitParam();
        }
        //参数
        private byte slaveId = 1;
        private ushort start = 0;
        private ushort count = 1;
        private DataType dataType;
        private StoreArea storeArea;
        private DataFormat dataFormat;
        private OperateResult<bool[]> rcResult;
        private OperateResult<byte[]> rrResult;

        private bool IsConnected=false;

        //通信对象
        private ModbusRTU modbus = new ModbusRTU();

        //初始化通信参数
        private void InitParam()
        {
            this.lst_Info.Columns[1].Width = this.lst_Info.Width - this.lst_Info.Columns[0].Width - 20;

            //初始化端口号
            string[] portList = SerialPort.GetPortNames();
            if (portList.Length>0)
            {
                this.cmb_PortName.Items.AddRange(portList);
                this.cmb_PortName.SelectedIndex = 0;
            }
            //初始化波特率
            this.cmb_BaudRate.Items.AddRange(new string[]
            {
                "4800","9600","19200","38400"
            });
            this.cmb_BaudRate.SelectedIndex = 1;
            //初始化校验位
            this.cmb_Parity.Items.AddRange(Enum.GetNames(typeof(Parity)));
            this.cmb_Parity.SelectedIndex = 0;
            //初始化数据位
            this.cmb_DataBits.Items.AddRange(new string[]{"7","8"});
            this.cmb_DataBits.SelectedIndex = 1;
            //初始化停止位
            this.cmb_StopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));
            this.cmb_StopBits.SelectedIndex = 1;
            //初始化大小端
            this.cmb_DataFormat.Items.AddRange(Enum.GetNames(typeof(DataFormat)));
            this.cmb_DataFormat.SelectedIndex = 0;
            //初始化存储区
            this.cmb_StoreArea.Items.AddRange(Enum.GetNames(typeof(StoreArea)));
            this.cmb_StoreArea.SelectedIndex = 0;
            //初始化数据类型
            this.cmb_DataType.Items.AddRange(Enum.GetNames(typeof (DataType)));
            this.cmb_DataType.SelectedIndex = 0;

            //初始化TextBox
            this.txt_SlaveId.Text = slaveId.ToString();
            this.txt_Start.Text = start.ToString();
            this.txt_Count.Text = count.ToString();
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                AddLog(1, "串口已经打开");
                return;
            }

            Parity parity = (Parity)Enum.Parse(typeof(Parity), this.cmb_Parity.Text);
            StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits),this.cmb_StopBits.Text);

            var result =  modbus.Open(this.cmb_PortName.Text, Convert.ToInt32(this.cmb_BaudRate.Text),parity,Convert.ToInt32(this.cmb_DataBits.Text),stopBits);
            if (result.IsSuccess)
            {
                AddLog(0,"串口打开成功");
                IsConnected = true;
            }
            else
            {
                AddLog(2, "串口打开失败"+result.Message);
                IsConnected= false;
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            modbus.Close();
            IsConnected = false;
            AddLog(0, "串口关闭成功");
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="index"></param>
        /// <param name="log"></param>
        private void AddLog(int index,string log)
        {
            ListViewItem listViewItem = new ListViewItem("   "+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), index);
            listViewItem.SubItems.Add(log);
            //往前插入
            this.lst_Info.Items.Insert(0,listViewItem);
        }

        private void btn_Read_Click(object sender, EventArgs e)
        {
            if (CommonVerify())
            {
                switch (dataType)   
                {
                    case DataType.Bool:
                        ReadBool(storeArea,slaveId,start,count);
                        break;
                    case DataType.Short:
                        ReadShort(storeArea, slaveId,start,count);
                        break;
                    case DataType.UShort:
                        ReadUShort(storeArea,slaveId,start,count);
                        break;
                    case DataType.Int:
                        ReadInt(storeArea,slaveId,start,count);
                        break;
                    case DataType.UInt:
                        ReadUInt(storeArea,slaveId,start,count);
                        break;
                    case DataType.Float:
                        ReadFloat(storeArea,slaveId,start,count);
                        break;
                    default:
                        AddLog(1, "读取失败，暂时不支持该类型");
                        break;
                }

            }
        }

        /// <summary>
        /// 读取Bool值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        private void ReadBool(StoreArea storeArea,byte slaveId,ushort start,ushort count)
        {
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                    rcResult = modbus.ReadCoils(start,count,slaveId);
                    break;
                case StoreArea.输入线圈1x:
                    rcResult = modbus.ReadInputs(start,count,slaveId);
                    break;
                default:
                    rcResult = OperateResult.CreateFailResult<bool[]>("暂时不支持该存储区");
                    break;
            }
            if (rcResult.IsSuccess)
            {
                AddLog(0, "读取成功：" + StringLib.GetStringFromValueArray(rcResult.Content));
            }
            else
            {
                AddLog(1, "读取失败："+rcResult.Message);
            }
        }
        /// <summary>
        /// 读取Short值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        private void ReadShort(StoreArea storeArea,byte slaveId,ushort start,ushort count)
        {
            switch (storeArea)
            {
                case StoreArea.输入寄存器3x:
                    rrResult = modbus.ReadInputsRegisters(start, count, slaveId);
                    break;
                case StoreArea.保持寄存器4x:
                    rrResult = modbus.ReadHoldingRegisters(start, count, slaveId);
                    break;
                default:
                    rrResult = OperateResult.CreateFailResult<byte[]>("暂时不支持该存储区");
                    break;
            }
            if (rrResult.IsSuccess)
            {
                AddLog(0, "读取成功：" + StringLib.GetStringFromValueArray(ShortLib.GetShortArrayFromByteArray(rrResult.Content,this.dataFormat)));
            }
            else
            {
                AddLog(1, "读取失败：" + rrResult.Message);
            }
        }
        /// <summary>
        /// 读取UShort值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        private void ReadUShort(StoreArea storeArea, byte slaveId, ushort start, ushort count)
        {
            switch (storeArea)
            {
                case StoreArea.输入寄存器3x:
                    rrResult = modbus.ReadInputsRegisters(start, count, slaveId);
                    break;
                case StoreArea.保持寄存器4x:
                    rrResult = modbus.ReadHoldingRegisters(start, count, slaveId);
                    break;
                default:
                    rrResult = OperateResult.CreateFailResult<byte[]>("暂时不支持该存储区");
                    break;
            }
            if (rrResult.IsSuccess)
            {
                AddLog(0, "读取成功：" + StringLib.GetStringFromValueArray(UShortLib.GetUShortArrayFromByteArray(rrResult.Content,this.dataFormat)));
            }
            else
            {
                AddLog(1, "读取失败：" + rrResult.Message);
            }
        }
        /// <summary>
        /// 读取Int值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        private void ReadInt(StoreArea storeArea, byte slaveId, ushort start, ushort count)
        {
            switch (storeArea)
            {
                case StoreArea.输入寄存器3x:
                    rrResult = modbus.ReadInputsRegisters(start, (ushort)(count*2), slaveId);
                    break;
                case StoreArea.保持寄存器4x:
                    rrResult = modbus.ReadHoldingRegisters(start, (ushort)(count * 2), slaveId);
                    break;
                default:
                    rrResult = OperateResult.CreateFailResult<byte[]>("暂时不支持该存储区");
                    break;
            }
            if (rrResult.IsSuccess)
            {
                AddLog(0, "读取成功：" + StringLib.GetStringFromValueArray(IntLib.GetIntArrayFromByteArray(rrResult.Content, this.dataFormat)));
            }
            else
            {
                AddLog(1, "读取失败：" + rrResult.Message);
            }
        }
        /// <summary>
        /// 读取UInt值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        private void ReadUInt(StoreArea storeArea, byte slaveId, ushort start, ushort count)
        {
            switch (storeArea)
            {
                case StoreArea.输入寄存器3x:
                    rrResult = modbus.ReadInputsRegisters(start, (ushort)(count * 2), slaveId);
                    break;
                case StoreArea.保持寄存器4x:
                    rrResult = modbus.ReadHoldingRegisters(start, (ushort)(count * 2), slaveId);
                    break;
                default:
                    rrResult = OperateResult.CreateFailResult<byte[]>("暂时不支持该存储区");
                    break;
            }
            if (rrResult.IsSuccess)
            {
                AddLog(0, "读取成功：" + StringLib.GetStringFromValueArray(UIntLib.GetUIntArrayFromByteArray(rrResult.Content, this.dataFormat)));
            }
            else
            {
                AddLog(1, "读取失败：" + rrResult.Message);
            }
        }
        /// <summary>
        /// 读取Float值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        private void ReadFloat(StoreArea storeArea, byte slaveId, ushort start, ushort count)
        {
            switch (storeArea)
            {
                case StoreArea.输入寄存器3x:
                    rrResult = modbus.ReadInputsRegisters(start, (ushort)(count * 2), slaveId);
                    break;
                case StoreArea.保持寄存器4x:
                    rrResult = modbus.ReadHoldingRegisters(start, (ushort)(count * 2), slaveId);
                    break;
                default:
                    rrResult = OperateResult.CreateFailResult<byte[]>("暂时不支持该存储区");
                    break;
            }
            if (rrResult.IsSuccess)
            {
                AddLog(0, "读取成功：" + StringLib.GetStringFromValueArray(FloatLib.GetFloatArrayFromByteArray(rrResult.Content, this.dataFormat)));
            }
            else
            {
                AddLog(1, "读取失败：" + rrResult.Message);
            }
        }

        /// <summary>
        /// 通用验证方法
        /// </summary>
        /// <returns></returns>
        private bool CommonVerify()
        {
            if (!IsConnected)
            {
                AddLog(1, "请检查是否打开串口");
                return false;
            }
            if(!byte.TryParse(this.txt_SlaveId.Text,out slaveId))
            {
                AddLog(1, "请检查站地址是否为有效的字节类型");
                return false;
            }
            if (!ushort.TryParse(this.txt_Start.Text, out start))
            {
                AddLog(1, "请检查起始地址是否为有效的无符号整型");
                return false;
            }
            if (!ushort.TryParse(this.txt_Count.Text, out count))
            {
                AddLog(1, "请检查读取数量是否为有效的无符号整型");
                return false;
            }
            dataType=(DataType)Enum.Parse(typeof(DataType),this.cmb_DataType.Text,true);
            storeArea=(StoreArea)Enum.Parse(typeof(StoreArea),this.cmb_StoreArea.Text,true);
            dataFormat=(DataFormat)Enum.Parse(typeof(DataFormat),this.cmb_DataFormat.Text,true);

            return true;
        }
    }
}
