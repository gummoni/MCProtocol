using System;
using System.Collections.Concurrent;
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

    public partial class DicEditForm : Form
    {
        public DicEditForm()
        {
            InitializeComponent();
        }

        public DicEditForm(string reg, ConcurrentDictionary<int, bool> dic)
        {
            InitializeComponent();

            var keys = dic.Keys.ToList();
            keys.Sort();
            Text = $"{reg}レジスタ一覧";
            foreach (var key in keys)
            {
                var adr = $"{reg}{key:D5}";
                var ret = dic[key] ? "●" : "－";
                var cmt = MemISDic.GetCommentOnly($"{reg}{key}".Trim());
                textBox1.Text += $"{adr}={ret}\t{cmt}\r\n";
            }
        }

        public DicEditForm(string reg, ConcurrentDictionary<int, ushort> dic)
        {
            InitializeComponent();

            Text = $"{reg}レジスタ一覧";
            var keys = dic.Keys.ToList();
            keys.Sort();
            foreach (var key in keys)
            {
                var adr = $"{reg}{key:D5}";
                var cmt = MemISDic.GetCommentOnly($"{reg}{key}".Trim());
                textBox1.Text += $"{adr}={dic[key]}\t{cmt}\r\n";
            }
        }


    }
}
