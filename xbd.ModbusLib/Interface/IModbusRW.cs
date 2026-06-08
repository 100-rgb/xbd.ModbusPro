using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xbd.DataConvertLib;

namespace xbd.ModbusLib
{
    /// <summary>
    /// Modbus读写接口
    /// </summary>
    public interface IModbusRW
    {
        //读取输出线圈
        OperateResult<bool[]> ReadCoils(ushort start, ushort length, byte slaveId = 1); 
        //读取输入线圈
        OperateResult<bool[]> ReadInputs(ushort start, ushort length, byte slaveId = 1); 
        //读取输出寄存器
        OperateResult<byte[]> ReadHoldingRegisters(ushort start, ushort length, byte slaveId = 1);
        //读取输入寄存器
        OperateResult<byte[]> ReadInputsRegisters(ushort start, ushort length, byte slaveId = 1);

        //写入单个线圈
        OperateResult WriteSingleCoil(ushort start,bool value,byte slaveId = 1);
        //写入单个寄存器
        OperateResult WriteSingleRegisters(ushort start,byte[] value,byte slaveId = 1);
        //写入多个线圈
        OperateResult WriteMultipleCoils(ushort start, bool[] values, byte slaveId = 1);
        //写入多个寄存器
        OperateResult WriteMultipleRegisters(ushort start, byte[] values, byte slaveId = 1);
    }
}
