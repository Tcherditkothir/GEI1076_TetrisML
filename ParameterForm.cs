/*	Bref

ParameterForm est la fenêtre qui permet à l'utilisateur de configurer et d'affiner le comportement de l'IA dans 
le jeu Tetris en ajustant facilement ses paramètres via une interface graphique conviviale

---

La classe ParameterForm sert d'interface utilisateur permettant d'ajuster dynamiquement les paramètres de l'intelligence artificielle (IA) du jeu Tetris. Voici sa raison d'être résumée :

Interface de Réglage des Paramètres de l'IA :
Elle présente plusieurs contrôles (sliders, labels, boutons) qui permettent à l'utilisateur de modifier les poids attribués aux différents critères utilisés par l'IA (hauteur, trous, lignes complètes, irrégularité). Ces poids influencent l'évaluation des positions de la grille par l'IA.

Mise à Jour Dynamique :
Chaque fois qu'un slider est ajusté, le formulaire convertit la valeur du contrôle en une valeur décimale, met à jour le label associé pour refléter la nouvelle valeur et transmet ces nouvelles valeurs à l'instance de l'IA via la méthode UpdateParameters de l'IAController.

Affichage des Informations de Débogage :
Un label dédié affiche en temps réel les valeurs actuelles des paramètres, offrant ainsi un retour visuel immédiat sur l'impact des réglages effectués.
*/

/*	Explications des principes POO appliqués :

Héritage
La classe ParameterForm hérite de Form, ce qui permet d'utiliser et de personnaliser les fonctionnalités d'une fenêtre Windows Forms.

Encapsulation
Les champs privés (par exemple, _game, _heightWeightSlider, etc.) et les méthodes privées (par exemple, ConfigureForm, CreateSlider, etc.) cachent les détails d'implémentation à l'utilisateur de la classe.
L'utilisation de la propriété Tag sur les sliders permet de stocker des informations de manière encapsulée.

Abstraction
Les méthodes comme SetupControls, CreateSlider et UpdateDebugLabel offrent une interface simple pour interagir avec l'interface utilisateur sans exposer la complexité de leur implémentation.
La création d'un objet IAController.AIParameters dans le gestionnaire d'événement simplifie la mise à jour des paramètres de l'IA.

Composition
La classe compose son interface en utilisant divers objets (contrôles, labels, sliders) et une référence à TetrisGame pour communiquer avec le reste de l'application.

Polymorphisme
L'override de OnFormClosing permet de modifier le comportement par défaut lors de la fermeture de la fenêtre.
L'utilisation d'événements (comme ValueChanged) permet de définir des comportements personnalisés en réponse aux interactions de l'utilisateur.

Ce code montre ainsi comment les principes fondamentaux de la POO sont appliqués dans la création d'une interface utilisateur pour ajuster les paramètres de l'IA dans un jeu Tetris.
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisML
{
    // La classe ParameterForm hérite de Form, illustrant ainsi le principe d'héritage.
    // Elle représente une fenêtre de configuration pour ajuster les paramètres de l'IA du jeu.
    // Principes POO appliqués :
    // - Encapsulation : Les champs privés et les méthodes internes masquent les détails d'implémentation.
    // - Abstraction : La complexité de l'interface utilisateur est cachée derrière des méthodes dédiées (SetupControls, CreateSlider, etc.).
    // - Composition : La classe utilise des objets (TetrisGame, TrackBar, Label) pour construire son interface.
    // - Polymorphisme : L'override de OnFormClosing permet de modifier le comportement de fermeture du formulaire.
    public partial class ParameterForm : Form
    {
        // Composition et Encapsulation :
        // Le champ _game représente une dépendance injectée dans le formulaire et reste en lecture seule.
        private readonly TetrisGame _game;

        // Champs privés pour les contrôles de l'interface utilisateur.
        private TrackBar _heightWeightSlider;
        private TrackBar _holesWeightSlider;
        private TrackBar _completeLinesWeightSlider;
        private TrackBar _bumpinessWeightSlider;
        private Label _debugLabel;

        // Constructeur qui initialise le formulaire avec une référence au jeu.
        // Injection de dépendance : TetrisGame est passé en paramètre.
        public ParameterForm(TetrisGame game)
        {
            _game = game;
            InitializeComponent();  // Initialisation des composants de base du formulaire.
            ConfigureForm();        // Configuration spécifique de la fenêtre.
            SetupControls();        // Création et configuration des contrôles (sliders et labels).
        }

        // Méthode générée pour initialiser les composants de base du formulaire.
        // Encapsulation : Les détails d'initialisation sont masqués à l'utilisateur de la classe.
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // 
            // ParameterForm
            // 
            this.KeyPreview = true;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ParameterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Paramètres IA";

            this.ResumeLayout(false);
        }

        // Configure l'apparence et la position du formulaire.
        // Abstraction : Les détails de configuration de la fenêtre sont cachés dans une méthode dédiée.
        private void ConfigureForm()
        {
            this.Text = "Paramètres IA";
            this.Size = new Size(400, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                Screen.PrimaryScreen.WorkingArea.Right - this.Width - 20,
                Screen.PrimaryScreen.WorkingArea.Top + 20);
        }

        // Crée et configure les contrôles de l'interface utilisateur.
        // Abstraction : La méthode SetupControls encapsule la logique de création et d'agencement des contrôles.
        private void SetupControls()
        {
            _heightWeightSlider = CreateSlider("Poids de la hauteur", 20);
            _holesWeightSlider = CreateSlider("Poids des trous", 100);
            _completeLinesWeightSlider = CreateSlider("Poids des lignes complètes", 180);
            _bumpinessWeightSlider = CreateSlider("Poids de l'irrégularité", 260);

            CreateDebugLabel();
        }

        // Crée un slider (TrackBar) ainsi que les labels associés pour afficher et modifier une valeur.
        // Abstraction et Réutilisabilité : La méthode encapsule la création d'un slider et peut être utilisée pour différents paramètres.
        private TrackBar CreateSlider(string labelText, int yPosition)
        {
            // Création du label descriptif du paramètre.
            var label = new Label
            {
                Text = labelText,
                Location = new Point(10, yPosition),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10F)
            };
            this.Controls.Add(label);

            // Création du slider permettant de modifier la valeur du paramètre.
            var slider = new TrackBar
            {
                Location = new Point(10, yPosition + 25),
                Size = new Size(350, 45),
                Minimum = -100,
                Maximum = 100,
                Value = 0,
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 1
            };
            // Abstraction : L'abonnement à l'événement ValueChanged permet de centraliser la gestion des changements de valeur.
            slider.ValueChanged += Slider_ValueChanged;
            this.Controls.Add(slider);

            // Création d'un label pour afficher la valeur actuelle du slider.
            var valueLabel = new Label
            {
                Location = new Point(270, yPosition),
                Size = new Size(100, 20),
                Text = "0.00",
                Font = new Font("Arial", 10F)
            };
            // Encapsulation : Le label est stocké dans la propriété Tag du slider pour y accéder facilement lors d'un changement de valeur.
            slider.Tag = valueLabel;
            this.Controls.Add(valueLabel);

            return slider;
        }

        // Crée un label de débogage pour afficher les valeurs actuelles des paramètres.
        // Abstraction : La méthode encapsule la création et l'ajout du label de débogage.
        private void CreateDebugLabel()
        {
            _debugLabel = new Label
            {
                Location = new Point(10, 320),
                Size = new Size(350, 60),
                Font = new Font("Arial", 9F),
                Text = "Ajustez les paramètres pour modifier le comportement de l'IA"
            };
            this.Controls.Add(_debugLabel);
        }

        // Gestionnaire d'événement appelé lorsque la valeur d'un slider change.
        // Polymorphisme : L'utilisation des événements permet de définir un comportement spécifique en réponse aux actions de l'utilisateur.
        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            if (sender is TrackBar slider && slider.Tag is Label valueLabel)
            {
                // Conversion de la valeur du slider en un format décimal.
                double value = slider.Value / 100.0;
                valueLabel.Text = value.ToString("F2");

                // Création d'un objet de paramètres pour l'IA en encapsulant les valeurs des différents sliders.
                var parameters = new IAController.AIParameters
                {
                    HeightWeight = _heightWeightSlider.Value / 100.0,
                    HolesWeight = _holesWeightSlider.Value / 100.0,
                    CompleteLinesWeight = _completeLinesWeightSlider.Value / 100.0,
                    BumpinessWeight = _bumpinessWeightSlider.Value / 100.0
                };

                // Communication entre objets : mise à jour des paramètres de l'IA dans le jeu.
                _game.GetIAController()?.UpdateParameters(parameters);

                // Mise à jour du label de débogage pour afficher les nouvelles valeurs.
                UpdateDebugLabel(parameters);
            }
        }

        // Met à jour le label de débogage pour afficher les valeurs actuelles des paramètres de l'IA.
        // Abstraction : La méthode encapsule la logique de mise en forme et d'affichage des valeurs.
        private void UpdateDebugLabel(IAController.AIParameters parameters)
        {
            _debugLabel.Text = $"Hauteur: {parameters.HeightWeight:F2}\n" +
                               $"Trous: {parameters.HolesWeight:F2}\n" +
                               $"Lignes: {parameters.CompleteLinesWeight:F2}\n" +
                               $"Irrégularité: {parameters.BumpinessWeight:F2}";
        }

        // Surcharge de la méthode OnFormClosing pour modifier le comportement lors de la fermeture du formulaire.
        // Polymorphisme : L'override permet de personnaliser la réponse à l'événement de fermeture.
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Au lieu de fermer le formulaire, on l'empêche et on le cache.
                e.Cancel = true;
                this.Hide();
            }
            base.OnFormClosing(e);
        }
    }
}
