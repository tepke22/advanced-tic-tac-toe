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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace AdvancedTicTacToe
{
    public partial class TicTacToe : Form
    {
        private bool gameStarted = false;
        private Button[,] matrixOfButtons;
        private int n;

        private List<Button> allButtons;
        private List<Button> availableButtons;

        private FieldValue humanSign;
        private FieldValue computerSign;

        private PlayerType firstToPlay;
        private PlayerType currentTurn;

        private PlayerType maximizingPlayer;
        private PlayerType minimizingPlayer;

        private int waitTime;
        private Random r;

        private readonly string firstPlaysLbl = "Prvi igra -> {0} ";

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

        private enum Score
        {
            X = 1,
            O = -1,
            tie = 0
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            waitTime = 500;
            r = new Random();

            n = 4;
            matrixOfButtons = new Button[n, n];

            allButtons = panel1.Controls.OfType<Button>().ToList();
            allButtons.ForEach(btn => { btn.Click += new EventHandler(XorO_Click); btn.Text = ""; });
            GetAvailableButtons();

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrixOfButtons[i, j] = allButtons.First(b => b.Name == $"b_{i}_{j}");

            gameStarted = false;
        }

        private void PrintMatrix(List<Button> cloned)
        {
            Console.WriteLine();
            //Console.WriteLine(cloned.Count.ToString());

            for (int i = 0; i < cloned.Count; i++)
            {
                if (i % 4 == 0 && i != 0)
                {
                    Console.WriteLine();
                }
                Console.Write(((cloned[i].Text == "") ? "-" : cloned[i].Text) + "   ");
            }
            Console.WriteLine();
            Console.WriteLine();
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
            lbPlayFirst.Text = String.Format(firstPlaysLbl, GetCurrentPlayerTypeName());
            gameStarted = true;

            maximizingPlayer = firstToPlay;
            minimizingPlayer = (firstToPlay == PlayerType.computer) ? PlayerType.human : PlayerType.computer;

            if (firstToPlay == PlayerType.computer)
                PCtimer.Start();

            lbTurn.Text = "{ " + GetCurrentPlayerTypeName() + " igra prvi }";
        }

        private void XorO_Click(object sender, EventArgs e)
        {
            Button buttonClicked = (Button)sender;
            if (!gameStarted)
                return;
            if (buttonClicked.Text != "")
                return;
            if (currentTurn == PlayerType.computer)
                return;

            buttonClicked.Text = humanSign.ToString();
            GetAvailableButtons();
            if (CheckForWin(allButtons, true, GetCurrentPlayerSign()))
            {
                GameWon();
                if (new CustomDialog("Pobedio je " + GetCurrentPlayerTypeName() + "\n\nRestartovati igru?", "Pobeda").ShowDialog() == DialogResult.Yes)
                    RestartGame();
                return;
            }
            GetAvailableButtons();
            if (availableButtons.Count == 0)
            {
                GameDraw();
                if (new CustomDialog(ResLibrary.DrawModalMessage, ResLibrary.DrawLbl).ShowDialog() == DialogResult.Yes)
                    RestartGame();
                return;
            }
            currentTurn = PlayerType.computer;
            lbTurn.Text = "{ " + GetCurrentPlayerTypeName() + " je na potezu }";
            PCtimer.Start();
        }

        /* !!! Button click event !!! */

        /* Computer logic */

        private void PCtimer_Tick(object sender, EventArgs e)
        {
            GetAvailableButtons();
            if (availableButtons.Count <= 0)
                return;

            Button[,] availableMoves = matrixOfButtons.Clone() as Button[,];
            List<Button> clonedAllButtons = new List<Button>();
            List<Button> tempAvailableButtons = new List<Button>();

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    clonedAllButtons.Add(availableMoves[i, j]);
                    if (availableMoves[i, j].Text == "")
                        tempAvailableButtons.Add(availableMoves[i, j]);
                }

            int[] randomSteps = { 16, 15, 14, 13 };

            if (Array.Exists(randomSteps, element => element == tempAvailableButtons.Count()))
                goto RANDOM;

            int bestScore = 0;
            bool isPlayerMaximizing = firstToPlay == PlayerType.computer;
            if (isPlayerMaximizing)
                bestScore = int.MinValue;
            else
                bestScore = int.MaxValue;

            List<int> scores = new List<int>();

            Button bestMove = null;
            int score = 0;
            Console.WriteLine();
            Console.WriteLine();
            foreach (Button tempAvailableMove in tempAvailableButtons)
            {
                clonedAllButtons.First(btn => btn.Name.Equals(tempAvailableMove.Name)).Text = GetCurrentPlayerSign();
                score = Minimax(clonedAllButtons, 0, !isPlayerMaximizing);
                scores.Add(score);
                clonedAllButtons.First(btn => btn.Name.Equals(tempAvailableMove.Name)).Text = "";
                Console.WriteLine(tempAvailableMove.Name + ": " + score.ToString());
                if (isPlayerMaximizing)
                {
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = tempAvailableMove;
                    }
                }
                else
                {
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestMove = tempAvailableMove;
                    }
                }

            }
            currentTurn = PlayerType.computer;
            if (scores.Where(s => s.Equals(0)).Count() == scores.Count()) 
            {
                goto RANDOM;
            }
            if (bestMove != null)
            {
                allButtons.First(btn => btn.Name.Equals(bestMove.Name)).Text = GetCurrentPlayerSign();
                goto CHECK;
            }

        RANDOM:
            int index = r.Next(tempAvailableButtons.Count());
            allButtons.First(btn => btn.Name.Equals(tempAvailableButtons[index].Name)).Text = GetCurrentPlayerSign();
        CHECK:
            PCtimer.Stop();
            if (CheckForWin(allButtons, true, GetCurrentPlayerSign()))
            {
                GameWon();
                if (new CustomDialog("Pobedio je " + GetCurrentPlayerTypeName() + "\n\nRestartovati igru?", "Pobeda").ShowDialog() == DialogResult.Yes)
                    RestartGame();
                return;
            }
            GetAvailableButtons();
            if (availableButtons.Count == 0)
            {
                GameDraw();
                if (new CustomDialog("Nerešeno\n\nRestartovati igru?", "Nerešeno").ShowDialog() == DialogResult.Yes)
                    RestartGame();
                return;
            }
            currentTurn = PlayerType.human;
            lbTurn.Text = "{ " + GetCurrentPlayerTypeName() + " je na potezu }";
        }

        private int Minimax(List<Button> clonedAllButtons, int depth, bool isMaximising)
        {
            if (depth == 5)
                return 0;
            currentTurn = (isMaximising) ? firstToPlay : (firstToPlay == PlayerType.human) ? PlayerType.computer : PlayerType.human;
            //PrintMatrix(clonedAllButtons);
            if (CheckForWin(clonedAllButtons, false, GetCurrentPlayerSign()))
                return (isMaximising) ? 1 : -1;

            if (CheckForWin(clonedAllButtons, false, GetOppositePlayerSign()))
                return (isMaximising) ? -1 : 1;

            if (clonedAllButtons.Where(btn => btn.Text != "").Count().Equals(n * n))
                return 0;

            if (isMaximising)
            {
                int bestScore = int.MinValue;
                int score = 0;

                foreach (Button tempAvailableMove in clonedAllButtons)
                {
                    if (tempAvailableMove.Text != "")
                        continue;
                    clonedAllButtons.First(btn => btn.Name.Equals(tempAvailableMove.Name)).Text = GetPlayerSign(maximizingPlayer);
                    score = Minimax(clonedAllButtons, depth + 1, !isMaximising);
                    clonedAllButtons.First(btn => btn.Name.Equals(tempAvailableMove.Name)).Text = "";
                    bestScore = Math.Max(score, bestScore);
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                int score = 0;

                foreach (Button tempAvailableMove in clonedAllButtons)
                {
                    if (tempAvailableMove.Text != "")
                        continue;
                    clonedAllButtons.First(btn => btn.Name.Equals(tempAvailableMove.Name)).Text = GetPlayerSign(minimizingPlayer);
                    score = Minimax(clonedAllButtons, depth + 1, !isMaximising);
                    clonedAllButtons.First(btn => btn.Name.Equals(tempAvailableMove.Name)).Text = "";
                    bestScore = Math.Min(score, bestScore);
                }
                return bestScore;
            }

        }

        /* !!! Computer logic !!! */

        /* Checking for win function */

        private bool CheckForWin(List<Button> buttonsToCheck, bool colorWin, string playerSign)
        {
            int i = 0, j = 0;

            foreach (OrderType orderType in Enum.GetValues(typeof(OrderType)))
            {
                if (CheckWin(buttonsToCheck, orderType, playerSign, ref i, ref j))
                {
                    //Console.WriteLine("Pobeda lol " + orderType.ToString());
                    if (colorWin)
                        ColorWin(orderType, i, j);
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

        private bool CheckWin(List<Button> buttonsToCheck, OrderType order, string playerSign, ref int index, ref int index2)
        {
            int i = 0, j = 0;

            string matchColumns = "b_{0}_[0-3]";
            string matchRows = "b_[0-3]_{0}";
            string matchMainDiagonal = "b_([0-3])_\\1";
            string matchSecondaryDiagonal = "\\b[bB]_(0_3|3_0|1_2|2_1)\\b";
            string matchCube = "\\b[b]_{0}_{1}\\b|\\b[b]_{0}_{2}\\b|\\b[b]_{3}_{1}\\b|\\b[b]_{3}_{2}\\b";

            string currentPlayerSign = GetCurrentPlayerSign();

            switch (order)
            {
                case OrderType.horizontal:
                    {
                        for (i = 0; i < n; i++)
                            if (buttonsToCheck.Where(field => Regex.IsMatch(field.Name, String.Format(matchColumns, i))).
                                           Where(field => field.Text == playerSign).
                                           Count().
                                           Equals(n))
                            {
                                index = i;
                                return true;
                            }
                        break;
                    }
                case OrderType.vertical:
                    {
                        for (i = 0; i < n; i++)
                            if (buttonsToCheck.Where(field => Regex.IsMatch(field.Name, String.Format(matchRows, i))).
                                               Where(field => field.Text == playerSign).
                                               Count().
                                               Equals(n))
                            {
                                index = i;
                                return true;
                            }
                        break;
                    }
                case OrderType.mainDiagonal:
                    {
                        return buttonsToCheck.Where(field => Regex.IsMatch(field.Name, matchMainDiagonal)).
                                           Where(field => field.Text == playerSign).
                                           Count().
                                           Equals(n);
                    }
                case OrderType.secondaryDiagonal:
                    {

                        return buttonsToCheck.Where(field => Regex.IsMatch(field.Name, matchSecondaryDiagonal)).
                                           Where(field => field.Text == playerSign).
                                           Count().
                                           Equals(n);
                    }
                case OrderType.cube:
                    {
                        for (i = 0; i < n - 1; i++)
                        {
                            for (j = 0; j < n - 1; j++)
                            {
                                if (buttonsToCheck.Where(field => Regex.IsMatch(field.Name, String.Format(matchCube, i, j, j + 1, i + 1))).
                                                   Where(field => field.Text == playerSign).
                                                   Count().
                                                   Equals(n))
                                {
                                    index = i;
                                    index2 = j;
                                    return true;
                                }
                            }
                        }
                        break;
                    }
                default:
                    return false;

            }
            return false;
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
            lbTurn.Text = "{ " + GetCurrentPlayerTypeName() + " je pobedio }";
            wait(waitTime);
            allButtons.ForEach(btn => btn.Enabled = false);
        }

        private void GameDraw()
        {
            gameStarted = false;
            lbTurn.Text = ResLibrary.DrawBracketsLbl;
            wait(waitTime);
            allButtons.ForEach(btn => btn.Enabled = false);
        }

        /* !!! Game states functions !!! */

        /* Helper functions */

        private string GetCurrentPlayerTypeName()
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

        private string GetPlayerSign(PlayerType player)
        {
            return (player == PlayerType.human) ? humanSign.ToString() : computerSign.ToString();
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

        private void GetAvailableButtons()
        {
            availableButtons = panel1.Controls.OfType<Button>().Where(btn => btn.Text == "").ToList();
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
