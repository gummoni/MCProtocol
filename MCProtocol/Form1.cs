using System.Collections.Generic;
using System.Drawing;

namespace MCProtocol
{
    public partial class Form1 : Form
    {
        readonly PLCDevice PLC = new(5000);

        public Form1()
        {
            InitializeComponent();
        }
    }
}