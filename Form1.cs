/*	Bref
Ce fichier représente souvent un squelette de formulaire dans une application Windows Forms, où la majorité de la logique d'affichage est gérée par l'outil de conception et la classe de base Form.

*/

/*	Explications des principes POO appliqués :

Héritage
La classe Form1 hérite de Form, ce qui lui permet de bénéficier de toutes les fonctionnalités d'une fenêtre Windows Forms sans avoir à les redéfinir.

Encapsulation
La classe Form1 encapsule la logique de l'interface utilisateur dans un seul endroit, et le mot-clé partial permet de séparer le code généré automatiquement (par l'outil de conception) du code personnalisé.

Abstraction
L'appel à InitializeComponent() dans le constructeur masque la complexité de la configuration de l'interface, offrant ainsi une interface simple pour initialiser le formulaire.

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisML
{
    // La classe Form1 hérite de la classe Form de Windows Forms.
    // Ceci illustre le principe d'héritage, qui permet à Form1 d'obtenir toutes les fonctionnalités d'une fenêtre (formulaire)
    // sans avoir à les réimplémenter.
    // De plus, le concept de partialité ("partial class") permet de séparer le code généré automatiquement (par le concepteur de formulaires)
    // du code écrit manuellement, favorisant ainsi l'encapsulation et la maintenabilité du code.
    public partial class Form1 : Form
    {
        // -----------------------------------------------------------
        // Constructeur de Form1
        // -----------------------------------------------------------
        // Abstraction : Le constructeur initialise le formulaire en appelant InitializeComponent(),
        // qui configure l'interface utilisateur (création et agencement des contrôles, etc.),
        // masquant ainsi la complexité de l'initialisation des composants graphiques.
        public Form1()
        {
            InitializeComponent();
        }
    }
}
