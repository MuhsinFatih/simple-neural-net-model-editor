﻿using System;
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
        //List<Layer> layers = new List<Layer>();


        Dictionary<string, Vector> offset = new Dictionary<string, Vector>();

        Vector getVector(UIElement of, UIElement relativeTo) {
			if(of is Neuron) {
				return (Vector)of.TransformToAncestor(relativeTo).Transform(new Point(neuronSize / 2, neuronSize / 2));
			}
            return (Vector)of.TransformToAncestor(relativeTo).Transform(new Point(0, 0));
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
            offset["grid_neurons"] = getVector(of: grid_neurons, relativeTo: mainCanvas);
            offset["layer0"] = getVector(of: grid_layer0, relativeTo: mainCanvas); // input visual representation layer
            offset["inputRect"] = getVector(of: inputRect, relativeTo: mainCanvas) + offset["layer0"]; // input visual representation rectangle (the pixels)

            // sum vectors to get real positions in canvas
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
        public void main(object sender, EventArgs e) {

			((Neuron)grid_layer1.Children[0]).ellipse.Fill = Brushes.Tomato;
			((Neuron)grid_layer2.Children[1]).ellipse.Fill = Brushes.Green;


			Link link = new Link();
			link.Connect(getVector(of: grid_layer1.Children[1], relativeTo: mainCanvas), getVector(of: grid_layer2.Children[1], relativeTo: mainCanvas));
			mainCanvas.Children.Add(link);
			mainCanvas.Children.Add(new Link(getVector(of: grid_layer1.Children[2], relativeTo: mainCanvas), getVector(of: grid_layer2.Children[3], relativeTo: mainCanvas)));

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

    //struct Layer {
    //    public string name;
    //    public List<Neuron> neurons;
    //    public Layer(string name) {
    //        this.name = name;
    //        neurons = new List<Neuron>();
    //    }
    //}

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
