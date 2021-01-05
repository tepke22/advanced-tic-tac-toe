using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AdvancedTicTacToe
{
    public partial class TicTacToe : Form
    {
        public TicTacToe()
        {
            InitializeComponent();
        }

        private bool PCplay;
        private bool playing = false;
        private Button[,] matrixOfButtons;
        private int n;

        private List<Button> allButtons;
        private List<Button> availableButtons;

        private Color darkBlue;
        private Color lightBlue;
        private Color red;

        private int first;
        private int waitTime;
        private Random r;

        private void Form1_Load(object sender, EventArgs e)
        {
            darkBlue = Color.FromArgb(7, 23, 49);
            lightBlue = Color.FromArgb(76, 153, 203);
            red = Color.FromArgb(224, 60, 67);
            waitTime = 700;
            r = new Random();

            PCplay = false;

            n = 4;
            matrixOfButtons = new Button[n, n];
            allButtons = panel1.Controls.OfType<Button>().ToList();
            availableButtons = panel1.Controls.OfType<Button>().ToList();

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    matrixOfButtons[i, j] = allButtons.First(b => b.Name == "b_" + i + "_" + j);

            playing = false;

            foreach (Button b in allButtons)
            {
                b.Click += new EventHandler(XorO_Click);
                b.Text = "";
            }


        }

        private void XorO_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Text == "" && playing && !PCplay)
            {

                b.Text = (first == 0) ? "O" : "X";
                lbTurn.Text = "{ " + ((!PCplay) ? "Računar" : "Čovek") + " je na potezu }";
                availableButtons.Remove(b);
                if (CheckForWin(matrixOfButtons, false))
                {
                    playing = false;
                    lbTurn.Text = "{ " + ((PCplay) ? "Računar" : "Čovek") + " je pobedio }";
                    wait(waitTime);
                    if (new CustomDialog("Pobedio je " + ((PCplay) ? "Računar" : "Čovek") + "\n\nRestartovati igru?", "Pobeda").ShowDialog() == DialogResult.Yes)
                    {
                        RestartGame();
                    }
                    return;
                }
                if (availableButtons.Count == 0)
                {
                    lbTurn.Text = "{ Nerešeno }";
                    playing = false;
                    allButtons.ForEach(btn => btn.Enabled = false);
                    wait(waitTime);
                    if (new CustomDialog("Nerešeno\n\nRestartovati igru?", "Nerešeno").ShowDialog() == DialogResult.Yes)
                    {
                        RestartGame();
                    }
                    return;
                }
                PCplay = true;
                PCtimer.Start();
            }
        }
        private void PCtimer_Tick(object sender, EventArgs e)
        {
            if (PCplay && availableButtons.Count > 0)
            {
                if (true)
                {
                    Button[,] availableMoves = matrixOfButtons.Clone() as Button[,];

                    for (int i = 0; i < n; i++)
                        for (int j = 0; j < n; j++)
                            if (availableMoves[i, j].Text == "")
                            {
                                availableMoves[i, j].Text = (first == 0) ? "X" : "O";
                                if (CheckForWin(availableMoves, true))
                                {
                                    availableMoves[i, j].Text = "";
                                    matrixOfButtons[i, j].Text = (first == 0) ? "X" : "O";
                                    goto CHECK;
                                }
                                availableMoves[i, j].Text = "";
                            }

                    for (int k = 0; k < n; k++)
                        for (int l = 0; l < n; l++)
                            if (availableMoves[k, l].Text == "")
                            {
                                availableMoves[k, l].Text = (first == 0) ? "O" : "X";
                                if (CheckForWin(availableMoves, true))
                                {
                                    availableMoves[k, l].Text = "";
                                    matrixOfButtons[k, l].Text = (first == 0) ? "X" : "O";
                                    for (int x = 0; x < availableButtons.Count; x++)
                                        if (availableButtons[x].Name == "b_" + k + "_" + l)
                                            availableButtons.RemoveAt(x);
                                    goto CHECK;
                                }
                                availableMoves[k, l].Text = "";
                            }
                }
                int index = r.Next(availableButtons.Count);
                availableButtons[index].Text = (first == 0) ? "X" : "O";
                availableButtons.RemoveAt(index);
            CHECK:
                PCtimer.Stop();
                if (CheckForWin(matrixOfButtons, false))
                {
                    playing = false;
                    lbTurn.Text = "{ " + ((PCplay) ? "Računar" : "Čovek") + " je pobedio }";
                    wait(waitTime);
                    if (new CustomDialog("Pobedio je " + ((PCplay) ? "Računar" : "Čovek") + "\n\nRestartovati igru?", "Pobeda").ShowDialog() == DialogResult.Yes)
                    {
                        RestartGame();
                    }
                    return;
                }
                if (availableButtons.Count == 0)
                {
                    lbTurn.Text = "{ Nerešeno }";
                    playing = false;
                    allButtons.ForEach(btn => btn.Enabled = false);
                    wait(waitTime);
                    if (new CustomDialog("Nerešeno\n\nRestartovati igru?", "Nerešeno").ShowDialog() == DialogResult.Yes)
                    {
                        RestartGame();
                    }
                    return;
                }
                lbTurn.Text = "{ " + ((!PCplay) ? "Računar" : "Čovek") + " je na potezu }";
                PCplay = false;
            }
        }
        public void wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!playing)
            {
                RestartGame();
                first = r.Next(0, 2);
                lbPlayFirst.Text = "Prvi igra -> " + first;
                playing = true;
                if (first == 0)
                {
                    PCplay = true;
                    PCtimer.Start();
                }
                else
                    PCplay = false;

                lbTurn.Text = "{ " + ((first == 0) ? "Računar" : "Čovek") + " igra prvi }";
            }
        }
        private bool CheckForWin(Button[,] matrix, bool clone)
        {
            int i, j, counter = 0;

            for (i = 0; i < n; i++)
            {
                counter = 0;
                for (j = 0; j < n - 1; j++)
                {
                    if (matrixOfButtons[i, j].Text != "" && matrixOfButtons[i, (j + 1)].Text != "" && (matrixOfButtons[i, j].Text == matrixOfButtons[i, (j + 1)].Text))
                    {
                        counter++;
                    }
                }
                if (counter == n - 1)
                {
                    if (!clone)
                        ColorWin("horizontal", i);
                    return true;
                }
            }

            for (j = 0; j < n; j++)
            {
                counter = 0;
                for (i = 0; i < n - 1; i++)
                {
                    if (matrixOfButtons[i, j].Text != "" && matrixOfButtons[(i + 1), j].Text != "" && (matrixOfButtons[i, j].Text == matrixOfButtons[(i + 1), j].Text))
                    {
                        counter++;
                    }
                }
                if (counter == n - 1)
                {
                    if (!clone)
                        ColorWin("vertical", j);
                    return true;
                }
            }

            counter = 0;
            for (i = 0; i < n - 1; i++)
            {
                if (matrixOfButtons[i, i].Text != "" && matrixOfButtons[i, i].Text == matrixOfButtons[(i + 1), (i + 1)].Text)
                {
                    counter++;
                }
                if (counter == n - 1)
                {
                    if (!clone)
                        ColorWin("main_diagonal");
                    return true;
                }
            }

            counter = 0;
            for (i = 0; i < n - 1; i++)
            {
                if (matrixOfButtons[i, (n - i - 1)].Text != "" && matrixOfButtons[i, (n - i - 1)].Text == matrixOfButtons[(i + 1), (n - i - 2)].Text)
                {
                    counter++;
                }
                if (counter == n - 1)
                {
                    if (!clone)
                        ColorWin("secondary_diagonal");
                    return true;
                }
            }


            for (i = 0; i < n - 1; i++)
            {
                counter = 0;
                for (j = 0; j < n - 1; j++)
                {
                    if (matrixOfButtons[i, j].Text != "" && matrixOfButtons[i, j].Text == matrixOfButtons[i, (j + 1)].Text &&
                       matrixOfButtons[i, j].Text == matrixOfButtons[(i + 1), j].Text &&
                       matrixOfButtons[i, j].Text == matrixOfButtons[(i + 1), (j + 1)].Text)
                    {
                        counter++;
                    }
                    if (counter == 1)
                    {
                        if (!clone)
                            ColorWin("cube", i, j);
                        return true;
                    }
                }
            }

            return false;
        }
        private void ColorWin(string direction, int index = 0, int index2 = 0)
        {

            List<Button> winnerButtons = null;
            switch (direction)
            {
                case "horizontal":
                    winnerButtons = allButtons.Where(b => Convert.ToInt32(b.Name.Split('_')[1]) == index).ToList();
                    break;
                case "vertical":
                    winnerButtons = allButtons.Where(b => Convert.ToInt32(b.Name.Split('_')[2]) == index).ToList();
                    break;
                case "main_diagonal":
                    winnerButtons = allButtons.Where(b => Convert.ToInt32(b.Name.Split('_')[1]) == Convert.ToInt32(b.Name.Split('_')[2])).ToList();
                    break;
                case "secondary_diagonal":
                    winnerButtons = allButtons.Where(b => (Convert.ToInt32(b.Name.Split('_')[1]) + Convert.ToInt32(b.Name.Split('_')[2]) == (n - 1))).ToList();
                    break;
                case "cube":
                    winnerButtons = allButtons.Where(b =>
                    {
                        if (b.Name == "b_" + index + "_" + index2 ||
                        b.Name == "b_" + index + "_" + (index2 + 1) ||
                        b.Name == "b_" + (index + 1) + "_" + index2 ||
                        b.Name == "b_" + (index + 1) + "_" + (index2 + 1))
                            return true;
                        return false;
                    }).ToList();
                    break;

            }
            allButtons.ForEach(b => b.Enabled = false);
            winnerButtons.ForEach(b => b.BackColor = lightBlue);
        }
        private void RestartGame()
        {
            allButtons.ForEach(btn =>
            {
                btn.Text = "";
                btn.BackColor = darkBlue;
                btn.Enabled = true;
            });
            PCtimer.Stop();
            availableButtons = panel1.Controls.OfType<Button>().ToList();
            playing = false;
            lbPlayFirst.Text = "Prvi igra -> ";
            lbTurn.Text = "{ }";
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
        private void lbMinimize_MouseEnter_1(object sender, EventArgs e)
        {
            lbMinimize.ForeColor = red;
        }
        private void lbMinimize_MouseLeave_1(object sender, EventArgs e)
        {
            lbMinimize.ForeColor = Color.White;
        }
        private void lbExit_MouseEnter_1(object sender, EventArgs e)
        {
            lbExit.ForeColor = red;
        }
        private void lbExit_MouseLeave_1(object sender, EventArgs e)
        {
            lbExit.ForeColor = Color.White;
        }

        private void lbMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lbExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
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
