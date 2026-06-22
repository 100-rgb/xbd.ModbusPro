using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xbd.DataConvertLib;

namespace xbd.ModbusLib
{

    /// <summary>
    /// 功能码的枚举
    /// </summary>
    public enum FunctionCode
    {
        ReadCoils = 0x01,
        ReadInputs = 0x02,
        ReadHoldingRegisters = 0x03,
        ReadInputRegisters = 0x04,
        WriteSingleCoil = 0x05,
        WriteSingleRegister = 0x06,
        WriteMultipleCoils = 0x0F,
        WriteMultipleRegisters = 0x10
    }

    public class ModbusTCP : TCPBase,IModbusRW
    {
        /// <summary>
        /// 默认的单元标识符
        /// </summary>
        public byte SlaveId { get; set; } = 0x01;

        #region 读取输出线圈
        public OperateResult<bool[]> ReadCoils(ushort start, ushort length)
        {
            return ReadCoils(start, length, SlaveId);
        }
        public OperateResult<bool[]> ReadCoils(ushort start, ushort length, byte slaveId = 1)
        {
            //第一步：拼接报文
            byte[] sendCommand = BuildReadMessageFrame(start, length, slaveId, FunctionCode.ReadCoils);
            //第二步：发送报文
            //第三步：接收报文
            var result = SenAndReceive(sendCommand);
            if (result.IsSuccess)
            {
                //第四步：验证报文
                var receive = CheckResponse(result.Content, slaveId, true, UShortLib.GetByteLengthFromBoolLength(length));
                if (receive.IsSuccess)
                {
                    //第五步：解析报文
                    byte[] data = AnalysisResponseMessage(result.Content, true).Content;
                    return OperateResult.CreateSuccessResult<bool[]>(data.Select(c => c == 0x01).Take(length).ToArray());
                }
                else
                {
                    return OperateResult.CreateFailResult<bool[]>(receive);
                }
            }
            else
            {
                return OperateResult.CreateFailResult<bool[]>(result.Message);
            }
        }
        #endregion

        #region 读取输入线圈
        public OperateResult<bool[]> ReadInputs(ushort start, ushort length)
        {
            return ReadInputs(start, length, SlaveId);
        }
        public OperateResult<bool[]> ReadInputs(ushort start, ushort length, byte slaveId = 1)
        {
            //第一步：拼接报文
            byte[] sendCommand = BuildReadMessageFrame(start, length, slaveId, FunctionCode.ReadInputs);
            //第二步：发送报文
            //第三步：接收报文
            var result = SenAndReceive(sendCommand);
            if (result.IsSuccess)
            {
                //第四步：验证报文
                var receive = CheckResponse(result.Content, slaveId, true, UShortLib.GetByteLengthFromBoolLength(length));
                if (receive.IsSuccess)
                {
                    //第五步：解析报文
                    byte[] data = AnalysisResponseMessage(result.Content, true).Content;
                    return OperateResult.CreateSuccessResult<bool[]>(data.Select(c => c == 0x01).Take(length).ToArray());
                }
                else
                {
                    return OperateResult.CreateFailResult<bool[]>(receive);
                }
            }
            else
            {
                return OperateResult.CreateFailResult<bool[]>(result.Message);
            }
        }
        #endregion

        #region 读取保持寄存器
        public OperateResult<byte[]> ReadHoldingRegisters(ushort start, ushort length)
        {
            return ReadHoldingRegisters(start, length, SlaveId);
        }
        public OperateResult<byte[]> ReadHoldingRegisters(ushort start, ushort length, byte slaveId = 1)
        {
            //第一步：拼接报文
            byte[] sendCommand = BuildReadMessageFrame(start, length, slaveId, FunctionCode.ReadHoldingRegisters);
            //第二步：发送报文
            //第三步：接收报文
            var result = SenAndReceive(sendCommand);
            if (result.IsSuccess)
            {
                //第四步：验证报文
                var receive = CheckResponse(result.Content, slaveId, true, (ushort)(length * 2));
                if (receive.IsSuccess)
                {
                    //第五步：解析报文
                    byte[] data = AnalysisResponseMessage(result.Content, false).Content;
                    return OperateResult.CreateSuccessResult<byte[]>(data);
                }
                else
                {
                    return OperateResult.CreateFailResult<byte[]>(receive);
                }
            }
            else
            {
                return OperateResult.CreateFailResult<byte[]>(result.Message);
            }
        }
        #endregion

        #region 读取输入寄存器
        public OperateResult<byte[]> ReadInputsRegisters(ushort start, ushort length)
        {
            return ReadInputsRegisters(start, length, SlaveId);
        }
        public OperateResult<byte[]> ReadInputsRegisters(ushort start, ushort length, byte slaveId = 1)
        {
            //第一步：拼接报文
            byte[] sendCommand = BuildReadMessageFrame(start, length, slaveId, FunctionCode.ReadInputRegisters);
            //第二步：发送报文
            //第三步：接收报文
            var result = SenAndReceive(sendCommand);
            if (result.IsSuccess)
            {
                //第四步：验证报文
                var receive = CheckResponse(result.Content, slaveId, true, (ushort)(length * 2));
                if (receive.IsSuccess)
                {
                    //第五步：解析报文
                    byte[] data = AnalysisResponseMessage(result.Content, false).Content;
                    return OperateResult.CreateSuccessResult<byte[]>(data);
                }
                else
                {
                    return OperateResult.CreateFailResult<byte[]>(receive);
                }
            }
            else
            {
                return OperateResult.CreateFailResult<byte[]>(result.Message);
            }
        }
        #endregion

        #region 写入单线圈

        public OperateResult WriteSingleCoil(ushort start, bool value)
        {
            return WriteSingleCoil(start, value, SlaveId);
        }

        public OperateResult WriteSingleCoil(ushort start, bool value, byte slaveId = 1)
        {
            //第一步：拼接报文
            byte[] sendCommand = BuildWriteMessageFrame(start, value ? new byte[] { 0xFF, 0x00 } : new byte[] { 0x00, 0x00 }, slaveId, FunctionCode.WriteSingleCoil);
            //第二步：发送报文
            //第三步：接收报文
            var result = SenAndReceive(sendCommand);
            if (result.IsSuccess)
            {
                //第四步：验证报文
                var receive = CheckResponse(result.Content, slaveId, false);
                if (receive.IsSuccess)
                {
                    //第五步：解析报文
                    bool compare = ByteArrayLib.GetByteArrayEquals(result.Content, sendCommand);
                    return compare ? OperateResult.CreateSuccessResult() : OperateResult.CreateSuccessResult("发送与返回报文不一致");
                }
                else
                {
                    return OperateResult.CreateFailResult(receive.Message);
                }
            }
            else
            {
                return OperateResult.CreateFailResult(result.Message);
            }
        }

        #endregion

        #region 写入单寄存器

        public OperateResult WriteSingleRegisters(ushort start, byte[] value)
        {
            return WriteSingleRegisters(start, value, SlaveId);
        }

        public OperateResult WriteSingleRegisters(ushort start, byte[] value, byte slaveId = 1)
        {
            if (value == null || value.Length != 2)
            {
                return OperateResult.CreateFailResult("写入字节数组的长度必须是2");
            }

            //第一步：拼接报文
            byte[] sendCommand = BuildWriteMessageFrame(start, value, slaveId, FunctionCode.WriteSingleRegister);
            //第二步：发送报文
            //第三步：接收报文
            var result = SenAndReceive(sendCommand);
            if (result.IsSuccess)
            {
                //第四步：验证报文
                var receive = CheckResponse(result.Content, slaveId, false);
                if (receive.IsSuccess)
                {
                    //第五步：解析报文
                    bool compare = ByteArrayLib.GetByteArrayEquals(result.Content, sendCommand);
                    return compare ? OperateResult.CreateSuccessResult() : OperateResult.CreateSuccessResult("发送与返回报文不一致");
                }
                else
                {
                    return OperateResult.CreateFailResult(receive.Message);
                }
            }
            else
            {
                return OperateResult.CreateFailResult(result.Message);
            }
        }

        #endregion

        #region 写入多输出线圈

        public OperateResult WriteMultipleCoils(ushort start, bool[] values)
        {
            return WriteMultipleCoils(start,values,SlaveId);
        }

        public OperateResult WriteMultipleCoils(ushort start, bool[] values, byte slaveId = 1)
        {

            //第一步：拼接报文
            byte[] sendCommand = BuildWriteMessageFrame(start, ByteArrayLib.GetByteArrayFromBoolArray(values), slaveId, FunctionCode.WriteMultipleCoils,(ushort)values.Length);
            //第二步：发送报文
            //第三步：接收报文
            var result = SenAndReceive(sendCommand);
            if (result.IsSuccess)
            {
                //第四步：验证报文
                var receive = CheckResponse(result.Content, slaveId, false);
                if (receive.IsSuccess)
                {
                    //第五步：解析报文
                    byte[] reqdata = sendCommand.Take(12).ToArray();
                    reqdata[4] = 0x00;
                    reqdata[5] = 0x06;//期望返回的报文

                    bool compare = ByteArrayLib.GetByteArrayEquals(result.Content, reqdata);
                    return compare ? OperateResult.CreateSuccessResult() : OperateResult.CreateSuccessResult("发送与返回报文不正确："+BitConverter.ToString(result.Content));
                }
                else
                {
                    return OperateResult.CreateFailResult(receive.Message);
                }
            }
            else
            {
                return OperateResult.CreateFailResult(result.Message);
            }
        }

        #endregion

        #region 写入多个输出寄存器

        public OperateResult WriteMultipleRegisters(ushort start, byte[] values)
        {
            return WriteMultipleRegisters(start, values, SlaveId);
        }

        public OperateResult WriteMultipleRegisters(ushort start, byte[] values, byte slaveId = 1)
        {
            if (values == null || values.Length == 0)
            {
                return OperateResult.CreateFailResult("写入字节数组不能为空");
            }

            if (values.Length % 2 != 0)
            {
                return OperateResult.CreateFailResult("写入字节数组必须为偶数");
            }

            //第一步：拼接报文
            byte[] sendCommand = BuildWriteMessageFrame(start, values, slaveId, FunctionCode.WriteMultipleRegisters);
            //第二步：发送报文
            //第三步：接收报文
            var result = SenAndReceive(sendCommand);
            if (result.IsSuccess)
            {
                //第四步：验证报文
                var receive = CheckResponse(result.Content, slaveId, false);
                if (receive.IsSuccess)
                {
                    //第五步：解析报文
                    byte[] reqdata = sendCommand.Take(12).ToArray();
                    reqdata[4] = 0x00;
                    reqdata[5] = 0x06;//期望返回的报文

                    bool compare = ByteArrayLib.GetByteArrayEquals(result.Content, reqdata);
                    return compare ? OperateResult.CreateSuccessResult() : OperateResult.CreateSuccessResult("发送与返回报文不正确：" + BitConverter.ToString(result.Content));
                }
                else
                {
                    return OperateResult.CreateFailResult(receive.Message);
                }
            }
            else
            {
                return OperateResult.CreateFailResult(result.Message);
            }
        }

        #endregion

        #region 写入寄存器中的单个值

        /// <summary>
        /// 写入寄存器中的单个值
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <param name="slaveId"></param>
        /// <returns></returns>
        public OperateResult WriteRegisterBit(string address, bool value, bool isLittleEndian = true)
        {
            return WriteRegisterBit(address, value, isLittleEndian, SlaveId);
        }

        public OperateResult WriteRegisterBit(string address, bool value, bool isLittleEndian = true, byte slaveId = 1)
        {
            //address的格式必须是0.10

            if (address.Contains(".") && address.Split('.').Length == 2)
            {
                string[] info = address.Split('.');
                if (ushort.TryParse(info[0], out ushort start) && ushort.TryParse(info[1], out ushort index))
                {
                    if (index >= 0 && index <= 15)
                    {
                        //先读取寄存器的值
                        var rResult = this.ReadHoldingRegisters(start, 1, slaveId);
                        if (rResult.IsSuccess)
                        {
                            //再做转换
                            byte[] wData = rResult.Content;
                            if (isLittleEndian)
                            {
                                int byteIndex = index < 8 ? 1 : 0;
                                wData[byteIndex] = ByteLib.SetbitValue(wData[byteIndex], index % 8, value);
                            }
                            else
                            {
                                int byteIndex = index < 8 ? 0 : 1;
                                wData[byteIndex] = ByteLib.SetbitValue(wData[byteIndex], index % 8, value);
                            }
                            //最后写入
                            return this.WriteSingleRegisters(start, wData, slaveId);
                        }
                        else
                        {
                            return rResult;
                        }
                    }
                    else
                    {
                        return OperateResult.CreateFailResult("位偏移索引必须在0-15之间");
                    }
                }
                else
                {
                    return OperateResult.CreateFailResult("地址格式X.Y必须是有效的整数");
                }
            }
            else
            {
                return OperateResult.CreateFailResult("地址格式必须为X.Y");
            }
        }

        #endregion

        //只读锁
        private static readonly object lockobj = new object();
        //事务处理标识符
        private ushort transactionId=0;
        public ushort TransactionId
        {
            get 
            {
                lock (lockobj)
                {
                    return transactionId == ushort.MaxValue ? (ushort)1 : ++transactionId;
                }
            }            
        }

        #region 通用拼接报文方法

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="slaveId"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        private byte[] BuildReadMessageFrame(ushort start,ushort count,byte slaveId,FunctionCode functionCode)
        {
            //创建一个ByteArray对象
            ByteArray sendCommand = new ByteArray();
            //事务处理标识符
            sendCommand.Add(TransactionId);
            //协议标识符
            sendCommand.Add((ushort)0);
            //长度
            sendCommand.Add((ushort)6);
            //站地址
            sendCommand.Add(slaveId);
            //功能码
            sendCommand.Add((byte)functionCode);
            //起始地址
            sendCommand.Add((ushort)start);
            //数量
            sendCommand.Add((ushort)count);

            return sendCommand.array;
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="value"></param>
        /// <param name="slaveId"></param>
        /// <param name="functionCode"></param>
        /// <param name="coiLength"></param>
        /// <returns></returns>
        private byte[] BuildWriteMessageFrame(ushort start, byte[] value,byte slaveId,FunctionCode functionCode,ushort coiLength = 0)
        {
            //创建一个ByteArray对象
            ByteArray sendCommand = new ByteArray();

            //写入单线圈和写入单寄存器
            if (functionCode == FunctionCode.WriteSingleCoil || functionCode == FunctionCode.WriteSingleRegister)
            {
                //事务处理标识符
                sendCommand.Add(TransactionId);
                //协议标识符
                sendCommand.Add((ushort)0);
                //长度
                sendCommand.Add((ushort)6);
                //站地址
                sendCommand.Add(slaveId);
                //功能码
                sendCommand.Add((byte)functionCode);
                //起始地址
                sendCommand.Add((ushort)start);
                //写入值
                sendCommand.Add(value);
            }
            else if (functionCode==FunctionCode.WriteMultipleCoils||functionCode==FunctionCode.WriteMultipleRegisters)
            {
                //事务处理标识符
                sendCommand.Add(TransactionId);
                //协议标识符
                sendCommand.Add((ushort)0);
                //长度
                sendCommand.Add((ushort)(7 + value.Length));
                //站地址
                sendCommand.Add(slaveId);
                //功能码
                sendCommand.Add((byte)functionCode);
                //起始地址
                sendCommand.Add((ushort)start);
                //线圈或者寄存器数量 
                sendCommand.Add(coiLength==0?(ushort)(value.Length / 2) :coiLength);
                //字节计数
                sendCommand.Add((byte)value.Length);
                //写入数据
                sendCommand.Add(value);
            }
            return sendCommand.array;

        }

        #endregion

        /// <summary>
        /// 通用验证报文方法
        /// </summary>
        /// <param name="response"></param>
        /// <param name="slaveId"></param>
        /// <param name="isRead"></param>
        /// <param name="bytelength"></param>
        /// <returns></returns>
        private OperateResult CheckResponse(byte[] response,byte slaveId,bool isRead,ushort bytelength=0)
        {
            // 验证最小长度（至少7字节：事务ID+协议ID+长度+单元ID）
            if (response.Length < 7)
            {
                return OperateResult.CreateFailResult("返回报文长度不足：" + BitConverter.ToString(response));
            }
            
            // 验证单元标识符是否正确
            if (response[6] != slaveId)
            {
                return OperateResult.CreateFailResult("返回报文单元标识符验证不通过：" + BitConverter.ToString(response));
            }
            
            // 检查是否为异常响应（功能码最高位为1）
            if ((response[7] & 0x80) != 0)
            {
                // 异常响应：长度固定为9字节（事务ID(2)+协议ID(2)+长度(2)+单元ID(1)+功能码(1)+异常码(1)）
                if (response.Length == 9)
                {
                    byte exceptionCode = response[8];
                    string exceptionMsg = GetExceptionMessage(exceptionCode);
                    return OperateResult.CreateFailResult($"设备返回异常响应: {exceptionMsg} (功能码: 0x{response[7]:X2}, 异常码: 0x{exceptionCode:X2})");
                }
                else
                {
                    return OperateResult.CreateFailResult("异常响应长度验证不通过：" + BitConverter.ToString(response));
                }
            }
            
            // 正常响应长度验证
            int reqLength = isRead ? 9 + bytelength : 12;
            if (response.Length == reqLength)
            {
                return OperateResult.CreateSuccessResult();
            }
            else
            {
                return OperateResult.CreateFailResult("返回报文长度验证不通过：" + BitConverter.ToString(response));
            }
        }
        
        private string GetExceptionMessage(byte exceptionCode)
        {
            switch (exceptionCode)
            {
                case 0x01: return "非法功能码 - 设备不支持该操作，请检查服务器是否启用了对应存储区";
                case 0x02: return "非法数据地址 - 请求的地址超出设备支持范围";
                case 0x03: return "非法数据值 - 写入的值超出允许范围";
                case 0x04: return "从站设备故障";
                case 0x05: return "确认 - 设备已接收请求，正在处理";
                case 0x06: return "从站设备忙 - 请稍后重试";
                default: return $"未知异常 (0x{exceptionCode:X2})";
            }
        }
        /// <summary>
        /// 通用解析报文方法
        /// </summary>
        /// <param name="response"></param>
        /// <param name="isBit"></param>
        /// <returns></returns>
        private OperateResult<byte[]> AnalysisResponseMessage(byte[] response,bool isBit)
        {
            //拿到原始数据
            byte[] data = ByteArrayLib.GetByteArrayFromByteArray(response, 9, response.Length - 9);
            if (isBit)
            {
                bool[] values = BitLib.GetBitArrayFromByteArray(data);
                return OperateResult.CreateSuccessResult(values.Select(c=>c==true?(byte)0x01:(byte)0x00).ToArray());
            }
            else
            {
                return OperateResult.CreateSuccessResult(data);
            }
        }

    }
}
