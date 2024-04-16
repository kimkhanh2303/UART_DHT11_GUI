using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using ZedGraph;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string AllData = "";
        string dataOUT;
        public event EventHandler OnUpdateConnection; //uy nhiem ham deligate
        public Form1()
        {
            InitializeComponent();
        }
       
        private void Form1_Load(object sender, EventArgs e) //Khi khoi dong se chay vao lenh nay dau tien
        {

            listcom.DataSource = SerialPort.GetPortNames();
            string[] baud = { "1200", "4800", "9600", "115200" }; //0 1 2 3
            listbaud.DataSource = baud;
            listbaud.SelectedIndex = 3;


            // Khởi tạo bieu đc
            GraphPane myPanne = zedGraphControl1.GraphPane;
            myPanne.Title.Text = "Dữ liệu cần hiển thị";
            myPanne.YAxis.Title.Text = "Giá trị";
            myPanne.XAxis.Title.Text = "Thời gian";

            RollingPointPairList list = new RollingPointPairList(500000);
            RollingPointPairList list2 = new RollingPointPairList(500000);

            LineItem line = myPanne.AddCurve("Nhiệt độ", list, Color.Red, SymbolType.Diamond);
            LineItem line2 = myPanne.AddCurve("Độ ẩm", list2, Color.Blue, SymbolType.Star);

            myPanne.XAxis.Scale.Min = 0; //giá trị nhỏ nhat
            myPanne.XAxis.Scale.Max = 20; //gia tri lon
            myPanne.XAxis.Scale.MinorStep = 1;
            myPanne.XAxis.Scale.MajorStep = 2;

            myPanne.YAxis.Scale.Min = 20; //giá tri nho nhất
            myPanne.YAxis.Scale.Max = 39; //gia tri lon
            myPanne.YAxis.Scale.MinorStep = 1;
            myPanne.YAxis.Scale.MajorStep = 2;

            zedGraphControl1.AxisChange();
        }
        int tong = 0;
        public void draw(double line, double line2) //do ve 2 cai bieu do nen can truyen 2 gia trị
        {
            LineItem duongline1 = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            LineItem duongline2 = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            if (duongline1 == null)
                return;
            if (duongline2 == null)
                return;
            //danh sach bieu thi duong cong do thi
            IPointListEdit list = duongline1.Points as IPointListEdit;
            IPointListEdit list2 = duongline2.Points as IPointListEdit;
            if (list == null)           
                return;
            if (list2 == null)
                return;
            list.Add(tong, line);
            list2.Add(tong, line2);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            tong += 2;
        } 
        private void button1_Click(object sender, EventArgs e)
        {
            if (listcom.Text == "")
            {
                MessageBox.Show("Bạn chưa chọn cổng COM", "Thông Báo");
            }
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
                button1.Text = "Kết nối";
            }
            else if(serialPort1.IsOpen == false)
            {
                try
                {
                    serialPort1.PortName = listcom.Text;
                    serialPort1.BaudRate = Convert.ToInt32(listbaud.Text);
                    serialPort1.Open();
                  
                }
                catch (Exception err) 
                {
                    MessageBox.Show(err.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
                
        }

        string data = "";
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           AllData = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(Showdata));
//Invoke(new MethodInvoker(() => draw(int.Parse(data)/100, int.Parse(data)/100)));
        }

        private void Showdata(object sender, EventArgs e)
        {

            nhandulieu.AppendText(AllData + Environment.NewLine);

        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true) 
            {
                serialPort1.Close();
                
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }


        private void listcom_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = listcom.Text;
        }


        private void timer1_Tick_1(object sender, EventArgs e) //timer cu moi 3s se kiem tra status co dang cam COM hay khong
        {
            if (this.OnUpdateConnection == null)
            {
                this.OnUpdateConnection += UpdateConnection; //su kien nay gan vao phuong thuc UpdateConnection

            }
            this.OnUpdateConnection(this, EventArgs.Empty);
        }
        private void UpdateConnection(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                C.Text = "ON";
                C.ForeColor = Color.Green; ;
             
            }
            else
            {
                C.Text = "OFF";
                C.ForeColor = Color.Red;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Write("A");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Write("B");
            }
        }

      



        //   private void button5_Click(object sender, EventArgs e)
        //   {
        //       if (serialPort1.IsOpen)
        //       {
        //           dataOUT = guidulieu.Text;
        //           serialPort1.Write(dataOUT);
        //       }
        //    }
    }
}
