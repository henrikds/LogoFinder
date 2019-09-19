using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageAnalysis
{
    class Importer
    {
        public static bool YesNoCancel(string question)
        {
            var result = false;

            // Configure the message box to be displayed
            var messageBoxText = question;
            const string caption = "Question";
            const MessageBoxButton button = MessageBoxButton.YesNoCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            var messageBoxResult = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (messageBoxResult)
            {
                case MessageBoxResult.Yes:
                    result = true;
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
                case MessageBoxResult.None:
                    break;
                case MessageBoxResult.OK:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public static Bitmap ImportImage()
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog pic = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select an image file.",
                FileName = "Picture",
                DefaultExt = ".png",
            };

            // Show open file dialog box
            Nullable<bool> result = pic.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                StreamReader Stream = new StreamReader(pic.FileName);
                Bitmap image = (Bitmap)Bitmap.FromStream(Stream.BaseStream);
                Stream.Close();

                return image;
            }

            else
            {
                string imgPath = ("D:/Dropbox/PHYS117/Logos/Apple/Low/apple2.png");
                Bitmap image = TypesAndFiles.OpenImage(imgPath);
                return image;
            }
        }
    }
}