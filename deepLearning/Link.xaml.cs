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
		public double weight {
			get {
				return this.weight;
			}
			set {
				border.BorderBrush = new SolidColorBrush(grayScale(value, -1, 1));
				this.weight = value;
			}
		}
		private Color grayScale(double val, double from, double upTo) {
			from = 0;
			upTo -= from;
			val -= from;
			byte rgb = (byte)((val / upTo) * 255);
			return Color.FromRgb(rgb, rgb, rgb);
		}
        public Link() {
            InitializeComponent();
        }
		public Link(Vector node1, Vector node2) : this(){
			Connect(node1, node2);

		}
		public void Connect(Vector node1, Vector node2) {
			Vector diff = node2 - node1;
			double w = Math.Abs(diff.X),
				h = Math.Abs(diff.Y);

			double angle = Math.Atan(diff.Y / diff.X) * 180 / Math.PI;
			this.RenderTransform = new RotateTransform(angle);
			border.RenderTransform = new RotateTransform(-angle);

			if (diff.X < 0) {
				Vector v = node1;
				node1 = node2;
				node2 = v;
			}
			
			Canvas.SetLeft(this, node1.X);
			Canvas.SetTop(this, node1.Y - this.Height / 2);


			this.Width = Math.Sqrt(Math.Pow(w,2) + Math.Pow(h,2));
		}

		private void UserControl_MouseEnter(object sender, MouseEventArgs e) {
			MessageBox.Show("it does!");
		}
	}
}
