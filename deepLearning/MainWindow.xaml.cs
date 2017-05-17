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
            







        }
        private void onMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null)
                return;
            DragDrop.DoDragDrop(element, new DataObject(this), DragDropEffects.Move);
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello");
            Line line = new Line();
            myCanvas.Children.Add(line);
            line.Stroke = Brushes.Red;
            line.StrokeThickness = 2;
            line.X1 = 0;
            line.Y1 = 0;
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(line.Y2, 100, new Duration(new TimeSpan(0, 0, 1)));
            DoubleAnimation da1 = new DoubleAnimation(line.X2, 100, new Duration(new TimeSpan(0, 0, 1)));
            Storyboard.SetTargetProperty(da, new PropertyPath("(Line.Y2)"));
            Storyboard.SetTargetProperty(da1, new PropertyPath("(Line.X2)"));
            sb.Children.Add(da);
            sb.Children.Add(da1);

            line.BeginStoryboard(sb);


        }



    }
}
