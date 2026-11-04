
using System;
using System.Linq;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class OrdersWindow : Window
    {
        private bool isAdmin;

        public OrdersWindow(bool admin)
        {
            InitializeComponent();

            isAdmin = admin;

            // Кнопки управления доступны только администратору
            btnAdd.Visibility = isAdmin
                ? Visibility.Visible : Visibility.Collapsed;

            btnDelete.Visibility = isAdmin
                ? Visibility.Visible : Visibility.Collapsed;

            LoadOrders();
        }

        // Загрузка списка заказов
        private void LoadOrders()
        {
            try
            {
                var context =
                    ShoeStore_SmolkinEntities4Entities.GetContext();

                var orders = context.order
                    .Include(x => x.status)
                    .Include(x => x.PickupPoints)
                    .Include(x => x.orderlist.Select(y => y.Products))
                    .AsNoTracking()
                    .ToList();

                var result = orders.Select(x =>
                {
                    var firstProduct = x.orderlist.FirstOrDefault();

                    string article = firstProduct != null &&
                                     firstProduct.Products != null
                        ? firstProduct.Products.articule
                        : "Не указан";

                    string address = x.PickupPoints == null
                        ? "Не указан"
                        : (x.PickupPoints.index.HasValue
                            ? x.PickupPoints.index.Value + ", "
                            : "") +
                          (x.PickupPoints.city ?? "") + ", " +
                          (x.PickupPoints.street ?? "") + ", д. " +
                          (x.PickupPoints.dom.HasValue
                            ? x.PickupPoints.dom.Value.ToString()
                            : "");

                    return new OrderViewModel
                    {
                        OrderId = x.id,
                        Article = article,
                        Status = x.status == null
                            ? "Не указан"
                            : x.status.status1,
                        PickupAddress = address,
                        OrderDate = x.dateorder,
                        DeliveryDate = x.datedelivery
                    };
                }).ToList();

                dgOrders.ItemsSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки заказов: " + ex.Message);
            }
        }

        // Добавление заказа
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!isAdmin)
                return;

            AddEditOrderWindow window =
                new AddEditOrderWindow(null);

            if (window.ShowDialog() == true)
                LoadOrders();
        }

        // Редактирование заказа
        private void dgOrders_MouseDoubleClick(
            object sender, MouseButtonEventArgs e)
        {
            if (!isAdmin)
                return;

            OrderViewModel selected =
                dgOrders.SelectedItem as OrderViewModel;

            if (selected == null)
                return;

            var context =
                ShoeStore_SmolkinEntities4Entities.GetContext();

            var orderToEdit = context.order
                .FirstOrDefault(x => x.id == selected.OrderId);

            if (orderToEdit == null)
                return;

            AddEditOrderWindow window =
                new AddEditOrderWindow(orderToEdit);

            if (window.ShowDialog() == true)
                LoadOrders();
        }

        // Удаление заказа
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!isAdmin)
                return;

            OrderViewModel selected =
                dgOrders.SelectedItem as OrderViewModel;

            if (selected == null)
            {
                MessageBox.Show("Выберите заказ для удаления!");
                return;
            }

            if (MessageBox.Show(
                "Вы действительно хотите удалить заказ №" +
                selected.OrderId + "?",
                "Удаление заказа",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                var context =
                    ShoeStore_SmolkinEntities4Entities.GetContext();

                var orderToDelete = context.order
                    .Include(x => x.orderlist)
                    .FirstOrDefault(x => x.id == selected.OrderId);

                if (orderToDelete == null)
                    return;

                // Сначала удаляем состав заказа
                foreach (var item in orderToDelete.orderlist.ToList())
                {
                    context.orderlist.Remove(item);
                }

                // Затем удаляем сам заказ
                context.order.Remove(orderToDelete);

                context.SaveChanges();

                MessageBox.Show("Заказ успешно удалён!");

                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка удаления заказа: " + ex.Message);
            }
        }

        // Обновление списка
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        // Возвращение назад
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
