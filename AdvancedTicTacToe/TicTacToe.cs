using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

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

        private enum OrderType
        {
            horizontal,
            vertical,
            mainDiagonal,
            secondaryDiagonal,
            cube
        }
        private enum FieldValue
        {
            X,
            O
        }

        private enum PlayerType
        {
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


        /* Button click event */

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

        private void XorO_Click(object sender, EventArgs e)
        {
            Button fieldButton = (Button)sender;
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

        /* !!! Button click event !!! */

        /* Computer logic */

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

        /* !!! Computer logic !!! */

        /* Checking for win function */

        private bool CheckForWin(Button[,] matrix, bool clone)
        {
            int i = 0, j = 0;

            foreach (OrderType order in Enum.GetValues(typeof(OrderType)))
            {
                if (CheckWin(order,clone, ref i, ref j))
                {
                    Console.WriteLine("Pobeda lol " + order.ToString());
                    if (!clone)
                        ColorWin(order, i, j);
                    return true;
                }
            }
            return false;
        }

        private void ColorWin(OrderType orderType, int index = 0, int index2 = 0)
        {

            List<Button> winnerButtons = null;
            switch (orderType)
            {
                case OrderType.horizontal:
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

        private bool CheckWin(OrderType order, bool clone, ref int index, ref int index2)
        {
            int i = 0, j = 0;

            string matchColumns = "b_{0}_[0-3]";
            string matchRows = "b_[0-3]_{0}";
            string matchMainDiagonal = "b_([0-3])_\\1";
            string matchSecondaryDiagonal = "\\b[bB]_(0_3|3_0|1_2|2_1)\\b";

            string currentPlayerSign = GetCurrentPlayerSign();
            if (clone)
                currentPlayerSign = GetOppositePlayerSign();

            switch (order)
            {
                case OrderType.horizontal:
                    {
                        for (i = 0; i < n; i++)
                            if (allButtons.Where(field => Regex.IsMatch(field.Name, String.Format(matchColumns, i))).
                                           Where(field => field.Text == currentPlayerSign).
                                           Count().
                                           Equals(n))
                            {
                                index = i;
                                return true;
                            }
                        return false;
                    }
                case OrderType.vertical:
                    {
                        for (i = 0; i < n; i++)
                            if (allButtons.Where(field => Regex.IsMatch(field.Name, String.Format(matchRows, i))).
                                           Where(field => field.Text == currentPlayerSign).
                                           Count().
                                           Equals(n))
                            {
                                index = i;
                                return true;
                            }
                        return false;
                    }
                case OrderType.mainDiagonal:
                    {
                        if (allButtons.Where(field => Regex.IsMatch(field.Name, matchMainDiagonal)).
                                       Where(field => field.Text == currentPlayerSign).
                                       Count().
                                       Equals(n))
                            return true;
                        return false;
                    }
                case OrderType.secondaryDiagonal:
                    {

                        if (allButtons.Where(field => Regex.IsMatch(field.Name, matchSecondaryDiagonal)).
                                       Where(field => field.Text == currentPlayerSign).
                                       Count().
                                       Equals(n))
                            return true;
                        return false;
                    }
                case OrderType.cube:
                    {
                        for (i = 0; i < n - 1; i++)
                            for (j = 0; j < n - 1; j++)
                                if (matrixOfButtons[i, j].Text != "" && matrixOfButtons[i, j].Text == matrixOfButtons[i, (j + 1)].Text &&
                                    matrixOfButtons[i, j].Text == matrixOfButtons[(i + 1), j].Text &&
                                    matrixOfButtons[i, j].Text == matrixOfButtons[(i + 1), (j + 1)].Text)
                                {
                                    index = i;
                                    index2 = j;
                                    return true;
                                }
                        return false;
                    }
                default:
                    return false;

            }
        }

        /* !!! Checking for win function !!! */


        /* Game states functions */

        private void RestartGame()
        {
            ResetAllButtons();
            PCtimer.Stop();
            availableButtons = panel1.Controls.OfType<Button>().ToList();
            gameStarted = false;
            lbTurn.Text = "{ }";
            lbPlayFirst.Text = String.Format(firstPlaysLbl, "");
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

        /* !!! Game states functions !!! */

        /* Helper functions */

        private string GetPlayerTypeName()
        {
            switch (currentTurn)
            {
                case PlayerType.human: return ResLibrary.humanLbl;
                case PlayerType.computer: return ResLibrary.computerLbl;
                default: return "";
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

        private string GetOppositePlayerSign()
        {
            return (currentTurn == PlayerType.human) ? computerSign.ToString() : humanSign.ToString();
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

        /* !!! Helper functions !!! */


        /* Label events */

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

        /* !!! Label events !!! */
    }
}
