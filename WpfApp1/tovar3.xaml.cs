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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для tovar3.xaml
    /// </summary>
    public partial class tovar3 : Window
    {
        public tovar3(string fio)
        {
            InitializeComponent();
            txtFio.Text = string.IsNullOrEmpty(fio) ? "Гость" : fio;

            var context = ShoeStore_SmolkinEntities4Entities.GetContext();
            var list = context.postavwik.AsNoTracking().ToList();
            list.Insert(0, new postavwik { id = -1, name = "Все поставщики" });
            cmbSupplier.ItemsSource = list;
            cmbSupplier.SelectedIndex = 0;

            var curr = ShoeStore_SmolkinEntities4Entities.GetContext()
                        .Products
                        .AsNoTracking()
                        .ToList();

            lview.ItemsSource = curr;

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var curr = ShoeStore_SmolkinEntities4Entities.GetContext().Products.AsNoTracking().ToList();

            if (txtSearch.Text != "")
            {
                curr = curr.Where(x => x.articule.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                       x.description.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            }

            if (cmbSupplier.SelectedIndex > 0)
            {
                var select = cmbSupplier.SelectedItem as postavwik;
                curr = curr.Where(x => x.postavwik == select.id).ToList();
            }

            lview.ItemsSource = curr;
        }

        private void cmbSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var curr = ShoeStore_SmolkinEntities4Entities.GetContext().Products.AsNoTracking().ToList();

            if (txtSearch.Text != "")
            {
                curr = curr.Where(x => x.articule.ToLower().Contains(txtSearch.Text.ToLower()) ||
                                       x.description.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            }

            if (cmbSupplier.SelectedIndex > 0)
            {
                var select = cmbSupplier.SelectedItem as postavwik;
                curr = curr.Where(x => x.postavwik == select.id).ToList();
            }

            lview.ItemsSource = curr;
        }

        private void btnSortAsc_Click(object sender, RoutedEventArgs e)
        {
            var list = (lview.ItemsSource as List<Products>).OrderBy(x => x.count).ToList();
            lview.ItemsSource = list;
        }

        private void btnSortDesc_Click(object sender, RoutedEventArgs e)
        {
            var list = (lview.ItemsSource as List<Products>).OrderByDescending(x => x.count).ToList();
            lview.ItemsSource = list;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cmbSupplier.SelectedIndex = 0;

            var curr = ShoeStore_SmolkinEntities4Entities.GetContext().Products.AsNoTracking().ToList();
            lview.ItemsSource = curr;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow auth = new MainWindow();
            auth.Show();
            this.Close();
        }
    }
}
        
