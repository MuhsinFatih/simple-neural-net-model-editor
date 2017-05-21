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
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace deepLearning {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.ContentRendered += main;
        }


        
        int pixelSize = 70;
        int neuronSize = 52;
		
        


        Dictionary<string, Vector> offset = new Dictionary<string, Vector>();

		Vector getVector(UIElement of) {
			if (of is Neuron) {
				return (Vector)of.TransformToAncestor(mainCanvas).Transform(new Point(neuronSize / 2, neuronSize / 2));
			}
			return (Vector)of.TransformToAncestor(mainCanvas).Transform(new Point(0,0));
		}
        Vector getVector(UIElement of, UIElement relativeTo) {
			if(of is Neuron) {
				return (Vector)of.TransformToAncestor(relativeTo).Transform(new Point(neuronSize / 2, neuronSize / 2));
			}
            return (Vector)of.TransformToAncestor(relativeTo).Transform(new Point(0, 0));
        }
		Vector getVector(UIElement of, UIElement relativeTo, Point offset) {
			return (Vector)of.TransformToAncestor(relativeTo).Transform(offset);
		}
		Vector getVector(UIElement of, Point offset) {
			return (Vector)of.TransformToAncestor(mainCanvas).Transform(offset);
		}

		private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            offset["demoLayers"] = getVector(of: demoLayers, relativeTo: mainCanvas);
            offset["demoInputLayer"] = getVector(of: demoInputLayer, relativeTo: mainCanvas); // input visual representation layer
            offset["inputRect"] = getVector(of: inputRect, relativeTo: mainCanvas); // input visual representation rectangle (the pixels)
			
            offset["layer1"] = getVector(of: grid_layer1, relativeTo: mainCanvas); // input layer
            offset["layer2"] = getVector(of: grid_layer2, relativeTo: mainCanvas);
            offset["layer3"] = getVector(of: grid_layer3, relativeTo: mainCanvas);
            offset["layer4"] = getVector(of: grid_layer4, relativeTo: mainCanvas); // output layer


            foreach (Neuron item in grid_layer1.Children) {
				offset[item.Name] = getVector(of: item, relativeTo: mainCanvas);
			}
            foreach (var item in offset) {
                Console.WriteLine("offset[{0}] = {1}:{2}", item.Key, item.Value.X, item.Value.Y);
            }
        }

		void addLink(Link link, Neuron to, Canvas drawOn) {
			to.links.Add(link);
			if (drawOn != null) {
				try {
					drawOn.Children.Add(link);
				}
				catch (Exception e) {
					if(e.HResult != -2147024809) {
						MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
					// else: canvas already has the child. No need to add it again
				}

			}
		}

		List<Rectangle> demoInput = new List<Rectangle>();
		List<Layer> layers = new List<Layer>();

		public void main(object sender, EventArgs e) {

			

			
		}



		void demo() {
			demoInputLayer.Visibility = Visibility.Visible;
			grid_layers.Visibility = Visibility.Hidden;
			demoLayers.Visibility = Visibility.Visible;

			demoInput.Add(p1); demoInput.Add(p2); demoInput.Add(p3); demoInput.Add(p4);
			foreach (UniformGrid item in demoLayers.Children) {
				layers.Add(new Layer(item.Name, item));
			}




			offset["p1"] = getVector(of: p1, relativeTo: mainCanvas);
			offset["p2"] = getVector(of: p2, relativeTo: mainCanvas);
			offset["p3"] = getVector(of: p3, relativeTo: mainCanvas);
			offset["p4"] = getVector(of: p4, relativeTo: mainCanvas);

			Link link = new Link(getVector(p1, new Point(p1.ActualWidth / 2, 0)), getVector(of: layers[0].neurons[0]), 1);
			addLink(link, to: layers[0].neurons[0], drawOn: mainCanvas);

			Link link1 = new Link(getVector(p2, new Point(p2.ActualWidth / 2, p2.ActualHeight / 2)), getVector(of: layers[0].neurons[1]), 1);
			addLink(link1, to: layers[0].neurons[1], drawOn: mainCanvas);

			Link link2 = new Link(getVector(p3, new Point(p3.ActualWidth / 2, p3.ActualHeight / 2)), getVector(of: layers[0].neurons[2]), 1);
			addLink(link2, to: layers[0].neurons[2], drawOn: mainCanvas);

			Link link3 = new Link(getVector(p4, new Point(p4.ActualWidth / 2, p4.ActualHeight)), getVector(of: layers[0].neurons[3]), 1);
			addLink(link3, to: layers[0].neurons[3], drawOn: mainCanvas);



		}





		// this is where the serious business is done
		void approximate() {
			for(int l=0; l<layers.Count; ++l) {
				for(int n=0; n<layers[l].neurons.Count; ++n) {
					var neuron = layers[l].neurons[n];
					for (int i = 0; i < neuron.links.Count; ++i) {
						var link = neuron.links[i];
						neuron.val += link.Weight * link.input.val;
					}
				}
			}
		}


#region UI buttons

        private void btn_addLayer_Click(object sender, RoutedEventArgs e) {
            layers_menu.Visibility = Visibility.Visible;
        }

        private void btn_removeLayer_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_addNeuron_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_removeNeuron_Click(object sender, RoutedEventArgs e) {

        }

#endregion


    }

    struct Layer {
		public string name;
		public List<Neuron> neurons;
		public UniformGrid grid;
		public Layer(string name, UniformGrid grid) {
			this.name = name;
			neurons = new List<Neuron>();
			this.grid = grid;
			foreach (Neuron item in grid.Children) {
				neurons.Add(item);
			}
		}
	}
    

	//class Neuron : Border{
	//    public double weight;
	//    public double value;
	//    public Ellipse ellipse;


	//    public Neuron(int neuronSize) {
	//        weight = 0;
	//        value = 0;
	//        ellipse = new Ellipse() { Width = neuronSize, Height = 52, Fill = Brushes.White, Stroke = Brushes.Black, StrokeThickness = 1 };
	//    }
	//}
}
