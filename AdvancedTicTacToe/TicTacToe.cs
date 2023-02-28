using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AdvancedTicTacToe
{
    public partial class TicTacToe : Form
    {
        private bool PCplay;
        private bool gameStarted = false;
        private Button[,] matrixOfButtons;
        private int n;

        private List<Button> allButtons;
        private List<Button> availableButtons;

        private FieldValue humanSign;
        private FieldValue computerSign;

        private PlayerType firstToPlay;
        private PlayerType currentTurn;

        private int waitTime;
        private Random r;

        private string firstPlaysLbl = "Prvi igra -> {0} ";

        public TicTacToe()
        {
            InitializeComponent();
        }

        private enum OrderType {
            hozironta,
            vertical,
            mainDiagonal,
            secondaryDiagonal,
            cube
        }
        private enum FieldValue {
            X,
            O
        }

        private enum PlayerType{
            computer = 0,
            human = 1
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            waitTime = 700;
            r = new Random();

            PCplay = false;

            n = 4;
            matrixOfButtons = new Button[n, n];

            allButtons = panel1.Controls.OfType<Button>().ToList();
            availableButtons = panel1.Controls.OfType<Button>().ToList();

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrixOfButtons[i, j] = allButtons.First(b => b.Name == $"b_{i}_{j}");

            gameStarted = false;

            foreach (Button b in allButtons)
            {
                b.Click += new EventHandler(XorO_Click);
                b.Text = "";
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (gameStarted)
                return;

            RestartGame();
            firstToPlay = (r.Next(0, 2) == 0) ? PlayerType.computer : PlayerType.human;
            currentTurn = firstToPlay;
            AssignPlayersSigns(firstToPlay);
            lbPlayFirst.Text = String.Format(firstPlaysLbl, GetPlayerTypeName());
            gameStarted = true;
            if (firstToPlay == PlayerType.computer)
            {
                PCplay = true;
                PCtimer.Start();
            }
            else
                PCplay = false;

            lbTurn.Text = "{ " + GetPlayerTypeName() + " igra prvi }";
        }

        

        private string GetPlayerTypeName() {
            switch (currentTurn) {
                case PlayerType.human: return ResLibrary.humanLbl;
                case PlayerType.computer: return ResLibrary.computerLbl;
                default: return "";
            }
        }

        private void XorO_Click(object sender, EventArgs e)
        {
            Button fieldButton = (Button) sender;
            if (!gameStarted)
                return;
            if (fieldButton.Text != "")
                return;
            if (currentTurn == PlayerType.computer)
                return;

            fieldButton.Text = humanSign.ToString();
            lbTurn.Text = "{ " + ((!PCplay) ? "Računar" : "Čovek") + " je na potezu }";
            availableButtons.Remove(fieldButton);
            if (CheckForWin(matrixOfButtons, false))
            {
                GameWon();
                wait(waitTime);
                if (new CustomDialog("Pobedio je " + ((PCplay) ? "Računar" : "Čovek") + "\n\nRestartovati igru?", "Pobeda").ShowDialog() == DialogResult.Yes)
                    RestartGame();
                return;
            }
            if (availableButtons.Count == 0)
            {
                GameDraw();
                wait(waitTime);
                if (new CustomDialog(ResLibrary.DrawModalMessage, ResLibrary.DrawLbl).ShowDialog() == DialogResult.Yes)
                    RestartGame();
                return;
            }
            currentTurn = PlayerType.computer;
            PCplay = true;
            PCtimer.Start();
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
                                availableMoves[i, j].Text = (firstToPlay == 0) ? "X" : "O";
                                if (CheckForWin(availableMoves, true))
                                {
                                    availableMoves[i, j].Text = "";
                                    matrixOfButtons[i, j].Text = (firstToPlay == 0) ? "X" : "O";
                                    goto CHECK;
                                }
                                availableMoves[i, j].Text = "";
                            }

                    for (int k = 0; k < n; k++)
                        for (int l = 0; l < n; l++)
                            if (availableMoves[k, l].Text == "")
                            {
                                availableMoves[k, l].Text = (firstToPlay == 0) ? "O" : "X";
                                if (CheckForWin(availableMoves, true))
                                {
                                    availableMoves[k, l].Text = "";
                                    matrixOfButtons[k, l].Text = (firstToPlay == 0) ? "X" : "O";
                                    for (int x = 0; x < availableButtons.Count; x++)
                                        if (availableButtons[x].Name == "b_" + k + "_" + l)
                                            availableButtons.RemoveAt(x);
                                    goto CHECK;
                                }
                                availableMoves[k, l].Text = "";
                            }
                }
                int index = r.Next(availableButtons.Count);
                availableButtons[index].Text = (firstToPlay == 0) ? "X" : "O";
                availableButtons.RemoveAt(index);
            CHECK:
                PCtimer.Stop();
                if (CheckForWin(matrixOfButtons, false))
                {
                    GameWon();
                    wait(waitTime);
                    if (new CustomDialog("Pobedio je " + ((PCplay) ? "Računar" : "Čovek") + "\n\nRestartovati igru?", "Pobeda").ShowDialog() == DialogResult.Yes)
                    {
                        RestartGame();
                    }
                    return;
                }
                if (availableButtons.Count == 0)
                {
                    GameDraw();
                    wait(waitTime);
                    if (new CustomDialog("Nerešeno\n\nRestartovati igru?", "Nerešeno").ShowDialog() == DialogResult.Yes)
                    {
                        RestartGame();
                    }
                    return;
                }
                lbTurn.Text = "{ " + ((!PCplay) ? "Računar" : "Čovek") + " je na potezu }";
                PCplay = false;
                currentTurn = PlayerType.human;
            }
        }
        
        
        private bool CheckForWin(Button[,] matrix, bool clone)
        {
            int i, j, counter = 0;
            int count = 0;
            string matchColumns = "b_{0}_[0-3]";
            string matchRows = "b_[0-3]_{0}";
            string matchMainDiagonal = "b_([0-3])_\\1";
            string matchSecondaryDiagonal = "\b[bB]_(0_3|3_0|1_2|2_1)\b";

            for (i = 0; i < n; i++)
            {
                /*counter = 0;
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
                        ColorWin(OrderType.hozironta, i);
                    return true;
                }*/
               count = allButtons.Where(field => Regex.IsMatch(field.Name, String.Format(matchColumns, i))).Where(field => field.Text == GetCurrentPlayerSign()).Count();
                if (count == n)
                    Console.WriteLine("Pobeda lol horizont");
            }

            for (j = 0; j < n; j++)
            {
                /*counter = 0;
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
                        ColorWin(OrderType.vertical, j);
                    return true;
                }*/
                count = allButtons.Where(field => Regex.IsMatch(field.Name, String.Format(matchRows, j))).Where(field => field.Text == GetCurrentPlayerSign()).Count();
                if (count == n)
                    Console.WriteLine("Pobeda lol vertical");
            }

            /*counter = 0;
            for (i = 0; i < n - 1; i++)
            {
                if (matrixOfButtons[i, i].Text != "" && matrixOfButtons[i, i].Text == matrixOfButtons[(i + 1), (i + 1)].Text)
                {
                    counter++;
                }
                if (counter == n - 1)
                {
                    if (!clone)
                        ColorWin(OrderType.mainDiagonal);
                    return true;
                }
            }*/

            count = allButtons.Where(field => Regex.IsMatch(field.Name,matchMainDiagonal)).Where(field => field.Text == GetCurrentPlayerSign()).Count();
            if (count == n)
                Console.WriteLine("Pobeda lol main diagonal");

            /*counter = 0;
            for (i = 0; i < n - 1; i++)
            {
                if (matrixOfButtons[i, (n - i - 1)].Text != "" && matrixOfButtons[i, (n - i - 1)].Text == matrixOfButtons[(i + 1), (n - i - 2)].Text)
                {
                    counter++;
                }
                if (counter == n - 1)
                {
                    if (!clone)
                        ColorWin(OrderType.secondaryDiagonal);
                    return true;
                }
            }*/

            count = allButtons.Where(field => Regex.IsMatch(field.Name, matchSecondaryDiagonal)).Count();
            if (count == n)
                Console.WriteLine("Pobeda lol secondary diagonal");


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
                            ColorWin(OrderType.cube, i, j);
                        return true;
                    }
                }
            }

            return false;
        }
        private void ColorWin(OrderType orderType, int index = 0, int index2 = 0)
        {

            List<Button> winnerButtons = null;
            switch (orderType)
            {
                case OrderType.hozironta:
                    winnerButtons = allButtons.Where(b => Convert.ToInt32(b.Name.Split('_')[1]) == index).ToList();
                    break;
                case OrderType.vertical:
                    winnerButtons = allButtons.Where(b => Convert.ToInt32(b.Name.Split('_')[2]) == index).ToList();
                    break;
                case OrderType.mainDiagonal:
                    winnerButtons = allButtons.Where(b => Convert.ToInt32(b.Name.Split('_')[1]) == Convert.ToInt32(b.Name.Split('_')[2])).ToList();
                    break;
                case OrderType.secondaryDiagonal:
                    winnerButtons = allButtons.Where(b => (Convert.ToInt32(b.Name.Split('_')[1]) + Convert.ToInt32(b.Name.Split('_')[2]) == (n - 1))).ToList();
                    break;
                case OrderType.cube:
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
            winnerButtons.ForEach(b => b.BackColor = ResLibrary.lightBlue);
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

        private void AssignPlayersSigns(PlayerType firstToPlay)
        {
            humanSign = (firstToPlay == PlayerType.human) ? FieldValue.X : FieldValue.O;
            computerSign = (humanSign == FieldValue.X) ? FieldValue.O : FieldValue.X;
        }

        private string GetCurrentPlayerSign() 
        {
            return (currentTurn == PlayerType.human) ? humanSign.ToString() : computerSign.ToString();
        }

        private void RestartGame()
        {
            ResetAllButtons();
            PCtimer.Stop();
            availableButtons = panel1.Controls.OfType<Button>().ToList();
            gameStarted = false;
            lbTurn.Text = "{ }";
        }

        private void ResetAllButtons()
        {
            allButtons.ForEach(btn =>
            {
                btn.Text = "";
                btn.BackColor = ResLibrary.darkBlue;
                btn.Enabled = true;
            });
        }
        private void GameWon()
        {
            gameStarted = false;
            lbTurn.Text = "{ " + ((PCplay) ? "Računar" : "Čovek") + " je pobedio }";
            allButtons.ForEach(btn => btn.Enabled = false);
        }

        private void GameDraw()
        {
            gameStarted = false;
            lbTurn.Text = ResLibrary.DrawBracketsLbl;
            allButtons.ForEach(btn => btn.Enabled = false);
        }

        private void lbExit_MouseEnter(object sender, EventArgs e)
        {
            lbExit.ForeColor = ResLibrary.red;
        }

        private void lbExit_MouseLeave(object sender, EventArgs e)
        {
            lbExit.ForeColor = ResLibrary.white;
        }
        private void lbMinimize_MouseEnter(object sender, EventArgs e)
        {
            lbMinimize.ForeColor = ResLibrary.red;
        }
        private void lbMinimize_MouseLeave(object sender, EventArgs e)
        {
            lbMinimize.ForeColor = ResLibrary.white;
        }
        private void lbMinimize_MouseEnter_1(object sender, EventArgs e)
        {
            lbMinimize.ForeColor = ResLibrary.red;
        }
        private void lbMinimize_MouseLeave_1(object sender, EventArgs e)
        {
            lbMinimize.ForeColor = ResLibrary.white;
        }
        private void lbExit_MouseEnter_1(object sender, EventArgs e)
        {
            lbExit.ForeColor = ResLibrary.red;
        }
        private void lbExit_MouseLeave_1(object sender, EventArgs e)
        {
            lbExit.ForeColor = ResLibrary.white;
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

        private void lbPlayFirst_Click(object sender, EventArgs e)
        {

        }
    }
}
