using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            this.cmb_DataFormat.SelectedIndex = 1;
            //初始化存储区
            this.cmb_StoreArea.Items.AddRange(Enum.GetNames(typeof(StoreArea)));
            this.cmb_StoreArea.SelectedIndex = 0;

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
    }
}
