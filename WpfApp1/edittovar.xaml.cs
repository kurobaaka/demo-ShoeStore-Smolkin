using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для edittovar.xaml
    /// </summary>
    public partial class edittovar : Window

    {
        private Products _currentPaym = new Products();
      
        public edittovar(Products selPaym)
        {
            InitializeComponent();
            var context = ShoeStore_SmolkinEntities4Entities.GetContext();
            cmbNameProduct.ItemsSource = context.nameproduct.ToList();
            cmbCategory.ItemsSource = context.categoryid.ToList();
            cmbProizvod.ItemsSource = context.nameproizvod.ToList();
            cmbPostavwik.ItemsSource = context.postavwik.ToList();

            if (selPaym != null)
            {
                _currentPaym = context.Products.Find(selPaym.id);
            }

            DataContext = _currentPaym;

            
            
        }

        private void SelectImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string targetDirectory = @"C:\Users\psykh\Downloads\dd\";
                string oldPath = System.IO.Path.Combine(targetDirectory, _currentPaym.PHOTO ?? "");

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".png";
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (dialog.ShowDialog() == true)
                {
                    string sourceFilePath = dialog.FileName;
                    string _photoPath = dialog.SafeFileName.ToString().ToLower();
                    FileNameTB.Text = _photoPath;

                    string newPath = System.IO.Path.Combine(targetDirectory, _photoPath);

                    PhotoTovarIm.Source = null;

                    if (sourceFilePath.ToLower() != newPath.ToLower())
                    {
                        if (!File.Exists(newPath))
                        {
                            File.Copy(sourceFilePath, newPath, true);
                        }
                    }

                    _currentPaym.PHOTO = _photoPath;

                    BitmapImage image = new BitmapImage();
                    using (FileStream stream = new FileStream(newPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                    }
                    PhotoTovarIm.Source = image;

                    ShoeStore_SmolkinEntities4Entities.GetContext().SaveChanges();
                    MessageBox.Show("Изображение товара успешно установлено!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка работы программы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if ((_currentPaym.cost ?? 0) < 0 || (_currentPaym.count ?? 0) < 0)
            {
                MessageBox.Show("Стоимость и минимальное количество не могут принимать отрицательные значения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var context = ShoeStore_SmolkinEntities4Entities.GetContext();

                if (_currentPaym.id == 0)
                {
                    context.Products.Add(_currentPaym);
                }

                context.SaveChanges();
                MessageBox.Show("Запись сохранена");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
}



