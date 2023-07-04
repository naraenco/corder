using System.Text;


namespace agentcs
{
    internal class ThemalPrint
    {
        private System.IO.Ports.SerialPort? serialPort = null;

        public void PrintPin(string portName, string pin, string createdAt, int width = 3, int height = 3)
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
                // 프린터 연결
                serialPort.Open();

                // 시간 인쇄
                byte[] data = Encoding.ASCII.GetBytes(createdAt);
                serialPort.Write(data, 0, data.Length);

                // 폰트 크기 변경
                //command = "\x1B!%c";
                string command = String.Format("\x1B!%c", ((width - 1) << 4) | (height - 1));
                data = Encoding.ASCII.GetBytes(command);
                serialPort.Write(data, 0, data.Length);

                // 핀 인쇄
                command = "\nPIN : " + pin + "\n\n\n\n\n\n\n\n\n\n";
                data = Encoding.ASCII.GetBytes(command);
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
