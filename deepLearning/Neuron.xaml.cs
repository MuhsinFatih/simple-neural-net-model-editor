using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public Layer parentLayer;
		// input links
		public List<Link> links = new List<Link>();

        Function func;
        
        public Function function {
            get {
                return func;
            }
            set {
                func = value;
                string img;
                switch (func) {
                    case Function.sigmoid:
                        img = "sigmoid.png";
                        break;
                    case Function.relu:
                        img = "relu.png";
                        break;
                    default:
                        return;
                }
                var imgbrush = new ImageBrush();
                imgbrush.ImageSource = new BitmapImage(new Uri(img, UriKind.Relative));
                functionImage.Fill = imgbrush;

            }
        }

        double val = 0;
        public double Value {
            get {
                return val;
            }
            set {
                val = value;
                txt_val.Text = Math.Round(val,3).ToString();
                displayColor();
            }
        }

        
#region constructors
        public Neuron() {
            InitializeComponent();
            this.function = Function.sigmoid;
            displayColor();
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
#endregion
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

        private void ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ellipse.Focus();
        }

        private Color grayScale(double val, double from, double upTo)
        {
            if (val < from) return Colors.Black;
            if (val > upTo) return Colors.White;
            upTo -= from;
            val -= from;
            from = 0;
            byte rgb = (byte)((val / upTo) * 255);
            return Color.FromRgb(rgb, rgb, rgb);
        }

        public void displayColor()
        {
            ellipse.Fill = new SolidColorBrush(grayScale(this.val, -1, 1));
        }

        private void txt_val_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(e.Text.IsInteger() || e.Text == "." || e.Text == "-");
        }

        private void txt_val_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape) {
                Keyboard.ClearFocus();
                setVal(txt_val.Text);
            }
        }
        private void setVal(string value)
        {
            Regex r = new Regex(@"^(\-?[\d\.]*)$");
            if (r.IsMatch(value)) {
                this.Value = double.Parse(value);
                displayColor();
            } else {
                MessageBox.Show("weight must be a number!");
                txt_val.Text = this.Value.ToString();
            }
        }

        private void txt_val_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            setVal(txt_val.Text);
        }

        private void ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.function = (Function)((int)(this.function + 1) % Enum.GetNames(typeof(Function)).Length);
        }
    }

    public enum Function {
		sigmoid,
		relu // rectified linear unit
	}

	

}
