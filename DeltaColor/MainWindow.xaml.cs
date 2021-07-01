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

namespace DeltaColor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Just to disturbe
        }

        #region Declare universal variable
        // These definations have global access.
        // TextBox can modify slider, slider modify textbox
        // Hue modify R G and B, vice versa

        private bool IsShiftHolding;
        private bool IsDraggingRGB;
        private bool IsDraggingHSL;
        private int ValueR;
        private int ValueG;
        private int ValueB;
        private int ValueA;
        private int ValueH;
        private int ValueS;
        private int ValueL;
        private double ValueAh;
        private String Hex;
        private String HexA;
        private String RGB;
        private String RGBA;
        private String ARGB;
        private String HSL;
        private String HSLA;
        private string CYMK;
        #endregion Declare universal variable

        #region Sliding Action

        private void ChangedValueOfSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /// Algorithm...
            /// Slider of RGBA modifies respective text box.
            /// RGB is converted to hex
            /// to avoid loops like change in rgb update hsl updates rgb updates hsl,
            /// Updating other color system can only be done if user is actively dragging slider.
            /// change in value of slider through code-behind won't update another slider.
            /// problem: what is textbox updates slider? user is not actively dragging...
            
            ValueR = (int)SliderOfR.Value;
                ValueOfR.Text = ValueR.ToString();

                ValueG = (int)SliderOfG.Value;
                ValueOfG.Text = ValueG.ToString();

                ValueB = (int)SliderOfB.Value;
                ValueOfB.Text = ValueB.ToString();

                ValueA = (int)SliderOfA.Value;
                ValueOfA.Text = ValueA.ToString();
            ToHex();
            if (IsDraggingRGB)
            {
                UpdateHSLbyRGB();
            }
                
                ValueHex.Text = HexA;
            
                if (ContrastValue() > 133)
                {
                    ValueHex.Foreground = Brushes.Black;
                } else
                {
                    ValueHex.Foreground = Brushes.White;
                }
            
                PreviewBox.Background = new SolidColorBrush(Color.FromArgb((byte)ValueA, (byte)ValueR, (byte)ValueG, (byte)ValueB));
            

        }

        private void ChangedValueOfSliderH(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /// Algorithm...
            /// Slider of HSLA modifies respective text box.
            /// to avoid loops like change in Hsl update rgb updates hsl updates rgb,
            /// Updating other color system can only be done if user is actively dragging slider.
            /// change in value of slider through code-behind won't update another slider.
            /// problem: what is textbox updates slider? user is not actively dragging...

            if (IsDraggingHSL) { }
            ValueH = (int)SliderOfH.Value;
                ValueOfH.Text = ValueH.ToString();

                ValueS = (int)SliderOfS.Value;
                ValueOfS.Text = ValueS.ToString();

                ValueL = (int)SliderOfL.Value;
                ValueOfL.Text = ValueL.ToString();

                ValueAh = Math.Round(SliderOfAh.Value, 2);
                ValueOfAh.Text = ValueAh.ToString();

                ToHex();
                ValueHex.Text = HexA;


            // Lightness of color determines hex text forground color
                if (ValueL > 50)
                {
                    ValueHex.Foreground = Brushes.Black;
                }
                else
                {
                    ValueHex.Foreground = Brushes.White;
                }

                PreviewBox.Background = new SolidColorBrush(Color.FromArgb((byte)ValueA, (byte)ValueR, (byte)ValueG, (byte)ValueB));

            if (IsDraggingHSL)
            {
                UpdateRGBbyHSL();
                // MessageBox.Show("IsTrue");
            }
        }
        #endregion Sliding Action

        #region Scrolling Action

        // Use scroll wheel in slider for precision

        // ForRGBA slider
        private void RScrolled(object sender, MouseWheelEventArgs e)
        {
            _ = (e.Delta > 0) ? ScrolledUP(SliderOfR) : ScrolledDown(SliderOfR);
        }
        private void GScrolled(object sender, MouseWheelEventArgs e)
        {
            _ = (e.Delta > 0) ? ScrolledUP(SliderOfG) : ScrolledDown(SliderOfG);
        }
        private void BScrolled(object sender, MouseWheelEventArgs e)
        {
            _ = (e.Delta > 0) ? ScrolledUP(SliderOfB) : ScrolledDown(SliderOfB);
        }
        private void AScrolled(object sender, MouseWheelEventArgs e)
        {
            _ = (e.Delta > 0) ? ScrolledUP(SliderOfA) : ScrolledDown(SliderOfA);
        }


        // FOR HSL SLider
        private void HScrolled(object sender, MouseWheelEventArgs e)
        {
            _ = (e.Delta > 0) ? ScrolledUP(SliderOfH) : ScrolledDown(SliderOfH);
        }
        private void SScrolled(object sender, MouseWheelEventArgs e)
        {
            _ = (e.Delta > 0) ? ScrolledUP(SliderOfS) : ScrolledDown(SliderOfS);
        }
        private void LScrolled(object sender, MouseWheelEventArgs e)
        {
            _ = (e.Delta > 0) ? ScrolledUP(SliderOfL) : ScrolledDown(SliderOfL);
        }
        private void AhScrolled(object sender, MouseWheelEventArgs e)
        {
            _ = (e.Delta > 0) ? ScrolledUP(SliderOfAh) : ScrolledDown(SliderOfAh);
        }

        #endregion Scrolling Action

        #region Button Action
        private void Clicked_To_Minimize(object sender, RoutedEventArgs e)
        {
            if (IsShiftHolding == false)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                this.Close();
            }
        }

        private void Clicked_To_Copy(object sender, RoutedEventArgs e)
        {
            if (IsShiftHolding == false)
            {
                Clipboard.SetText(Hex);
            }
            else
            {
                ShiftCopy.Visibility = Visibility.Visible;
            }

        }

        private void Clicked_To_CopyCMYK(object sender, RoutedEventArgs e)
        {
            ToCMYK();
            Clipboard.SetText(CYMK);
        }

        private void Click_On_Preview(object sender, MouseButtonEventArgs e)
        {
            ShiftCopy.Visibility = Visibility.Hidden;
        }

        private void Clicked_To_CopyHex(object sender, RoutedEventArgs e)
        {
            ToHex();
            switch (IsShiftHolding)
            {
                case false:
                    Clipboard.SetText(Hex);
                    break;
                case true:
                    Clipboard.SetText(HexA);
                    break;
            }

        }
        private void Clicked_To_CopyRGB(object sender, RoutedEventArgs e)
        {
            ToRGB();
            switch (IsShiftHolding)
            {
                case false:
                    Clipboard.SetText(RGB);
                    break;
                case true:
                    Clipboard.SetText(RGBA);
                    break;
            }

        }
        private void Clicked_To_CopyARGB(object sender, RoutedEventArgs e)
        {
            ToRGB();
            Clipboard.SetText(ARGB);
        }
        private void Clicked_To_CopyHSL(object sender, RoutedEventArgs e)
        {
            RGBtoHSL();
            switch (IsShiftHolding)
            {
                case false:
                    Clipboard.SetText(HSL);
                    break;
                case true:
                    Clipboard.SetText(HSLA);
                    break;
            }
        }

        private void mover(object sender, MouseButtonEventArgs e)
        {
            // MessageBox.Show("Middle button is clicked");
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
            
        }

        private void TakeScreenshot(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is eye dropper.\nWorking methodes are commented");

            /// Algorithm...
            /// Minimize this window.
            /// take screenshot
            /// open new window and display the image full screen
            /// user clicks on image bu mouse....
            ///      Color below pointer is selected
            ///      updates hex, rgb, hsl, cymk to that color
            /// user shift clicks.... exit screenshot
            /// user held alt... Show color below pointer with hex code
        }

        #endregion Button Action

        #region Processing
        private bool ScrolledUP(Slider e)
        {
            e.Value += 1;
            return true;
        }
        private bool ScrolledDown(Slider e)
        {
            e.Value -= 1;
            return false;
        }

        private void ToHex()
        {
            // convert int to hex code..
            Hex = $"#{ValueR.ToString("X2")}{ValueG.ToString("X2")}{ValueB.ToString("X2")}";
            HexA = $"#{ValueR.ToString("X2")}{ValueG.ToString("X2")}{ValueB.ToString("X2")}{ValueA.ToString("X2")}";
        }

        private void ToCMYK()
        {
            // mathmatical formula stolen striaght from stack overflow
            double dubR, dubG, dubB, c, m, y, k;

            dubR = ValueR / 255.0;
            dubG = ValueG / 255.0;
            dubB = ValueB / 255.0;

            k = 1 - new double[] { dubR, dubG, dubB }.Max();
            c = (1 - dubR - k) / (1 - k);
            m = (1 - dubG - k) / (1 - k);
            y = (1 - dubB - k) / (1 - k);
            CYMK = $"cmyk({Math.Round(c * 100)}%, {Math.Round(m * 100)}%, {Math.Round(y * 100)}%, {Math.Round(k * 100)}%)";
        }

        private void ToRGB()
        {
            RGB = $"rgba({ValueR}, {ValueG}, {ValueB})";
            RGBA = $"rgba({ValueR}, {ValueG}, {ValueB}, {ValueA})";
            ARGB = $"rgba({ValueA}, {ValueR}, {ValueG}, {ValueB})";
        }

        private void RGBtoHSL()
        {
            // mathmatical formula stolen striaght from stack overflow
            double dubR, dubG, dubB, max, min, diff;
            dubR = ValueR / 255.0;
            dubG = ValueG / 255.0;
            dubB = ValueB / 255.0;
            ValueAh = Math.Round(ValueA / 255.0, 2);
            max = new double[] { dubR, dubG, dubB }.Max();
            min = new double[] { dubR, dubG, dubB }.Min();
            diff = max - min;
            // For level
            ValueL = (int)Math.Round((max + min) / 2 * 100);

            if (diff == 0)
            {
                ValueH = 0;
            }
            else if (max == dubR)
            {
                ValueH = (int)Math.Round(60 * (((dubG - dubB) / diff) % 6));
            }
            else if (max == dubG)
            {
                ValueH = (int)Math.Round(60 * (((dubB - dubR) / diff) + 2));
            }
            else
            {
                ValueH = (int)Math.Round(60 * (((dubR - dubG) / diff) + 4));
            }

            // for Saturation
            if (diff != 0)
            {
                ValueS = (int)Math.Round(diff / (1 - Math.Abs(max + min - 1)) * 100);
            }
            else
            {
                ValueS = 0;
            }
            HSL = $"hsla({ValueH}, {ValueS}%, {ValueL}%)";
            HSLA = $"hsla({ValueH}, {ValueS}%, {ValueL}%, {ValueAh})";
        }

        private void UpdateHSLbyRGB()
        {
            RGBtoHSL();
            SliderOfH.Value = ValueH;
            SliderOfS.Value = ValueS;
            SliderOfL.Value = ValueL;
            SliderOfAh.Value = ValueAh;
        }

        private void HSLtoRGB()
        {
            ValueR = (int)Math.Round(HSLtoRGBfunc(0) * 255);
            ValueG = (int)Math.Round(HSLtoRGBfunc(8) * 255);
            ValueB = (int)Math.Round(HSLtoRGBfunc(4) * 255);
            ValueA = (int)Math.Round(ValueAh * 255);
        }

        private double HSLtoRGBfunc(int x)
        {
            // mathmatical formula stolen striaght from wikipedia

            double k = (x + ValueH / 30.0) % 12;
            double min = new double[] {k - 3, 9 - k, 1 }.Min();
            double a = (ValueS/100.0) * Math.Min(ValueL / 100.0, 1 - ValueL / 100.0);

            return (ValueL / 100.0) - (a * Math.Max(-1, min));
        }
        private void UpdateRGBbyHSL()
        {
            HSLtoRGB();
            SliderOfR.Value = ValueR;
            SliderOfG.Value = ValueG;
            SliderOfB.Value = ValueB;
            SliderOfA.Value = ValueA;

        }

        private int ContrastValue()
        {
            return (int)Math.Sqrt((ValueR * ValueR * 0.299) + (ValueG * ValueG * 0.587) + (ValueB * ValueB * 0.114));
            // Yeah.. Somehow, this is rule to know either black or white test is presered on given RGB
            // mathmatical formula stolen striaght from stack overflow
        }
        #endregion Processing

        #region Window Key Press
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.LeftShift:
                    IsShiftHolding = true;
                    break;
                case Key.RightShift:
                    IsShiftHolding = true;
                    break;
            }
        }
        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    IsShiftHolding = false;
                    break;
                case Key.RightShift:
                    IsShiftHolding = false;
                    break;
            }
        }

        private void TextEnterValue(object sender, KeyEventArgs e)
        {
            // enter in one textbox updates all slider and color system
            int TextOfR;
            int TextOfG;
            int TextOfB;
            int TextOfA;
            if (e.Key == Key.Enter)
            {
                if (Int32.TryParse(ValueOfR.Text, out TextOfR))
                {
                    SliderOfR.Value = TextOfR;
                    ValueOfR.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    ValueOfR.BorderBrush = Brushes.Red;
                }

                if (Int32.TryParse(ValueOfG.Text, out TextOfG))
                {
                    SliderOfG.Value = TextOfG;
                    ValueOfG.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    ValueOfG.BorderBrush = Brushes.Red;
                }

                if (Int32.TryParse(ValueOfB.Text, out TextOfB))
                {
                    SliderOfB.Value = TextOfB;
                    ValueOfB.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    ValueOfB.BorderBrush = Brushes.Red;
                }

                if (Int32.TryParse(ValueOfA.Text, out TextOfA))
                {
                    SliderOfA.Value = TextOfA;
                    ValueOfA.BorderBrush = Brushes.Transparent;
                    
                }
                else
                {
                    ValueOfA.BorderBrush = Brushes.Red;
                }
                UpdateHSLbyRGB();
            }
        }

        private void TextEnterValueH(object sender, KeyEventArgs e)
        {
            int TextOfH;
            int TextOfS;
            int TextOfL;
            int TextOfAh;
            if (e.Key == Key.Enter)
            {
                if (Int32.TryParse(ValueOfH.Text, out TextOfH))
                {
                    SliderOfH.Value = TextOfH;
                    ValueOfH.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    ValueOfH.BorderBrush = Brushes.Red;
                }

                if (Int32.TryParse(ValueOfS.Text, out TextOfS))
                {
                    SliderOfS.Value = TextOfS;
                    ValueOfS.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    ValueOfS.BorderBrush = Brushes.Red;
                }

                if (Int32.TryParse(ValueOfL.Text, out TextOfL))
                {
                    SliderOfL.Value = TextOfL;
                    ValueOfL.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    ValueOfL.BorderBrush = Brushes.Red;
                }

                if (Int32.TryParse(ValueOfAh.Text, out TextOfAh))
                {
                    SliderOfAh.Value = TextOfAh;
                    ValueOfAh.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    ValueOfAh.BorderBrush = Brushes.Red;
                }
                UpdateRGBbyHSL();
            }
        }

        // Enter is pressed in hex text box
        private void hexEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
            
                string hex = ValueHex.Text.ToUpper();
                if (hex.StartsWith("#"))
                {
                    hex = hex.Remove(0, 1);
                }
                int length = hex.Length;
                try { 
                    if (length == 3) // fff to ffffff
                    {
                        SliderOfR.Value = Convert.ToInt32(hex.Substring(0, 1) + hex.Substring(0, 1), 16);
                        SliderOfG.Value = Convert.ToInt32(hex.Substring(1, 1) + hex.Substring(1, 1), 16);
                        SliderOfB.Value = Convert.ToInt32(hex.Substring(2, 1) + hex.Substring(2, 1), 16);
                    }
                    else if (length == 4) // ffff to ffffffff
                    {
                        SliderOfR.Value = Convert.ToInt32(hex.Substring(0, 1) + hex.Substring(0, 1), 16);
                        SliderOfG.Value = Convert.ToInt32(hex.Substring(1, 1) + hex.Substring(1, 1), 16);
                        SliderOfB.Value = Convert.ToInt32(hex.Substring(2, 1) + hex.Substring(2, 1), 16);
                        SliderOfA.Value = Convert.ToInt32(hex.Substring(3, 1) + hex.Substring(3, 1), 16);
                    }
                    else if (length == 6)
                    {
                        SliderOfR.Value = Convert.ToInt32(hex.Substring(0, 2), 16);
                        SliderOfG.Value = Convert.ToInt32(hex.Substring(2, 2), 16);
                        SliderOfB.Value = Convert.ToInt32(hex.Substring(4, 2), 16);
                    }
                    else if (length == 8)
                    {
                        SliderOfR.Value = Convert.ToInt32(hex.Substring(0, 2), 16);
                        SliderOfG.Value = Convert.ToInt32(hex.Substring(2, 2), 16);
                        SliderOfB.Value = Convert.ToInt32(hex.Substring(4, 2), 16);
                        SliderOfA.Value = Convert.ToInt32(hex.Substring(6, 2), 16);
                    }
                    ValueHex.BorderBrush = Brushes.Transparent;
                    UpdateHSLbyRGB();
                }
                catch
                {
                    ValueHex.BorderBrush = Brushes.Red;
                }
            }
        }

        #endregion Window Key Press

        #region Radio Buton
        private void VisibleRGB(object sender, RoutedEventArgs e)
        {
            GroupOfRGB.Visibility = Visibility.Visible;
            GroupOfHSL.Visibility = Visibility.Collapsed;
        }
        private void VisibleHSL(object sender, RoutedEventArgs e)
        {
            GroupOfHSL.Visibility = Visibility.Visible;
            GroupOfRGB.Visibility = Visibility.Collapsed;
            
        }
        #endregion Radio Button

        #region Dragging Mode
        private void StartDraggingRGB(object sender, MouseEventArgs e)
        {
            IsDraggingRGB = true;
        }
        private void EndDraggingRGB(object sender, MouseEventArgs e)
        {
            IsDraggingRGB = false;
        }

        private void StartDraggingHSL(object sender, MouseEventArgs e)
        {
            IsDraggingHSL = true;
        }
        private void EndDraggingHSL(object sender, MouseEventArgs e)
        {
            IsDraggingHSL = false;
        }

        #endregion Dragging Mode


    }

}
