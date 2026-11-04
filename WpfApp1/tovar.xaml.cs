using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для tovar.xaml
    /// </summary>
    public partial class tovar : Window
    {
        public tovar(string fio)
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
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            edittovar w = new edittovar(null);
            w.ShowDialog();

            var context = ShoeStore_SmolkinEntities4Entities.GetContext();
            context.SaveChanges();
            lview.ItemsSource = context.Products.AsNoTracking().ToList();
        }

        private void lview_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selected = lview.SelectedItem as Products;
            if (selected != null)
            {
                var context = ShoeStore_SmolkinEntities4Entities.GetContext();
                var productToEdit = context.Products.FirstOrDefault(x => x.id == selected.id);

                if (productToEdit != null)
                {
                    edittovar w = new edittovar(productToEdit);
                    w.ShowDialog();

                    lview.ItemsSource = null;
                    lview.ItemsSource = context.Products.AsNoTracking().ToList();
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {

            var pfdel = lview.SelectedItems.Cast<Products>().ToList();
            if (pfdel.Count == 0) return;

            if (MessageBox.Show($"Вы точно хотите удалить сл. элементы ({pfdel.Count()})?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = ShoeStore_SmolkinEntities4Entities.GetContext();

                    foreach (Products p in pfdel)
                    {
                        int orderCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM orderlist WHERE articuleid = " + p.id).FirstOrDefault();
                        if (orderCount > 0)
                        {
                            MessageBox.Show($"Товар с артикулом {p.articule} удалить нельзя, так как он присутствует в заказе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        context.Database.ExecuteSqlCommand("DELETE FROM Products WHERE id = " + p.id);
                    }

                    MessageBox.Show("Данные удалены!");

                    lview.ItemsSource = context.Products.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
     
    
private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow window = new OrdersWindow(true);
            window.ShowDialog();
        }

    } 
}





