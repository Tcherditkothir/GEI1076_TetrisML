/*	Bref

GameForm est le point d'interaction visuel et utilisateur du jeu Tetris. 
Elle assure la présentation graphique du jeu, gère les interactions avec l'utilisateur via des événements (clavier, boutons, timer), 
et fait le lien entre la logique de jeu encapsulée dans TetrisGame et l'affichage de cette logique à l'écran. 
Grâce à sa structure modulaire et à l'utilisation de techniques comme le double buffering, GameForm garantit un rendu fluide et une expérience utilisateur réactive.

---
La classe GameForm constitue la fenêtre principale de l'interface graphique du jeu Tetris et joue un rôle central dans la présentation et l'interaction avec la logique de jeu. Voici en détail ses principales responsabilités et caractéristiques :

Héritage et Structure Globale

Héritage de Form :
GameForm hérite de la classe Form de Windows Forms, ce qui lui confère toutes les fonctionnalités d'une fenêtre (affichage, gestion des événements, etc.).
Organisation en Méthodes :
Le fichier est structuré en plusieurs méthodes dédiées à la configuration initiale, à la création des composants graphiques, à l'attribution des gestionnaires d'événements, et au rendu graphique.
Gestion des Composants Graphiques

Double Buffering pour un Rendu Fluide :
Une classe interne privée, DoubleBufferedPanel, dérivée de Panel, est utilisée pour créer des panneaux (GamePanel et NextPiecePanel) avec le double buffering activé. Cela permet d'éviter les scintillements lors du rafraîchissement graphique.
Création et Configuration des Contrôles :
La méthode ConfigureGameComponents instancie et configure plusieurs contrôles clés, tels que :
Un panneau principal (_gamePanel) pour afficher la grille du jeu.
Un panneau secondaire (_nextPiecePanel) pour afficher la prochaine pièce.
Un label (_scoreLabel) pour afficher le score en temps réel.
Des boutons (_startButton et _aiButton) pour démarrer/mettre en pause le jeu et activer/désactiver l'IA.
Formulaire de Paramètres :
Une instance de ParameterForm est créée et affichée, permettant ainsi à l'utilisateur de régler les paramètres de l'IA.
Configuration et Gestion des Événements

Gestionnaire d'Événements :
La méthode ConfigureEventHandlers attache des gestionnaires aux différents événements :
Événements de dessin pour les panneaux (via Paint) afin de redessiner la grille et la pièce en cours, ainsi que la prochaine pièce.
Événements de boutons pour répondre aux clics sur les boutons de démarrage et d'activation de l'IA.
Événements clavier (via KeyDown) pour permettre au joueur de contrôler le déplacement et la rotation des pièces.
Timer de rafraîchissement (via _gameRefreshTimer) qui déclenche périodiquement la mise à jour de l'affichage (recalcul du score, rafraîchissement des panneaux) et la vérification de l'état du jeu (fin de partie).
Rendu Graphique et Mise à Jour de l'Interface

Méthode de Dessin du Jeu (GamePanel_Paint) :
Elle parcourt la grille retournée par TetrisGame, dessine les blocs déjà placés et, si la pièce en cours existe, elle dessine chacun de ses blocs à la position adéquate en utilisant la méthode DrawBlock.
Méthode de Dessin de la Prochaine Pièce (NextPiecePanel_Paint) :
Elle affiche la configuration de la prochaine pièce dans son panneau dédié, souvent avec un léger décalage pour un meilleur rendu visuel.
Dessin des Blocs :
La méthode DrawBlock prend en charge le dessin d’un bloc en définissant un rectangle correspondant à sa position et en utilisant une couleur spécifique basée sur le type de pièce. Un remplissage et un contour sont appliqués pour améliorer la visibilité.
Interaction avec la Logique de Jeu

Mise à Jour de l'Affichage :
Le timer de rafraîchissement (_gameRefreshTimer) s'assure que les panneaux se redessinent régulièrement, mettant ainsi à jour l'affichage en fonction de l'évolution du jeu (position des pièces, score, etc.).
Contrôle du Jeu :
Les boutons et les événements clavier permettent au joueur de démarrer ou mettre en pause la partie, de contrôler le déplacement et la rotation des pièces, ainsi que d'activer ou désactiver l'intelligence artificielle intégrée dans la logique du jeu.
Gestion de la Fin de Partie :
Lorsque le jeu se termine (détecté par TetrisGame), le timer est arrêté, l'affichage est mis à jour (le bouton de démarrage passe à "Nouvelle Partie"), et un message est affiché pour informer le joueur du score final.
*/

/*	Explications des principes POO appliqués :

Héritage
La classe GameForm hérite de Form, permettant d'utiliser et de personnaliser le comportement d'une fenêtre Windows Forms.

Encapsulation
Les variables membres et méthodes privées (comme ConfigureInitialState, ConfigureGameComponents, etc.) masquent les détails d'implémentation de la configuration et du rendu graphique.
Les détails du rendu (dessin de la grille, des blocs et des pièces) sont encapsulés dans des méthodes dédiées.

Abstraction
Les différentes méthodes de configuration (initialisation, configuration des composants, gestion des événements) offrent une interface claire pour organiser la logique du formulaire sans exposer les détails sous-jacents.
Les méthodes de dessin (GamePanel_Paint, NextPiecePanel_Paint, DrawBlock) fournissent une abstraction du rendu des éléments graphiques du jeu.

Composition
GameForm est composé de plusieurs objets (par exemple, un objet TetrisGame pour la logique du jeu, des Panel pour l'affichage, des Button pour les commandes, etc.) qui, ensemble, constituent l'interface utilisateur et le comportement du jeu.
La classe interne DoubleBufferedPanel est utilisée pour améliorer les performances graphiques en activant le double buffering.

Polymorphisme (via les événements)
La surcharge de la méthode OnFormClosing permet de personnaliser le comportement de fermeture du formulaire.
L'utilisation d'événements pour les boutons, le timer et les touches du clavier illustre comment le comportement peut être personnalisé en fonction des interactions de l'utilisateur.

Ce code montre comment la POO permet de structurer et d'organiser l'interface graphique d'un jeu en séparant les préoccupations (configuration, rendu, gestion des événements) dans des méthodes et classes dédiées, rendant ainsi le code clair, modulaire et maintenable.
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisML
{
    // La classe GameForm hérite de Form et représente l'interface graphique principale du jeu Tetris.
    // Principes POO appliqués :
    // - Héritage : GameForm hérite de Form pour bénéficier de toutes les fonctionnalités d'une fenêtre Windows Forms.
    // - Encapsulation : Les variables membres et méthodes privées cachent la complexité de la configuration et du rendu du jeu.
    // - Composition : GameForm est composé d'objets tels que TetrisGame, Timer, Panels, Labels, et Buttons pour construire son interface.
    // - Abstraction : La logique de configuration et de rafraîchissement de l'affichage est encapsulée dans des méthodes dédiées.
    public partial class GameForm : Form
    {
        // -----------------------------------------------------------
        // Classe interne pour le double buffering
        // -----------------------------------------------------------
        // Abstraction et Héritage : Cette classe hérite de Panel et configure le double buffering pour un rendu graphique fluide.
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

        // -----------------------------------------------------------
        // Variables membres (encapsulation des données)
        // -----------------------------------------------------------
        private int _blockSize;
        private TetrisGame _game;
        private Timer _gameRefreshTimer;
        private DoubleBufferedPanel _gamePanel;
        private DoubleBufferedPanel _nextPiecePanel;
        private Label _scoreLabel;
        private Button _startButton;
        private Button _aiButton;
        private ParameterForm _parameterForm;

        // -----------------------------------------------------------
        // Constructeur
        // -----------------------------------------------------------
        // Abstraction : Le constructeur initialise l'état initial, les composants du jeu et configure les gestionnaires d'événements.
        public GameForm()
        {
            _blockSize = 30;
            InitializeComponent();
            ConfigureInitialState();
            ConfigureGameComponents();
            ConfigureEventHandlers();
        }

        // -----------------------------------------------------------
        // Méthode ConfigureInitialState
        // -----------------------------------------------------------
        // Abstraction : Initialise l'état initial du jeu (création de TetrisGame, ParameterForm et configuration de la fenêtre).
        private void ConfigureInitialState()
        {
            _game = new TetrisGame();
            _parameterForm = new ParameterForm(_game);

            this.Text = "Tetris ML";
            this.ClientSize = new Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Affiche le formulaire de paramètres pour permettre l'ajustement des paramètres de l'IA.
            _parameterForm.Show();
        }

        // -----------------------------------------------------------
        // Méthode ConfigureGameComponents
        // -----------------------------------------------------------
        // Abstraction et Composition : Crée et configure les différents composants de l'interface graphique (panneaux, labels, boutons, timer).
        private void ConfigureGameComponents()
        {
            // Panneau principal du jeu avec double buffering pour un rendu fluide.
            _gamePanel = new DoubleBufferedPanel
            {
                Location = new Point(20, 20),
                Size = new Size(300, 600),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Panneau pour afficher la prochaine pièce.
            _nextPiecePanel = new DoubleBufferedPanel
            {
                Location = new Point(340, 20),
                Size = new Size(120, 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Label pour afficher le score actuel.
            _scoreLabel = new Label
            {
                Location = new Point(340, 160),
                Size = new Size(120, 30),
                Text = "Score: 0",
                Font = new Font("Arial", 12F, FontStyle.Bold)
            };

            // Bouton pour démarrer ou mettre en pause le jeu.
            _startButton = new Button
            {
                Location = new Point(340, 200),
                Size = new Size(120, 40),
                Text = "Démarrer",
                Font = new Font("Arial", 10F)
            };

            // Bouton pour activer ou désactiver l'IA.
            _aiButton = new Button
            {
                Location = new Point(340, 250),
                Size = new Size(120, 40),
                Text = "Activer IA",
                Font = new Font("Arial", 10F)
            };

            // Timer de rafraîchissement pour mettre à jour l'affichage à intervalle régulier.
            _gameRefreshTimer = new Timer
            {
                Interval = 50
            };

            // Ajout de tous les contrôles au formulaire.
            this.Controls.AddRange(new Control[]
            {
                _gamePanel,
                _nextPiecePanel,
                _scoreLabel,
                _startButton,
                _aiButton
            });
        }

        // -----------------------------------------------------------
        // Méthode ConfigureEventHandlers
        // -----------------------------------------------------------
        // Abstraction : Configure les gestionnaires d'événements pour les contrôles, le timer et les événements clavier.
        private void ConfigureEventHandlers()
        {
            // Événements de dessin pour les panneaux du jeu.
            _gamePanel.Paint += GamePanel_Paint;
            _nextPiecePanel.Paint += NextPiecePanel_Paint;

            // Gestionnaires pour les boutons.
            _startButton.Click += StartButton_Click;
            _aiButton.Click += AIButton_Click;

            // Gestion des événements clavier pour contrôler le jeu.
            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;

            // Gestionnaire pour le timer de rafraîchissement.
            _gameRefreshTimer.Tick += RefreshTimer_Tick;
            _gameRefreshTimer.Start();
        }

        // -----------------------------------------------------------
        // Méthode GamePanel_Paint
        // -----------------------------------------------------------
        // Abstraction : Rendu graphique du panneau principal qui affiche la grille de jeu et la pièce courante.
        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            var grid = _game.GetGrid();
            var currentPiece = _game.GetCurrentPiece();

            // Dessiner la grille en parcourant chaque case.
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[y, x] > 0)
                    {
                        // Les blocs existants dans la grille sont dessinés en fonction de leur type.
                        DrawBlock(e.Graphics, x, y, grid[y, x] - 1);
                    }
                }
            }

            // Dessiner la pièce courante en utilisant ses blocs selon sa rotation.
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

            // Dessiner les lignes de la grille pour aider à visualiser la structure.
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

        // -----------------------------------------------------------
        // Méthode NextPiecePanel_Paint
        // -----------------------------------------------------------
        // Abstraction : Rendu graphique du panneau qui affiche la prochaine pièce.
        private void NextPiecePanel_Paint(object sender, PaintEventArgs e)
        {
            var nextPiece = _game.GetNextPiece();
            if (nextPiece != null)
            {
                // Décalage pour centrer la pièce dans le panneau.
                foreach (var block in nextPiece.GetCurrentRotationBlocks())
                {
                    DrawBlock(e.Graphics, block.X + 1, block.Y + 1, nextPiece.Type);
                }
            }
        }

        // -----------------------------------------------------------
        // Méthode DrawBlock
        // -----------------------------------------------------------
        // Abstraction : Dessine un bloc individuel à une position donnée et selon le type de pièce (pour la couleur).
        private void DrawBlock(Graphics g, int x, int y, int type)
        {
            Rectangle rect = new Rectangle(
                x * _blockSize,
                y * _blockSize,
                _blockSize,
                _blockSize);

            // Remplissage du bloc avec la couleur associée au type de pièce.
            using (SolidBrush brush = new SolidBrush(Piece.PieceColors[type]))
            {
                g.FillRectangle(brush, rect);
            }

            // Dessiner la bordure du bloc pour une meilleure visibilité.
            using (Pen pen = new Pen(Color.DarkGray))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        // -----------------------------------------------------------
        // Gestionnaire d'événement StartButton_Click
        // -----------------------------------------------------------
        // Abstraction : Démarre ou met en pause le jeu en fonction de l'état actuel.
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
            // Remet le focus sur le formulaire pour que les événements clavier continuent de fonctionner.
            this.Focus();
        }

        // -----------------------------------------------------------
        // Gestionnaire d'événement AIButton_Click
        // -----------------------------------------------------------
        // Abstraction : Active ou désactive l'intelligence artificielle et met à jour le libellé du bouton.
        private void AIButton_Click(object sender, EventArgs e)
        {
            _game.ToggleAI();
            _aiButton.Text = _aiButton.Text == "Activer IA" ? "Désactiver IA" : "Activer IA";
            this.Focus();
        }

        // -----------------------------------------------------------
        // Gestionnaire d'événement GameForm_KeyDown
        // -----------------------------------------------------------
        // Abstraction : Permet à l'utilisateur de contrôler le jeu via le clavier.
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

        // -----------------------------------------------------------
        // Gestionnaire d'événement RefreshTimer_Tick
        // -----------------------------------------------------------
        // Abstraction : Met à jour régulièrement l'affichage du jeu, du panneau de la prochaine pièce et du score.
        // Vérifie également la fin de partie.
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            _gamePanel.Invalidate();      // Demande un rafraîchissement du panneau de jeu
            _nextPiecePanel.Invalidate(); // Demande un rafraîchissement du panneau de la prochaine pièce
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

        // -----------------------------------------------------------
        // Surcharge de la méthode OnFormClosing
        // -----------------------------------------------------------
        // Encapsulation : Permet de s'assurer que le formulaire de paramètres est également fermé lorsque le GameForm se ferme.
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _parameterForm?.Close();  // Ferme le formulaire de paramètres s'il existe
            base.OnFormClosing(e);
        }
    }
}
