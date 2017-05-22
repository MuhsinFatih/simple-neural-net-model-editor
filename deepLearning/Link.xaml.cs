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

namespace deepLearning
{
    /// <summary>
    /// Interaction logic for Link.xaml
    /// </summary>
    public partial class Link : UserControl
    {
		double weight;
		public double Weight {
			get {
				return this.weight;
			}
			set {
				label_weight.Text = value.ToString();
				Brush brush = new SolidColorBrush(grayScale(value, -1, 1));
				border.BorderBrush = brush;
				line.Stroke = brush;
				this.weight = value;
			}
		}
		private Color grayScale(double val, double from, double upTo) {
			upTo -= from;
			val -= from;
			from = 0;
			byte rgb = (byte)((val / upTo) * 255);
			return Color.FromRgb(rgb, rgb, rgb);
		}

		public Neuron input;

#region constructors
        public Link() {
            InitializeComponent();
			this.MouseEnter += delegate {
				border.Visibility = Visibility.Visible;
                line.StrokeThickness = 2;
			};
			this.MouseLeave += delegate {
				border.Visibility = Visibility.Hidden;
                line.StrokeThickness = 1;
            };
			
        }
		public Link(double weight) : this() {
			this.Weight = weight;
		}
		public Link(Vector node1, Vector node2) : this(){
			Connect(node1, node2);
			this.Weight = 0;
		}
		public Link(Vector node1, Vector node2, double weight) : this() {
			Connect(node1, node2);
			this.Weight = weight;
		}

		#endregion

		public void Connect(Vector node1, Vector node2) {
			Vector diff = node2 - node1;
			if (diff.X < 0) {
				MessageBox.Show("You cannot connect a neuron to a parent neuron!","Error",MessageBoxButton.OK,MessageBoxImage.Exclamation);
			}
			double w = Math.Abs(diff.X),
				h = Math.Abs(diff.Y);

			double angle = Math.Atan(diff.Y / diff.X) * 180 / Math.PI;
			this.RenderTransform = new RotateTransform(angle);
			border.RenderTransform = new RotateTransform(-angle);

			
			Canvas.SetLeft(this, node1.X);
			Canvas.SetTop(this, node1.Y - this.Height / 2);


			this.Width = Math.Sqrt(Math.Pow(w,2) + Math.Pow(h,2));
		}

        private void label_weight_GotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            (sender as TextBox).SelectAll();
        }

        private void label_weight_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) {
                Keyboard.ClearFocus();
            }
        }
    }
}
