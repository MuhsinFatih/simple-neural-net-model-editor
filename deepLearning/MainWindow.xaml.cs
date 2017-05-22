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




        Dictionary<string, Vector> offset = new Dictionary<string, Vector>();

        Vector getVector(UIElement of)
        {
            if (of is Neuron) {
                return (Vector)of.TransformToAncestor(mainCanvas).Transform(new Point(neuronSize / 2, neuronSize / 2));
            }
            return (Vector)of.TransformToAncestor(mainCanvas).Transform(new Point(0, 0));
        }
        Vector getVector(UIElement of, UIElement relativeTo)
        {
            if (of is Neuron) {
                return (Vector)of.TransformToAncestor(relativeTo).Transform(new Point(neuronSize / 2, neuronSize / 2));
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
                }
                catch (Exception e) {
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





        // this is where the serious business is done
        void approximate()
        {
            for (int l = 0; l < layers.Count; ++l) {
                for (int n = 0; n < layers[l].neurons.Count; ++n) {
                    var neuron = layers[l].neurons[n];
                    for (int i = 0; i < neuron.links.Count; ++i) {
                        var link = neuron.links[i];
                        neuron.val += link.Weight * link.input.val;
                    }
                }
            }
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

        private void btn_removeLayer_Click(object sender, RoutedEventArgs e)
        {
            grid_addLayer.Visibility = Visibility.Collapsed;
            if (selectedLayerIndex == -1) {
                warn("Select a layer to remove first");
            }
            else {
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
                            if(link.input.parentLayer == selectedLayer) {
                                layer.neurons[i].links.Remove(link);
                                mainCanvas.Children.Remove(link);
                            }
                        }
                    }
                }
                layers.Remove(selectedLayer);
                grid_layers.Children.Remove(selectedLayer);
                layerList.Items.RemoveAt(selectedLayerIndex);
                grid_layers.UpdateLayout();
                foreach (var layer in layers) {
                    foreach (var neuron in layer.neurons) {
                        foreach (var link in neuron.links) {
                            link.Connect(getVector(link.input), getVector(neuron));
                        }
                    }
                }
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
                Neuron neuron = new Neuron() { Width = 52, Height = 52, parentLayer = layer };
                neuron.ellipse.MouseUp += selectNeuron;
                layer.neurons.Add(neuron);
                layer.Content.Children.Add(neuron);
            }
            if (txt_layerName.Text.ToLower() == "auto" || txt_layerName.Text == "") {
                int biggest = -1;
                for (int i = layers.Count - 1; i >= 0; --i) {
                    if (layers[i].Name.StartsWith("layer ") && layers[i].Name.Length > 6 && layers[i].Name[6].ToString().IsInteger()) {
                        int a = int.Parse(layers[i].Name[6].ToString());
                        if (a > biggest)
                            biggest = a;
                    }
                }
                layer.Name = "layer " + (biggest + 1);
            }
            else {
                for (int i = 0; i < layers.Count; ++i) {
                    if (layers[i].Name == layer.Name) {
                        warn("Two layers cannot have the same name");
                        return;
                    }
                }
                layer.Name = txt_layerName.Text;
            }
            layer.Content.MouseUp += selectLayer;


            // add layer
            if (chAddBeforeSelectedLayer.IsChecked.Value) {
                if (selectedLayer == null) { warn("you haven't selected any layer to insert before"); return; }
                grid_layers.Children.Insert(selectedLayerIndex,layer);
                layerList.Items.Insert(selectedLayerIndex, new ListBoxItem() { Content = layer.Name });
                layers.Insert(selectedLayerIndex, layer);
            }
            else {
                grid_layers.Children.Add(layer);
                layerList.Items.Add(new ListBoxItem() { Content = layer.Name });
                layers.Add(layer);
            }
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

            Neuron neuron = (sender as Ellipse).Parent as Neuron;
            ((Ellipse)sender).Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBF7272"));
            if (selectedNeuron[0] == null) {
                deselectPrevious(false);
                selectedNeuron[0] = neuron;
            }
            else {
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
                }
                else {
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
                if(layerListUnselect) layerList.UnselectAll();
                label_selected.Content = "None";
                selectedLayerIndex = -1;
                if(collapse) layer_menu.Visibility = Visibility.Collapsed;
                selectedLayer.selected = false;
                selectedLayer.Content.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (deselectNeurons) {
                if (selectedNeuron[0] != null) selectedNeuron[0].ellipse.Fill = Brushes.White;
                if (selectedNeuron[1] != null) selectedNeuron[1].ellipse.Fill = Brushes.White;
                selectedNeuron = new Neuron[2];
            }
        }
        void selectLayer(object sender, MouseButtonEventArgs e)
        {
            deselectPrevious(collapse: false);
            Layer layer = (Layer)(((sender as UniformGrid).Parent as Grid).Parent);
            selectedLayer = layer;
            for (int i = 0; i < layers.Count; ++i) {
                if (layers[i].Name == layer.Name) {
                    selectedLayerIndex = i;
                    break;
                }
            }
            label_selected.Content = selectedLayer.Name;
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
                if (layers[i].Name == (string)(layerList.SelectedItem as ListBoxItem).Content) {
                    selectedLayer = layers[i];
                    break;
                }
            }
            label_selected.Content = selectedLayer.Name;
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

        #endregion

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.IsInteger();
        }


        private void emptySpace_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == sender) {
                Focus();
                Keyboard.ClearFocus();
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
