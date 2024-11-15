using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClientV
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;

        public Form1()
        {
            InitializeComponent();
        }

        // Sự kiện khi nhấn nút Send
        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text;
            if (!string.IsNullOrEmpty(message))
            {
                SendMessage(message);  // Gửi tin nhắn
                txtMessage.Clear();    // Xóa TextBox sau khi gửi
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xử lý sự kiện khi chọn một mục trong ListBox
            // Ví dụ, hiển thị thông báo
            MessageBox.Show("Selected item: " + listBox1.SelectedItem.ToString());
        }

        // Sự kiện khi Form được tải
        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectToServer();  // Kết nối tới server khi form được tải
        }

        // Kết nối tới server
        private void ConnectToServer()
        {
            try
            {
                lblStatus.Text = "Connecting...";
                client = new TcpClient("127.0.0.1", 8888);  // Kết nối tới server tại địa chỉ 127.0.0.1 và cổng 8888
                stream = client.GetStream();
                lblStatus.Text = "Connected to server.";  // Hiển thị trạng thái kết nối

                // Khởi động một luồng riêng để nhận tin nhắn
                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Connection failed: " + ex.Message;  // Hiển thị lỗi nếu không kết nối được
            }
        }

        // Gửi tin nhắn tới server
        private void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);  // Gửi tin nhắn đến server
        }

        // Nhận tin nhắn từ server
        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Invoke(new Action(() => lstMessages.Items.Add("Received: " + message)));  // Hiển thị tin nhắn nhận được
                }
                catch
                {
                    Invoke(new Action(() => lblStatus.Text = "Disconnected from server."));  // Hiển thị khi mất kết nối
                    break;
                }
            }

            client.Close();  // Đóng kết nối khi thoát
        }
    }
}

