using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace ImageAnalysis
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

        private int _edgeLimitImage = 20;
        private int _edgeLimitLogo = 20;
        private double _hitCap = 0.5;
        private bool _grayScale;
        public static List<double> HitRatesPercent = new List<double>();

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var newImage = Importer.ImportImage();

            var filtredImage = EdgeOrientation.EdgeDetectBitmap(newImage,
                Filters.Sobel3x3Horizontal, Filters.Sobel3x3Vertical, _edgeLimitImage,
                true);

            Image1.Source = TypesAndFiles.BitmapToImageSource(filtredImage);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            var newImage = Importer.ImportImage();

            var squaredImage = Shapes.SquaredImage(newImage, _edgeLimitImage, _grayScale);

            Image1.Source = TypesAndFiles.BitmapToImageSource(squaredImage);
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            var newImage = Importer.ImportImage();
            var newLogo = Importer.ImportImage();

            var filteredImage = Shapes.ShapeComparison(newImage, newLogo, _edgeLimitImage,
                _edgeLimitLogo, _hitCap, _grayScale);

            ListView.Items.Clear();
            foreach (var percent in HitRatesPercent)
            {
                ListView.Items.Add(percent.ToString("00.00%"));
            }

            Image1.Source = TypesAndFiles.BitmapToImageSource(filteredImage);
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            _edgeLimitImage = int.Parse(EdgeLimitImage.Text);
            _edgeLimitLogo = int.Parse(EdgeLimitLogo.Text);
            _hitCap = double.Parse(HitPercentCap.Text) / 100;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _grayScale = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _grayScale = false;
        }
    }
}