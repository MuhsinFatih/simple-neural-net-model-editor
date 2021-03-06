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
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32;


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

        




        Dictionary<string, Vector> offset = new Dictionary<string, Vector>();

        Vector getVector(UIElement of)
        {
            if (of is Neuron) {
                double offset = (((of as Neuron).ActualHeight) - ((of as Neuron).txt_val.Parent as Viewbox).ActualHeight) / 2 + ((of as Neuron).txt_val.Parent as Viewbox).ActualHeight;
                return (Vector)of.TransformToAncestor(mainCanvas).Transform(new Point((of as Neuron).ActualWidth / 2, offset));
            }
            return (Vector)of.TransformToAncestor(mainCanvas).Transform(new Point(0, 0));
        }
        Vector getVector(UIElement of, UIElement relativeTo)
        {
            if (of is Neuron) {
                double offset = (((of as Neuron).ActualHeight) - ((of as Neuron).txt_val.Parent as Viewbox).ActualHeight) / 2 + ((of as Neuron).txt_val.Parent as Viewbox).ActualHeight;
                return (Vector)of.TransformToAncestor(relativeTo).Transform(new Point((of as Neuron).ActualWidth / 2, offset));
            }
            return (Vector)of.TransformToAncestor(relativeTo).Transform(new Point(0, 0));
        }
        Vector getVector(UIElement of, UIElement relativeTo, Point offset)
        {
            return (Vector)of.TransformToAncestor(relativeTo).Transform(offset);
        }
        Vector getVector(UIElement of, Point offset)
        {
            return (Vector)of.TransformToAncestor(mainCanvas).Transform(offset);
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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

        void addLink(Link link, Neuron to, Canvas drawOn)
        {
            to.links.Add(link);
            if (drawOn != null) {
                try {
                    drawOn.Children.Add(link);
                    Panel.SetZIndex(link, -1);
                } catch (Exception e) {
                    if (e.HResult != -2147024809) {
                        System.Windows.MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    // else: canvas already has the child. No need to add it again
                }

            }
        }

        List<Rectangle> demoInput = new List<Rectangle>();
        List<Layer> layers = new List<Layer>();

        public void main(object sender, EventArgs e)
        {
            // well, everything is event driven. So yeah, nothing here ^^
        }



        void demo()
        {
            demoInputLayer.Visibility = Visibility.Visible;
            grid_layers.Visibility = Visibility.Hidden;
            demoLayers.Visibility = Visibility.Visible;

            demoInput.Add(p1); demoInput.Add(p2); demoInput.Add(p3); demoInput.Add(p4);
            foreach (UniformGrid item in demoLayers.Children) {
                //layers.Add(new Layer(item.Name, item));
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





        private void btn_run_Click(object sender, RoutedEventArgs e)
        {
            approximate();
        }

        Neuron lastresult;
        // this is where the serious business is done
        void approximate()
        {
            //remove all values except for input layer (first layer)
            for(int i=1; i < layers.Count; ++i) {
                foreach (var neuron in layers[i].neurons) {
                    neuron.Value = 0;
                }
            }
            if (lastresult != null) lastresult.Background = null;
            for (int l = 0; l < layers.Count; ++l) {
                for (int n = 0; n < layers[l].neurons.Count; ++n) {
                    var neuron = layers[l].neurons[n];
                    for (int i = 0; i < neuron.links.Count; ++i) {
                        var link = neuron.links[i];
                        neuron.Value += link.Weight * link.input.output();
                    }
                    neuron.displayColor();
                }
            }
            var biggest = 0D;
            var biggestIndex = 0;
            var layer = layers[layers.Count - 1];
            for (int i=0;i<layer.neurons.Count;++i) {
                if (layer.neurons[i].Value > biggest) {
                    biggest = layer.neurons[i].Value;
                    biggestIndex = i;
                }
            }
            layer.neurons[biggestIndex].Background = new SolidColorBrush(Color.FromArgb(50, 255,255,255));
            lastresult = layer.neurons[biggestIndex];
        }

        void warn(string message)
        {
            MessageBox.Show(message, "deepLearning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        #region UI buttons

        private void btn_addLayer_Click(object sender, RoutedEventArgs e)
        {
            grid_addLayer.Visibility = Visibility.Visible;
        }

        void removeLayer(Layer selectedLayer)
        {
            var selectedLayerIndex = layers.IndexOf(selectedLayer);
            grid_addLayer.Visibility = Visibility.Collapsed;
            for (int i = 0; i < selectedLayer.neurons.Count; ++i) {
                for (int k = 0; k < selectedLayer.neurons[i].links.Count; ++k) {
                    var links = selectedLayer.neurons[i].links;
                    mainCanvas.Children.Remove(links[k]);
                }
            }
            if (layers.Count > selectedLayerIndex + 1) {
                var layer = layers[selectedLayerIndex + 1];
                for (int i = 0; i < layer.neurons.Count; ++i) {
                    foreach (var link in layer.neurons[i].links.ToList()) {
                        if (link.input.parentLayer == selectedLayer) {
                            layer.neurons[i].links.Remove(link);
                            mainCanvas.Children.Remove(link);
                        }
                    }
                }
            }
            layers.Remove(selectedLayer);
            grid_layers.Children.Remove(selectedLayer);
            layerList.Items.RemoveAt(selectedLayerIndex);
            reconnectLinks();
        }
        void reconnectLinks()
        {
            grid_layers.UpdateLayout();
            foreach (var layer in layers) {
                foreach (var neuron in layer.neurons) {
                    neuron.txt_val.IsEnabled = false;
                    foreach (var link in neuron.links) {
                        link.Connect(getVector(link.input), getVector(neuron));
                    }
                }
            }
            if (layers.Count > 0) {
                foreach (var neuron in layers[0].neurons) {
                    neuron.txt_val.IsEnabled = true;
                }
            }
        }
        private void btn_removeLayer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLayerIndex == -1) {
                warn("Select a layer to remove first");
            } else {
                removeLayer(selectedLayer);
            }
        }
        /*
        private void btn_addNeuron_Click(object sender, RoutedEventArgs e) {

        }

        private void btn_removeNeuron_Click(object sender, RoutedEventArgs e) {

        }
        */



        private void btn_addLayerDone_Click(object sender, RoutedEventArgs e)
        {
            var layer = new Layer();
            int ic;
            if (!int.TryParse(txt_neuronCount.Text, out ic)) {
                warn("Please enter the number of neurons you want this layer to have");
                return;
            }

            for (int i = 0; i < ic; ++i) {
                Neuron neuron = new Neuron() { parentLayer = layer };
                neuron.ellipse.MouseLeftButtonUp += selectNeuron;
                layer.neurons.Add(neuron);
                layer.Content.Children.Add(neuron);
            }
            if (txt_layerName.Text.ToLower() == "auto" || txt_layerName.Text == "") {
                int biggest = -1;
                for (int i = layers.Count - 1; i >= 0; --i) {
                    if (layers[i].layerName.StartsWith("layer ") && layers[i].layerName.Length > 6 && layers[i].layerName[6].ToString().IsInteger()) {
                        int a = int.Parse(layers[i].layerName[6].ToString());
                        if (a > biggest)
                            biggest = a;
                    }
                }
                layer.layerName = "layer " + (biggest + 1);
            } else {
                for (int i = 0; i < layers.Count; ++i) {
                    if (layers[i].layerName == layer.layerName) {
                        warn("Two layers cannot have the same name");
                        return;
                    }
                }
                layer.layerName = txt_layerName.Text;
            }
            layer.Content.MouseUp += selectLayer;


            // add layer
            if (chAddBeforeSelectedLayer.IsChecked.Value) {
                if (selectedLayer == null) { warn("you haven't selected any layer to insert before"); return; }
                grid_layers.Children.Insert(selectedLayerIndex, layer);
                layerList.Items.Insert(selectedLayerIndex, new ListBoxItem() { Content = layer.layerName });
                layers.Insert(selectedLayerIndex, layer);
            } else {
                grid_layers.Children.Add(layer);
                layerList.Items.Add(new ListBoxItem() { Content = layer.layerName });
                layers.Add(layer);
            }
            reconnectLinks();
        }

        Layer selectedLayer;
        int selectedLayerIndex;

        Neuron[] selectedNeuron = new Neuron[2];

        enum Selected
        {
            layer, neuron
        }
        Selected selected;

        void selectNeuron(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // prevent parent mouse events

            Neuron neuron = ((sender as Ellipse).Parent as Grid).Parent as Neuron;
            ((Ellipse)sender).Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBF7272"));
            if (selectedNeuron[0] == null) {
                deselectPrevious(false);
                selectedNeuron[0] = neuron;
            } else {
                selectedNeuron[1] = neuron;
                Neuron[] neurons = {
                    selectedNeuron[0],
                    selectedNeuron[1]
                };
                deselectPrevious();
                neurons[1] = neuron;
                var n0i = layers.IndexOf(neurons[0].parentLayer);
                var n1i = layers.IndexOf(neuron.parentLayer);
                Link link;
                if (n1i - n0i == 1)
                    link = new Link(getVector(of: neurons[0]), getVector(of: neuron));
                else if (n1i - n0i == -1)
                    link = new Link(getVector(of: neuron), getVector(of: neurons[0]));
                else if (n1i == n0i) {
                    warn("You cannot connect two neurons at same layer!");
                    return;
                } else {
                    warn("Neurons can only be connected to the next layer!");
                    return;
                }
                link.input = neurons[0];
                addLink(link, to: neuron, drawOn: mainCanvas);
                selected = Selected.neuron;
            };
        }

        void deselectPrevious(bool deselectNeurons = true, bool layerListUnselect = true, bool collapse = true)
        {
            if (selectedLayer != null) {
                if (layerListUnselect) layerList.UnselectAll();
                label_selected.Content = "None";
                selectedLayerIndex = -1;
                if (collapse) layer_menu.Visibility = Visibility.Collapsed;
                selectedLayer.selected = false;
                selectedLayer.Content.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (deselectNeurons) {
                if (selectedNeuron[0] != null) selectedNeuron[0].displayColor();
                if (selectedNeuron[1] != null) selectedNeuron[1].displayColor();
                selectedNeuron = new Neuron[2];
            }
        }
        void selectLayer(object sender, MouseButtonEventArgs e)
        {
            deselectPrevious(collapse: false);
            Layer layer = (Layer)(((sender as UniformGrid).Parent as Grid).Parent);
            selectedLayer = layer;
            for (int i = 0; i < layers.Count; ++i) {
                if (layers[i].layerName == layer.layerName) {
                    selectedLayerIndex = i;
                    break;
                }
            }
            label_selected.Content = selectedLayer.layerName;
            layer_menu.Visibility = Visibility.Visible;

            layerList.SelectedIndex = selectedLayerIndex;
            layer.selected = true;
            layer.Content.Background = new SolidColorBrush(Color.FromArgb(25, 0, 0, 0));
            selected = Selected.layer;
        }
        bool layerListMouseUp = true;
        private void layerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (layerList.SelectedValue == null)
                return;
            deselectPrevious(layerListUnselect: false, collapse: false);
            layerListMouseUp = false;
            selectedLayerIndex = layerList.SelectedIndex;
            for (int i = 0; i < layers.Count; ++i) {
                if (layers[i].layerName == (string)(layerList.SelectedItem as ListBoxItem).Content) {
                    selectedLayer = layers[i];
                    break;
                }
            }
            label_selected.Content = selectedLayer.layerName;
            selectedLayer.selected = true;
            selectedLayer.Content.Background = new SolidColorBrush(Color.FromArgb(25, 0, 0, 0));
            selected = Selected.layer;
        }

        private void layerList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!layerListMouseUp) {
                layer_menu.Visibility = Visibility.Visible;
                layerListMouseUp = true;
            }
        }


        private void btn_addLayerCancel_Click(object sender, RoutedEventArgs e)
        {
            grid_addLayer.Visibility = Visibility.Collapsed;
        }

        private void btn_deselectLayer_Click(object sender, RoutedEventArgs e)
        {
            deselectPrevious();
        }

        private void emptySpace_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == sender) {
                Focus();
                Keyboard.ClearFocus();
            }
        }


        class Model
        {

            public List<Layer> layers = new List<Layer>();
            public class Layer
            {
                public string name;
                public List<Neuron> neurons = new List<Neuron>();
            }
            public class Neuron
            {
                public Function function;
                public double value;
                public List<Link> links = new List<Link>();
            }
            public class Link
            {
                public int inputIndex;
                public double weight;
            }
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            if(layers.Count == 0) {
                warn("There is no network to save!");
                return;
            }
            //var txt_model = JsonConvert.SerializeObject(layers); // won't work because there are self referencing objects. + there are UI elements which is nonsense to store in a json object
            var model = new Model();

            for (int l = 0; l < layers.Count; ++l) {
                model.layers.Add(new Model.Layer() { name = layers[l].layerName });
                for (int n = 0; n < layers[l].neurons.Count; ++n) {
                    Neuron neuron = layers[l].neurons[n];
                    model.layers[l].neurons.Add(new Model.Neuron() { function = neuron.function, value = ( l==0 ? neuron.Value : 0 /*only save input layer values*/) });
                    for (int k = 0; k < neuron.links.Count; ++k) {
                        Model.Link link = new Model.Link() {
                            inputIndex = layers[l - 1].neurons.IndexOf(neuron.links[k].input),
                            weight = neuron.links[k].Weight
                        };

                        model.layers.ElementAt(l).neurons[n].links.Add(link);

                    }
                }
            }

            var txt_model = JsonConvert.SerializeObject(model);

            try {
                StreamWriter file;
                if (chCustomPath.IsChecked.Value) {

                    SaveFileDialog d = new SaveFileDialog() {
                        Filter = "Text file (*.txt)|*.txt"
                    };
                    if (d.ShowDialog().Value) {
                        file = new StreamWriter(d.FileName, append: false);
                    } else return;

                } else
                    file = new StreamWriter(@"model.txt", append: false);

                file.Write(txt_model);
                file.Close();
            } catch (Exception ex) {
                warn(ex.Message);
                return;
            }
            MessageBox.Show("Model saved!", "deepLearning", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            string txt_model;
            try {
                if (chCustomPath.IsChecked.Value) {
                    OpenFileDialog d = new OpenFileDialog() {
                        Filter = "Text file (*.txt)|*.txt"
                    };
                    if (d.ShowDialog().Value) {
                        txt_model = File.ReadAllText(d.FileName);
                    } else return;
                } else
                    txt_model = File.ReadAllText(@"model.txt");
            } catch (Exception ex) {
                warn(ex.Message);
                return;
            }
            Model model = JsonConvert.DeserializeObject<Model>(txt_model);

            resetNetwork();
            for (int l = 0; l < model.layers.Count; ++l) {
                var modelLayer = model.layers[l];
                var layer = new Layer() { layerName = modelLayer.name };
                for (int n = 0; n < modelLayer.neurons.Count; ++n) {
                    var modelNeuron = modelLayer.neurons[n];
                    var neuron = new Neuron() { parentLayer = layer, function = modelNeuron.function, Value = modelNeuron.value };
                    for (int k = 0; k < modelNeuron.links.Count; ++k) {
                        var modelLink = modelNeuron.links[k];
                        var link = new Link() { Weight = modelLink.weight };
                        link.input = layers[l - 1].neurons[modelLink.inputIndex];
                        neuron.links.Add(link);

                        Panel.SetZIndex(link, -1);
                        mainCanvas.Children.Add(link);
                    }
                    neuron.ellipse.MouseLeftButtonUp += selectNeuron;
                    layer.neurons.Add(neuron);
                    layer.Content.Children.Add(neuron);
                }
                layers.Add(layer);
                layerList.Items.Add(new ListBoxItem() { Content = layer.layerName });
                grid_layers.Children.Add(layer);
            }
            reconnectLinks();
            mainCanvas.UpdateLayout();
        }
        #endregion

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.IsInteger();
        }
        void resetNetwork()
        {
            foreach (var layer in layers.ToList()) {
                removeLayer(layer);
            }
            mainCanvas.UpdateLayout();
        }
        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            resetNetwork();
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
