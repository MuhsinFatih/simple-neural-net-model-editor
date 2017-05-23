using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace deepLearning
{
    /// <summary>
    /// Interaction logic for Layer.xaml
    /// </summary>
    public partial class Layer : UserControl
    {
        string name = "layer name";
        public string layerName {
            get {
                return name;
            }
            set {
                name = value;
                label_layerName.Content = value;
            }
        }
        
        public bool selected = false;

        public List<Neuron> neurons = new List<Neuron>();
        public Layer()
        {
            InitializeComponent();
            Content.MouseEnter += delegate {
                if(!selected)
                    Content.Background = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0));
            };
            Content.MouseLeave += delegate {
                if(!selected)
                    Content.Background = new SolidColorBrush(Colors.Transparent);
            };
        }
		
    }
}
