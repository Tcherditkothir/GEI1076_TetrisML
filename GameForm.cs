using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisML
{
    public partial class GameForm : Form
    {
        // Classe interne pour le double buffering
        private class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                             ControlStyles.UserPaint |
                             ControlStyles.OptimizedDoubleBuffer, true);
            }
        }

        // Variables membres
        private int _blockSize;
        private TetrisGame _game;
        private Timer _gameRefreshTimer;
        private DoubleBufferedPanel _gamePanel;
        private DoubleBufferedPanel _nextPiecePanel;
        private Label _scoreLabel;
        private Button _startButton;
        private Button _aiButton;
        private ParameterForm _parameterForm;

        public GameForm()
        {
            _blockSize = 30;
            InitializeComponent();
            ConfigureInitialState();
            ConfigureGameComponents();
            ConfigureEventHandlers();
        }

        private void ConfigureInitialState()
        {
            _game = new TetrisGame();
            _parameterForm = new ParameterForm(_game);

            this.Text = "Tetris ML";
            this.ClientSize = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            _parameterForm.Show();
        }   

        private void ConfigureGameComponents()
        {
            // Panneau principal du jeu avec double buffering
            _gamePanel = new DoubleBufferedPanel
            {
                Location = new Point(20, 20),
                Size = new Size(300, 600),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Panneau pour la prochaine pièce avec double buffering
            _nextPiecePanel = new DoubleBufferedPanel
            {
                Location = new Point(340, 20),
                Size = new Size(120, 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Label pour le score
            _scoreLabel = new Label
            {
                Location = new Point(340, 160),
                Size = new Size(120, 30),
                Text = "Score: 0",
                Font = new Font("Arial", 12F, FontStyle.Bold)
            };

            // Bouton Start/Pause
            _startButton = new Button
            {
                Location = new Point(340, 200),
                Size = new Size(120, 40),
                Text = "Démarrer",
                Font = new Font("Arial", 10F)
            };

            // Bouton IA
            _aiButton = new Button
            {
                Location = new Point(340, 250),
                Size = new Size(120, 40),
                Text = "Activer IA",
                Font = new Font("Arial", 10F)
            };

            // Timer de rafraîchissement
            _gameRefreshTimer = new Timer
            {
                Interval = 50
            };

            // Ajout des contrôles au formulaire
            this.Controls.AddRange(new Control[]
            {
                _gamePanel,
                _nextPiecePanel,
                _scoreLabel,
                _startButton,
                _aiButton
            });
        }

        private void ConfigureEventHandlers()
        {
            // Événements de dessin
            _gamePanel.Paint += GamePanel_Paint;
            _nextPiecePanel.Paint += NextPiecePanel_Paint;

            // Événements des boutons
            _startButton.Click += StartButton_Click;
            _aiButton.Click += AIButton_Click;

            // Événements clavier
            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;

            // Timer
            _gameRefreshTimer.Tick += RefreshTimer_Tick;
            _gameRefreshTimer.Start();
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            var grid = _game.GetGrid();
            var currentPiece = _game.GetCurrentPiece();

            // Dessiner la grille
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] > 0)
                    {
                        DrawBlock(e.Graphics, x, y, grid[y, x] - 1);
                    }
                }
            }

            // Dessiner la pièce courante
            if (currentPiece != null)
            {
                foreach (var block in currentPiece.GetCurrentRotationBlocks())
                {
                    int x = currentPiece.X + block.X;
                    int y = currentPiece.Y + block.Y;
                    if (y >= 0)
                    {
                        DrawBlock(e.Graphics, x, y, currentPiece.Type);
                    }
                }
            }

            // Dessiner la grille
            using (Pen gridPen = new Pen(Color.LightGray))
            {
                for (int x = 0; x <= grid.GetLength(1); x++)
                {
                    e.Graphics.DrawLine(gridPen,
                        x * _blockSize, 0,
                        x * _blockSize, grid.GetLength(0) * _blockSize);
                }
                for (int y = 0; y <= grid.GetLength(0); y++)
                {
                    e.Graphics.DrawLine(gridPen,
                        0, y * _blockSize,
                        grid.GetLength(1) * _blockSize, y * _blockSize);
                }
            }
        }

        private void NextPiecePanel_Paint(object sender, PaintEventArgs e)
        {
            var nextPiece = _game.GetNextPiece();
            if (nextPiece != null)
            {
                foreach (var block in nextPiece.GetCurrentRotationBlocks())
                {
                    DrawBlock(e.Graphics, block.X + 1, block.Y + 1, nextPiece.Type);
                }
            }
        }

        private void DrawBlock(Graphics g, int x, int y, int type)
        {
            Rectangle rect = new Rectangle(
                x * _blockSize,
                y * _blockSize,
                _blockSize,
                _blockSize);

            // Remplir le bloc
            using (SolidBrush brush = new SolidBrush(Piece.PieceColors[type]))
            {
                g.FillRectangle(brush, rect);
            }

            // Dessiner la bordure
            using (Pen pen = new Pen(Color.DarkGray))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (_startButton.Text == "Démarrer" || _startButton.Text == "Reprendre")
            {
                _game.StartGame();
                _startButton.Text = "Pause";
            }
            else
            {
                _game.PauseGame();
                _startButton.Text = "Reprendre";
            }
            this.Focus();
        }

        private void AIButton_Click(object sender, EventArgs e)
        {
            _game.ToggleAI();
            _aiButton.Text = _aiButton.Text == "Activer IA" ? "Désactiver IA" : "Activer IA";
            this.Focus();
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _game.MovePiece(-1, 0);
                    break;
                case Keys.Right:
                    _game.MovePiece(1, 0);
                    break;
                case Keys.Down:
                    _game.MovePiece(0, 1);
                    break;
                case Keys.Up:
                    _game.RotatePiece();
                    break;
                case Keys.P:
                    if (_startButton.Text == "Pause")
                    {
                        _game.PauseGame();
                        _startButton.Text = "Reprendre";
                    }
                    else
                    {
                        _game.StartGame();
                        _startButton.Text = "Pause";
                    }
                    break;
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            _gamePanel.Invalidate();
            _nextPiecePanel.Invalidate();
            _scoreLabel.Text = $"Score: {_game.GetScore()}";

            if (_game.IsGameOver())
            {
                _gameRefreshTimer.Stop();
                _startButton.Text = "Nouvelle Partie";
                MessageBox.Show($"Game Over!\nScore final: {_game.GetScore()}",
                              "Fin de partie",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _parameterForm?.Close();  // Fermer aussi le formulaire de paramètres
            base.OnFormClosing(e);
        }

    }
}