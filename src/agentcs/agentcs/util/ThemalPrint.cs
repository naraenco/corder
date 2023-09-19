using System.Text;


namespace agentcs
{
    internal class ThemalPrint
    {
        private System.IO.Ports.SerialPort? serialPort = null;

        public void PrintPin(string portName, string pin, string createdAt)
        {
            serialPort = new System.IO.Ports.SerialPort
            {
                PortName = portName,
                BaudRate = 9600,
                DataBits = 8,
                Parity = System.IO.Ports.Parity.None,
                StopBits = System.IO.Ports.StopBits.One
            };

            try
            {
                serialPort.Open();
                string command = "";

                // 시간 인쇄
                byte[] datetime = Encoding.ASCII.GetBytes(createdAt);
                serialPort.Write(datetime, 0, datetime.Length);

                byte[] setFontSizeCommand = new byte[] { 27, 33, 1 };
                serialPort.Write(setFontSizeCommand, 0, setFontSizeCommand.Length);

                // 핀 인쇄
                command = "\nPIN : " + pin + "\n\n\n\n\n\n\n\n\n\n";
                byte[] data = Encoding.ASCII.GetBytes(command);
                serialPort.Write(data, 0, data.Length);

                // CUT
                byte[] comm = { 0x1B, 0x69 };
                serialPort.Write(comm, 0, comm.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("프린터와 통신 중 오류 발생: " + ex.Message);
            }
            finally
            {
                serialPort.Close();
            }
        }

        public void PrintOrder(string portName, string order)
        {
            serialPort = new System.IO.Ports.SerialPort
            {
                PortName = portName,
                BaudRate = 9600,
                DataBits = 8,
                Parity = System.IO.Ports.Parity.None,
                StopBits = System.IO.Ports.StopBits.One
            };

            try
            {
                serialPort.Open();
                string command = "";

                // 폰트 크기 변경
                byte[] setFontSizeCommand = new byte[] { 27, 33, 0 };
                serialPort.Write(setFontSizeCommand, 0, setFontSizeCommand.Length);

                // 주문 인쇄
                command = "\n" + order + "\n\n\n\n\n\n\n\n\n\n";

                Encoding encode = Encoding.GetEncoding(51949);
                byte[] data = encode.GetBytes(command);
                serialPort.Write(data, 0, data.Length);

                // CUT
                byte[] comm = { 0x1B, 0x69 };
                serialPort.Write(comm, 0, comm.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("프린터와 통신 중 오류 발생: " + ex.Message);
            }
            finally
            {
                serialPort.Close();
            }
        }
    }
}
