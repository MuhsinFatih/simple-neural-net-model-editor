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

namespace deepLearning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ContentRendered += main;
        }





        



        int pixelSize = 70;
        int neuronSize = 52;
        List<Layer> layers = new List<Layer>();


        Dictionary<string, Vector> offset;

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            offset["grid_neurons"] = VisualTreeHelper.GetOffset(grid_neurons);
            offset["layer0"] = VisualTreeHelper.GetOffset(grid_layer0); // input visual representation layer

            // sum vectors to get real positions in canvas
            offset["layer1"] = VisualTreeHelper.GetOffset(grid_layer1) + offset["grid_neurons"]; // input layer
            offset["layer2"] = VisualTreeHelper.GetOffset(grid_layer2) + offset["grid_neurons"];
            offset["layer3"] = VisualTreeHelper.GetOffset(grid_layer3) + offset["grid_neurons"];
            offset["layer4"] = VisualTreeHelper.GetOffset(grid_layer4) + offset["grid_neurons"]; // output layer
        }
        public void main(object sender, EventArgs e)
        {


            Vector offset = VisualTreeHelper.GetOffset(inputRect);
            Vector offset2 = VisualTreeHelper.GetOffset(btn2);

            Console.WriteLine("left = " + offset.X);
            Console.WriteLine("left2 = " + offset2.X);




            var canvasHeight = mainCanvas.ActualHeight;
            var r = new List<Rectangle>
                {
                    rect(pixelSize, 10, canvasHeight / 2 - pixelSize),
                    rect(pixelSize, 10, canvasHeight / 2),
                    rect(pixelSize, 10 + 70, canvasHeight / 2 - pixelSize),
                    rect(pixelSize, 10 + 70, canvasHeight / 2)
                };
            for (int i = 0; i < r.Count; ++i)
            {
                mainCanvas.Children.Add(r[i]);
            }

            layers.Add(new Layer(name : "input"));
            for (int i = 0; i < r.Count; ++i)
            {
                layers[0].neurons.Add(new Neuron(neuronSize));
                Canvas.SetTop(layers[0].neurons[i].ellipse, canvasHeight / 4 * i + canvasHeight / 8 - neuronSize / 2);
                Canvas.SetLeft(layers[0].neurons[i].ellipse, 200);
                mainCanvas.Children.Add(layers[0].neurons[i].ellipse);
            }
            for (int i = 0; i < layers.Count; ++i)
            {

            }
        }



        Rectangle rect(double pixelSize, double left, double right)
        {
            
            Rectangle r = new Rectangle()
            {
                Width = this.pixelSize,
                Height = this.pixelSize,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.White
            };
            Canvas.SetLeft(r, left); // margin
            Canvas.SetTop(r, right);
            return r;
        }

        
    }

    struct Layer
    {
        public string name;
        public List<Neuron> neurons;
        public Layer(string name)
        {
            this.name = name;
            neurons = new List<Neuron>();
        }
    }

    class Neuron
    {
        public double weight;
        public double value;
        public Ellipse ellipse;

        public Neuron(int neuronSize)
        {
            weight = 0;
            value = 0;
            ellipse = new Ellipse() { Width = neuronSize, Height = 52, Fill = Brushes.White, Stroke = Brushes.Black, StrokeThickness = 1 };
        }
    }
}
