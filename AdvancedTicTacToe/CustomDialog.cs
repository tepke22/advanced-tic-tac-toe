using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvancedTicTacToe
{
    public partial class CustomDialog : Form
    {
        private Color red;
        public CustomDialog(string message, string title)
        {
            InitializeComponent();
            red = Color.FromArgb(224, 60, 67);
            lbMessage.Text = message;
            lbTitle.Text = title;
        }
        private void lbMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lbExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbExit_MouseEnter(object sender, EventArgs e)
        {
            lbExit.ForeColor = red;
        }

        private void lbExit_MouseLeave(object sender, EventArgs e)
        {
            lbExit.ForeColor = Color.White;
        }

        private void lbMinimize_MouseEnter(object sender, EventArgs e)
        {
            lbMinimize.ForeColor = red;
        }

        private void lbMinimize_MouseLeave(object sender, EventArgs e)
        {
            lbMinimize.ForeColor = Color.White;
        }

        private Point mouseLocation;
        private void lbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = e.Location;
        }

        private void lbTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.Location.X - mouseLocation.X;
                int dy = e.Location.Y - mouseLocation.Y;
                this.Location = new Point(this.Location.X + dx, this.Location.Y + dy);
            }
        }
    }
}
