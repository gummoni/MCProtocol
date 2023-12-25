using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCProtocol
{
    public partial class FlagForm : Form
    {
        readonly PLCDevice? PLC;
        private int savedScrollPosition = 0;
        int DMAddress => int.Parse(DMAddressTextBox.Text);
        ushort DMValue => ushort.Parse(DMValueTextBox.Text);

        public FlagForm()
        {
            InitializeComponent();
        }

        public FlagForm(PLCDevice plc)
        {
            InitializeComponent();
            PLC = plc;
        }

        private void DMReadButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (PLC == null) return;
                var address = DMAddress;
                var value = PLC.DM[address];
                DMAddressTextBox.Text = $"{value}";
            }
            catch
            {
                MessageBox.Show("DMアドレスに数値を入力してください。");
            }
        }

        private void DMWriteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (PLC == null) return;
                var address = DMAddress;
                var value = DMValue;
                PLC.DM[address] = value;
            }
            catch
            {
                MessageBox.Show("DMアドレスまたは値に数値を入力してください。");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
