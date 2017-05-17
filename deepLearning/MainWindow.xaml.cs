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
            offset["grid_layers"] = getVector(of: grid_layers, relativeTo: mainCanvas);
            offset["layer0"] = getVector(of: grid_layer0, relativeTo: mainCanvas); // input visual representation layer
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

		List<Rectangle> input = new List<Rectangle>();
		List<Layer> layers = new List<Layer>();

		public void main(object sender, EventArgs e) {

			input.Add(p1); input.Add(p2); input.Add(p3); input.Add(p4);
			foreach (UniformGrid item in grid_layers.Children) {
				layers.Add(new Layer(item.Name, item));
			}


			offset["p1"] = getVector(of: p1, relativeTo: mainCanvas);
			offset["p2"] = getVector(of: p2, relativeTo: mainCanvas);
			offset["p3"] = getVector(of: p3, relativeTo: mainCanvas);
			offset["p4"] = getVector(of: p4, relativeTo: mainCanvas);

			Link link = new Link(getVector(p1, new Point(p1.ActualWidth / 2, 0)), getVector(of: layers[0].neurons[0]));
			layers[0].neurons[0].links.Add(link);
			
			Link link1 = new Link(getVector(p2, new Point(p2.ActualWidth / 2, p2.ActualHeight / 2)), getVector(of: layers[0].neurons[1]));
			layers[0].neurons[1].links.Add(link1);

			Link link2 = new Link(getVector(p3, new Point(p3.ActualWidth / 2, p3.ActualHeight / 2)), getVector(of: layers[0].neurons[2]));
			layers[0].neurons[2].links.Add(link2);

			Link link3 = new Link(getVector(p4, new Point(p4.ActualWidth / 2, p4.ActualHeight)), getVector(of: layers[0].neurons[3]));
			layers[0].neurons[3].links.Add(link3);

			mainCanvas.Children.Add(link);
			mainCanvas.Children.Add(link1);
			mainCanvas.Children.Add(link2);
			mainCanvas.Children.Add(link3);


			//for (int i = 0; i < layers[0].neurons.Count; ++i) {
			//	Link link = new Link(getVector(input[i], new Point(input[i].ActualWidth / 2, input[i].ActualHeight / 2)), getVector(of: layers[0].neurons[i]));
			//	layers[0].neurons[i].links.Add(link);
			//	mainCanvas.Children.Add(link);
			//}



			//mainCanvas.Children.Add(new Link(getVector(input[0], new Point(input[0].ActualWidth / 2, input[0].ActualHeight / 2)), getVector(layers[0].neurons[0])));




            //    Vector relativePoint = (Vector)n1.TransformToAncestor(mainCanvas).Transform(new Point(0, 0));
            //    Point relativePoint2 = n1.TransformToAncestor(mainCanvas).Transform(new Point(0, 0));

            //    Console.WriteLine("relativePoint = {0}", relativePoint);

            //    Line line = new Line() {
            //        Stroke = Brushes.Black,
            //        StrokeThickness = 2,
            //        X1 = 0,
            //        Y1 = 0,
            //        X2 = relativePoint.X,
            //        Y2 = relativePoint.Y
            //    };
            //    mainCanvas.Children.Add(line);
        }



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
