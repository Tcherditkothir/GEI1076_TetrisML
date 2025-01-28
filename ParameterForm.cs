using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisML
{
    public partial class ParameterForm : Form
    {
        private readonly TetrisGame _game;
        private TrackBar _heightWeightSlider;
        private TrackBar _holesWeightSlider;
        private TrackBar _completeLinesWeightSlider;
        private TrackBar _bumpinessWeightSlider;
        private Label _debugLabel;

        public ParameterForm(TetrisGame game)
        {
            _game = game;
            InitializeComponent();
            ConfigureForm();
            SetupControls();
        }

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

        private void SetupControls()
        {
            _heightWeightSlider = CreateSlider("Poids de la hauteur", 20);
            _holesWeightSlider = CreateSlider("Poids des trous", 100);
            _completeLinesWeightSlider = CreateSlider("Poids des lignes complètes", 180);
            _bumpinessWeightSlider = CreateSlider("Poids de l'irrégularité", 260);

            CreateDebugLabel();
        }

        private TrackBar CreateSlider(string labelText, int yPosition)
        {
            // Créer le label
            var label = new Label
            {
                Text = labelText,
                Location = new Point(10, yPosition),
                Size = new Size(200, 20),
                Font = new Font("Arial", 10F)
            };
            this.Controls.Add(label);

            // Créer le slider
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
            slider.ValueChanged += Slider_ValueChanged;
            this.Controls.Add(slider);

            // Créer le label de valeur
            var valueLabel = new Label
            {
                Location = new Point(270, yPosition),
                Size = new Size(100, 20),
                Text = "0.00",
                Font = new Font("Arial", 10F)
            };
            slider.Tag = valueLabel;  // Stocker le label dans le Tag du slider
            this.Controls.Add(valueLabel);

            return slider;
        }

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

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            if (sender is TrackBar slider && slider.Tag is Label valueLabel)
            {
                double value = slider.Value / 100.0;
                valueLabel.Text = value.ToString("F2");

                var parameters = new IAController.AIParameters
                {
                    HeightWeight = _heightWeightSlider.Value / 100.0,
                    HolesWeight = _holesWeightSlider.Value / 100.0,
                    CompleteLinesWeight = _completeLinesWeightSlider.Value / 100.0,
                    BumpinessWeight = _bumpinessWeightSlider.Value / 100.0
                };

                _game.GetIAController()?.UpdateParameters(parameters);

                UpdateDebugLabel(parameters);
            }
        }

        private void UpdateDebugLabel(IAController.AIParameters parameters)
        {
            _debugLabel.Text = $"Hauteur: {parameters.HeightWeight:F2}\n" +
                             $"Trous: {parameters.HolesWeight:F2}\n" +
                             $"Lignes: {parameters.CompleteLinesWeight:F2}\n" +
                             $"Irrégularité: {parameters.BumpinessWeight:F2}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            base.OnFormClosing(e);
        }
    }
}