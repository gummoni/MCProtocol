using System.Collections.Concurrent;

namespace MCProtocol
{
    public partial class FlagForm : Form
    {
        readonly PLCDevice? PLC;
        private int savedScrollPosition = 0;
        int DMAddress => int.Parse(DMAddressTextBox.Text);
        uint DMValue => uint.Parse(DMValueTextBox.Text);

        public FlagForm()
        {
            InitializeComponent();
        }

        public FlagForm(PLCDevice plc)
        {
            InitializeComponent();
            PLC = plc;
            ListUpdate();
        }

        private void DMReadButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (PLC == null) return;
                var address = DMAddress;
                var value = PLC.DM.TryGetValue(address, out ushort _v) ? _v : 0;
                DMValueTextBox.Text = $"{value}";
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
                PLC.WriteDM2(address, value);
            }
            catch
            {
                MessageBox.Show("DMアドレスまたは値に数値を入力してください。");
            }
        }

        private void ReloadButton_Click(object sender, EventArgs e)
        {
            ListUpdate();
        }

        void ListUpdate()
        {
            if (PLC == null) return;
            ListUpdate("R", PLC.R, RRegCheckedListBox);
            ListUpdate("MR", PLC.MR, MRRegCheckedListBox);
        }

        static void ListUpdate(string reg, ConcurrentDictionary<int, bool> dic, CheckedListBox listBox)
        {
            listBox.SuspendLayout();
            listBox.Visible = false;
            listBox.Items.Clear();
            var keys = dic.Keys.ToList();
            keys.Sort();

            foreach (var key in keys)
            {
                var adr = $"{reg}{key:D5}";
                var ret = dic[key];
                var cmt = MemISDic.GetCommentOnly($"{reg}{key}".Trim());
                listBox.Items.Add($"{adr}:{cmt}", ret);
            }

            listBox.Visible = true;
            listBox.ResumeLayout();
        }

        private void RRegCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (PLC == null) return;
            if (sender is CheckedListBox listBox)
            {
                if (listBox.SelectedItem is string key)
                {
                    try
                    {
                        var val = e.NewValue == CheckState.Checked;
                        PLC.WriteR(int.Parse(key.Split(':')[0][2..]), val);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void MRRegCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (PLC == null) return;
            if (sender is CheckedListBox listBox)
            {
                if (listBox.SelectedItem is string key)
                {
                    try
                    {
                        var val = e.NewValue == CheckState.Checked;
                        PLC.WriteMR(int.Parse(key.Split(':')[0][2..]), val);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
