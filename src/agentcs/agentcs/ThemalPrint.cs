using System.Text;


namespace agentcs
{
    internal class ThemalPrint
    {
        private System.IO.Ports.SerialPort serialPort = null;

        public void PrintPin(string portName, string pin, string createdAt, int width = 3, int height = 3)
        {
            serialPort = new System.IO.Ports.SerialPort();

            serialPort.PortName = portName;
            serialPort.BaudRate = 9600;
            serialPort.DataBits = 8;
            serialPort.Parity = System.IO.Ports.Parity.None;
            serialPort.StopBits = System.IO.Ports.StopBits.One;

            try
            {
                // 프린터 연결
                serialPort.Open();

                // 시간 인쇄
                string command = createdAt;
                byte[] data = Encoding.ASCII.GetBytes(command);
                serialPort.Write(data, 0, data.Length);

                // 폰트 크기 변경
                command = "\x1B!%c";
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
