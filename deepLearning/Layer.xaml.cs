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
        public string name;
		public List<Neuron> neurons;
		public Layer(string name) {
			this.name = name;
			neurons = new List<Neuron>();
			foreach (Neuron item in Content.Children) {
				neurons.Add(item);
			}
		}
        public Layer()
        {
            InitializeComponent();
        }
    }
}
