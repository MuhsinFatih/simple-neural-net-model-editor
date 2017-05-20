using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace deepLearning {
    /// <summary>
    /// Interaction logic for Neuron.xaml
    /// </summary>
    public partial class Neuron : UserControl {
		
		public double val = 0;
		// input links
		public Function function;
		public List<Link> links = new List<Link>();

        public Neuron() {
            InitializeComponent();
        }

		public Neuron(Function function) : this() {
			this.function = function;
		}

		public Neuron(double val) : this() {
			this.val = val;
		}
	
		public Neuron(Function function, double val) : this() {
			this.function = function;
			this.val = val;
		}

		public double output() {
			switch (function) {
				case Function.sigmoid:
					return 2 / (1 + Math.Pow(Math.E, -2 * val)) - 1;
				case Function.relu:
					return (val > 0 ? val : 0);
				default:
					return 0;
			}
		}
    }
	
	public enum Function {
		sigmoid,
		relu // rectified linear unit
	}

	

}
