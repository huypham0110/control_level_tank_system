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

namespace test_1
{
    public partial class Form1 : Form
    {
        string sth;
        string ste;
        string stcontrol;
        string stsetpoint;
        double time1;
        double time2;
        double time3;

        public Form1()
        {
            InitializeComponent();
            String[] Baudrate = { "1200", "2400", "4800", "9600", "115200" };
            cbBaudrate.Items.AddRange(Baudrate);
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (butConnect.Enabled == false)
            {
                Draw(sth, ste, stcontrol, stsetpoint);

                labControl.Text = stcontrol;
                labControl.ForeColor = Color.Blue;

                labE.Text = ste;
                labE.ForeColor = Color.Blue;

                labH.Text = sth;
                labH.ForeColor = Color.Blue;

                labSetpoint.Text = stsetpoint;
                labSetpoint.ForeColor = Color.Red;
            }
        }

        int TickStart1;
        int TickStart2;
        int TickStart3;
        private void Form1_Load(object sender, EventArgs e)
        {
            cbPort.DataSource = SerialPort.GetPortNames();
            cbBaudrate.Text = "9600";
            butConnect.Enabled = true;
            butDisconnect.Enabled = false;

            // GRAPH 1
            GraphPane myPane1 = zedGraphControl1.GraphPane;
            myPane1.Title.Text = "Đáp ứng ngõ ra";
            myPane1.XAxis.Title.Text = "Time (Seconds)";
            myPane1.YAxis.Title.Text = "Height (cm)";

            RollingPointPairList list1 = new RollingPointPairList(60000);
            LineItem Curve1 = myPane1.AddCurve("Height", list1, Color.Blue, SymbolType.None);

            RollingPointPairList list1_ = new RollingPointPairList(60000);
            LineItem Curve1_ = myPane1.AddCurve("Setpoint", list1_, Color.Red, SymbolType.None);

            myPane1.XAxis.Scale.Min = 0;
            myPane1.XAxis.Scale.Max = 10;
            myPane1.YAxis.Scale.Min = -20;
            myPane1.YAxis.Scale.Max = 20;

            zedGraphControl1.AxisChange();

            // GRAPH 2
            GraphPane myPane2 = zedGraphControl2.GraphPane;
            myPane2.Title.Text = "Sai số hệ thống";
            myPane2.XAxis.Title.Text = "Time (Seconds)";
            myPane2.YAxis.Title.Text = "Height (cm)";

            RollingPointPairList list2 = new RollingPointPairList(60000);
            LineItem Curve2 = myPane2.AddCurve("Error", list2, Color.Blue, SymbolType.None);

            myPane2.XAxis.Scale.Min = 0;
            myPane2.XAxis.Scale.Max = 10;
            myPane2.YAxis.Scale.Min = -20;
            myPane2.YAxis.Scale.Max = 20;

            zedGraphControl2.AxisChange();

            // GRAPH 3
            GraphPane myPane3 = zedGraphControl3.GraphPane;
            myPane3.Title.Text = "Tín hiệu điều khiển";
            myPane3.XAxis.Title.Text = "Time (Seconds)";
            myPane3.YAxis.Title.Text = "Amplitude (Pulse)";

            RollingPointPairList list3 = new RollingPointPairList(60000);
            LineItem Curve3 = myPane3.AddCurve("Control", list3, Color.Blue, SymbolType.None);

            myPane3.XAxis.Scale.Min = 0;
            myPane3.XAxis.Scale.Max = 10;
            myPane3.YAxis.Scale.Min = -260;
            myPane3.YAxis.Scale.Max = 260;

            zedGraphControl3.AxisChange();
        }

        private void Draw(string inh, string ine, string incontrol, string insetpoint)
        {
            double _h;
            double _e;
            double _control;
            double _setpoint;

            double.TryParse(inh, out _h);
            double.TryParse(ine, out _e);
            double.TryParse(incontrol, out _control);
            double.TryParse(insetpoint, out _setpoint);

            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)
                return;
            if (zedGraphControl2.GraphPane.CurveList.Count <= 0)
                return;
            if (zedGraphControl3.GraphPane.CurveList.Count <= 0)
                return;

            LineItem curve1 = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            LineItem curve2 = zedGraphControl2.GraphPane.CurveList[0] as LineItem;
            LineItem curve3 = zedGraphControl3.GraphPane.CurveList[0] as LineItem;
            LineItem curve1_ = zedGraphControl1.GraphPane.CurveList[1] as LineItem;

            if (curve1 == null)
                return;
            if (curve2 == null)
                return;
            if (curve3 == null)
                return;
            if (curve1_ == null)
                return;

            IPointListEdit list1 = curve1.Points as IPointListEdit;
            IPointListEdit list2 = curve2.Points as IPointListEdit;
            IPointListEdit list3 = curve3.Points as IPointListEdit;
            IPointListEdit list1_ = curve1_.Points as IPointListEdit;

            if (list1 == null)
                return;
            if (list2 == null)
                return;
            if (list3 == null)
                return;
            if (list1_ == null)
                return;

            list1.Add(time1, _h);
            list2.Add(time2, _e);
            list3.Add(time3, _control);
            list1_.Add(time2, _setpoint);

            Scale xScale1 = zedGraphControl1.GraphPane.XAxis.Scale;
            Scale xScale2 = zedGraphControl2.GraphPane.XAxis.Scale;
            Scale xScale3 = zedGraphControl3.GraphPane.XAxis.Scale;

            if (time1 > xScale1.Max - xScale1.MajorStep)
            {
                xScale1.Max = time1 + xScale1.MajorStep;
                xScale1.Min = xScale1.Max - 10;
            }
            if (time2 > xScale2.Max - xScale2.MajorStep)
            {
                xScale2.Max = time2 + xScale2.MajorStep;
                xScale2.Min = xScale2.Max - 10;
            }
            if (time3 > xScale3.Max - xScale3.MajorStep)
            {
                xScale3.Max = time3 + xScale3.MajorStep;
                xScale3.Min = xScale3.Max - 10;
            }

            zedGraphControl1.AxisChange();
            zedGraphControl2.AxisChange();
            zedGraphControl3.AxisChange();

            zedGraphControl1.Invalidate();
            zedGraphControl2.Invalidate();
            zedGraphControl3.Invalidate();

            time1 = (Environment.TickCount - TickStart1) / 1000.0;
            time2 = (Environment.TickCount - TickStart2) / 1000.0;
            time3 = (Environment.TickCount - TickStart3) / 1000.0;
        }

        private void butConnect_Click(object sender, EventArgs e)
        {
            if(!seCOM.IsOpen)
            {
                seCOM.PortName = cbPort.Text;
                seCOM.BaudRate = Convert.ToInt32(cbBaudrate.Text);
                seCOM.Open();
                butConnect.Enabled = false;
                butDisconnect.Enabled = true;
                butExit.Enabled = false;

                labStatus.Text = "Connected!";
                labStatus.ForeColor = Color.DarkGreen;
            }
        }

        private void butDisconnect_Click(object sender, EventArgs e)
        {
            seCOM.Close();
            butConnect.Enabled = true;
            butDisconnect.Enabled = false;
            butExit.Enabled = true;
            timer1.Stop();

            labStatus.Text = "Disconnected!";
            labStatus.ForeColor = Color.Red;
        }

        private void butSend_Click(object sender, EventArgs e)
        {
            if (!seCOM.IsOpen)
                MessageBox.Show("Chưa kết nối cổng Serial");
            else if (txtKd.Text=="" || txtKi.Text == "" || txtKp.Text == "" || txtSetpoint.Text == "") 
            {
                return;
            }
            else
            {
                String dataSend = txtKp.Text + ',' + txtKi.Text + ',' + txtKd.Text + ',' + txtSetpoint.Text;
                seCOM.Write(dataSend);
                labSend.Text = dataSend;
                if (timer1.Enabled == false)
                {
                    timer1.Start();
                    TickStart1 = Environment.TickCount;
                    TickStart2 = Environment.TickCount;
                    TickStart3 = Environment.TickCount;
                    time1 = 0;
                    time2 = 0;
                    time3 = 0;
                }
            }
        }

        private void seCOM_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //split data receive from serialport
                string[] arrList = seCOM.ReadLine().Split(',');
                sth = arrList[0];
                ste = arrList[1];
                stcontrol = arrList[2];
                stsetpoint = arrList[3];
            }
            catch
            {
                return;
            }
        }

        private void butExit_Click(object sender, EventArgs e)
        {
            seCOM.Close();
            Close();
        }

        private void butStop_Click(object sender, EventArgs e)
        {
            if (!seCOM.IsOpen)
                MessageBox.Show("Chưa kết nối cổng Serial");
            else seCOM.Write("stop");
        }
    }
}
