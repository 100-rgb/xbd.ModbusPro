using System;
using System.IO.Ports;
using System.Windows.Forms;
using xbd.DataConvertLib;
using xbd.ModbusLib;

namespace xbd.ModbusPro
{

    public partial class FrmModbusTCP : Form
    {
        public FrmModbusTCP()
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
        private OperateResult wResult;

        private bool IsConnected = false;

        //通信对象
        private ModbusTCP modbus = new ModbusTCP();

        //初始化通信参数
        private void InitParam()
        {
            this.lst_Info.Columns[1].Width = this.lst_Info.Width - this.lst_Info.Columns[0].Width - 20;

            //初始化大小端
            this.cmb_DataFormat.Items.AddRange(Enum.GetNames(typeof(DataFormat)));
            this.cmb_DataFormat.SelectedIndex = 0;
            //初始化存储区
            this.cmb_StoreArea.Items.AddRange(Enum.GetNames(typeof(StoreArea)));
            this.cmb_StoreArea.SelectedIndex = 0;
            //初始化数据类型
            this.cmb_DataType.Items.AddRange(Enum.GetNames(typeof(DataType)));
            this.cmb_DataType.SelectedIndex = 0;

            //初始化TextBox
            this.txt_SlaveId.Text = slaveId.ToString();
            this.txt_Start.Text = start.ToString();
            this.txt_Count.Text = count.ToString();
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                AddLog(1, "ModbusTCP已经建立连接");
                return;
            }
            var result = modbus.Connect(this.txt_IP.Text.Trim(), Convert.ToInt32(this.txt_Port.Text.Trim()));
            if (result.IsSuccess)
            {
                IsConnected = true;
                AddLog(0, "ModbusTCP连接成功");
            }
            else
            {
                AddLog(1, "ModbusTCP连接失败："+result.Message);
            }
        }

        private void btn_DisConnect_Click(object sender, EventArgs e)
        {
            modbus.DisConnect();
            IsConnected = false;
            AddLog(0, "Modbus断开连接成功");
        }


        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="index"></param>
        /// <param name="log"></param>
        private void AddLog(int index, string log)
        {
            ListViewItem listViewItem = new ListViewItem("   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), index);
            listViewItem.SubItems.Add(log);
            //往前插入
            this.lst_Info.Items.Insert(0, listViewItem);
        }

        private void btn_Read_Click(object sender, EventArgs e)
        {
            if (CommonVerify())
            {
                switch (dataType)
                {
                    case DataType.Bool:
                        ReadBool(storeArea, slaveId, start, count);
                        break;
                    case DataType.Short:
                        ReadShort(storeArea, slaveId, start, count);
                        break;
                    case DataType.UShort:
                        ReadUShort(storeArea, slaveId, start, count);
                        break;
                    case DataType.Int:
                        ReadInt(storeArea, slaveId, start, count);
                        break;
                    case DataType.UInt:
                        ReadUInt(storeArea, slaveId, start, count);
                        break;
                    case DataType.Float:
                        ReadFloat(storeArea, slaveId, start, count);
                        break;
                    default:
                        AddLog(1, "读取失败，暂时不支持该类型");
                        break;
                }

            }
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (CommonVerify())
            {
                string writeValue = this.txt_Write.Text.Trim();

                if (writeValue.Length == 0)
                {
                    AddLog(1, "写入失败：写入值不能为空");
                }
                switch (dataType)
                {
                    case DataType.Bool:
                        WriteBool(storeArea, slaveId, start, writeValue);
                        break;
                    case DataType.Short:
                        WriteShort(storeArea, slaveId, start, writeValue);
                        break;
                    case DataType.UShort:
                        WriteUShort(storeArea, slaveId, start, writeValue);
                        break;
                    case DataType.Int:
                        WriteInt(storeArea, slaveId, start, writeValue);
                        break;
                    case DataType.UInt:
                        WriteUInt(storeArea, slaveId, start, writeValue);
                        break;
                    case DataType.Float:
                        WriteFloat(storeArea, slaveId, start, writeValue);
                        break;
                    default:
                        AddLog(1, "写入失败：暂不支持该数据类型");
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
        private void ReadBool(StoreArea storeArea, byte slaveId, ushort start, ushort count)
        {
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                    rcResult = modbus.ReadCoils(start, count, slaveId);
                    break;
                case StoreArea.输入线圈1x:
                    rcResult = modbus.ReadInputs(start, count, slaveId);
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
                AddLog(1, "读取失败：" + rcResult.Message);
            }
        }
        /// <summary>
        /// 读取Short值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        private void ReadShort(StoreArea storeArea, byte slaveId, ushort start, ushort count)
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
                AddLog(0, "读取成功：" + StringLib.GetStringFromValueArray(ShortLib.GetShortArrayFromByteArray(rrResult.Content, this.dataFormat)));
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
                AddLog(0, "读取成功：" + StringLib.GetStringFromValueArray(UShortLib.GetUShortArrayFromByteArray(rrResult.Content, this.dataFormat)));
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
        /// 写入Bool值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="writeValue"></param>
        private void WriteBool(StoreArea storeArea, byte slaveId, ushort start, string writeValue)
        {
            bool[] values = BitLib.GetBitArrayFromBitArrayString(writeValue);
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                    //单个线圈写入
                    if (values.Length == 1)
                    {
                        wResult = this.modbus.WriteSingleCoil(start, values[0], slaveId);
                    }
                    //多个线圈写入
                    else
                    {
                        wResult = this.modbus.WriteMultipleCoils(start, values, slaveId);
                    }
                    break;
                case StoreArea.保持寄存器4x:
                    //写入寄存器中的某一位
                    if (values.Length == 1)
                    {
                        wResult = this.modbus.WriteRegisterBit(start + "." + count, values[0], this.dataFormat == DataFormat.ABCD
                            || this.dataFormat == DataFormat.CDAB, slaveId);
                        //ABCD和CDAB属于小端
                    }
                    else
                    {
                        wResult = OperateResult.CreateFailResult("寄存器位写入只支持单个写入");
                    }
                    break;
                default:
                    wResult = OperateResult.CreateFailResult("暂不支持该存储区");
                    break;
            }
            if (wResult.IsSuccess)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败");
            }
        }
        /// <summary>
        /// 写入Short值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="writeValue"></param>
        private void WriteShort(StoreArea storeArea, byte slaveId, ushort start, string writeValue)
        {
            short[] values = ShortLib.GetShortArrayFromString(writeValue);
            switch (storeArea)
            {
                case StoreArea.保持寄存器4x:
                    //写入单寄存器
                    if (values.Length == 1)
                    {
                        wResult = this.modbus.WriteSingleRegisters(start, ByteArrayLib.GetByteArrayFromShort(values[0], this.dataFormat), slaveId);
                    }
                    //写入多寄存器
                    else
                    {
                        wResult = this.modbus.WriteMultipleRegisters(start, ByteArrayLib.GetByteArrayFromShortArray(values, this.dataFormat), slaveId);
                    }
                    break;
                default:
                    wResult = OperateResult.CreateFailResult("暂不支持该存储区");
                    break;
            }
            if (wResult.IsSuccess)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败");
            }
        }
        /// <summary>
        /// 写入UShort值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="writeValue"></param>
        private void WriteUShort(StoreArea storeArea, byte slaveId, ushort start, string writeValue)
        {
            ushort[] values = UShortLib.GetUShortArrayFromString(writeValue);
            switch (storeArea)
            {
                case StoreArea.保持寄存器4x:
                    //写入单寄存器
                    if (values.Length == 1)
                    {
                        wResult = this.modbus.WriteSingleRegisters(start, ByteArrayLib.GetByteArrayFromUShort(values[0], this.dataFormat), slaveId);
                    }
                    //写入多寄存器
                    else
                    {
                        wResult = this.modbus.WriteMultipleRegisters(start, ByteArrayLib.GetByteArrayFromUShortArray(values, this.dataFormat), slaveId);
                    }
                    break;
                default:
                    wResult = OperateResult.CreateFailResult("暂不支持该存储区");
                    break;
            }
            if (wResult.IsSuccess)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败");
            }
        }
        /// <summary>
        /// 写入Int值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="writeValue"></param>
        private void WriteInt(StoreArea storeArea, byte slaveId, ushort start, string writeValue)
        {
            int[] values = IntLib.GetIntArrayFromString(writeValue);
            switch (storeArea)
            {
                case StoreArea.保持寄存器4x:
                    //写入多寄存器
                    wResult = this.modbus.WriteMultipleRegisters(start, ByteArrayLib.GetByteArrayFromIntArray(values, this.dataFormat), slaveId);
                    break;
                default:
                    wResult = OperateResult.CreateFailResult("暂不支持该存储区");
                    break;
            }
            if (wResult.IsSuccess)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败");
            }
        }
        /// <summary>
        /// 写入UInt值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="writeValue"></param>
        private void WriteUInt(StoreArea storeArea, byte slaveId, ushort start, string writeValue)
        {
            uint[] values = UIntLib.GetUIntArrayFromString(writeValue);
            switch (storeArea)
            {
                case StoreArea.保持寄存器4x:
                    //写入多寄存器
                    wResult = this.modbus.WriteMultipleRegisters(start, ByteArrayLib.GetByteArrayFromUIntArray(values, this.dataFormat), slaveId);
                    break;
                default:
                    wResult = OperateResult.CreateFailResult("暂不支持该存储区");
                    break;
            }
            if (wResult.IsSuccess)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败");
            }
        }
        /// <summary>
        /// 写入Float值
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="slaveId"></param>
        /// <param name="start"></param>
        /// <param name="writeValue"></param>
        private void WriteFloat(StoreArea storeArea, byte slaveId, ushort start, string writeValue)
        {
            float[] values = FloatLib.GetFloatArrayFromString(writeValue);
            switch (storeArea)
            {
                case StoreArea.保持寄存器4x:
                    //写入多寄存器
                    wResult = this.modbus.WriteMultipleRegisters(start, ByteArrayLib.GetByteArrayFromFloatArray(values, this.dataFormat), slaveId);
                    break;
                default:
                    wResult = OperateResult.CreateFailResult("暂不支持该存储区");
                    break;
            }
            if (wResult.IsSuccess)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败");
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
                AddLog(1, "请检查是否连接ModbusTCP");
                return false;
            }
            if (!byte.TryParse(this.txt_SlaveId.Text, out slaveId))
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
            dataType = (DataType)Enum.Parse(typeof(DataType), this.cmb_DataType.Text, true);
            storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.Text, true);
            dataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), this.cmb_DataFormat.Text, true);

            return true;
        }


    }
}
