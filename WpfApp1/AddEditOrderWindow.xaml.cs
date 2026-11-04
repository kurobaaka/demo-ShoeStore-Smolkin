
using System;
using System.Linq;
using System.Windows;

namespace WpfApp1
{
    public partial class AddEditOrderWindow : Window
    {
        private order currentOrder;
        private bool isEdit;

        public AddEditOrderWindow(order selectedOrder)
        {
            InitializeComponent();

            currentOrder = selectedOrder;
            isEdit = selectedOrder != null;

            LoadData();

            if (isEdit)
            {
                txtTitle.Text = "Редактирование заказа";
                LoadOrderData();
            }
            else
            {
                txtTitle.Text = "Добавление заказа";
                dpOrderDate.SelectedDate = DateTime.Today;
                dpDeliveryDate.SelectedDate = DateTime.Today.AddDays(3);
            }
        }

        // Загрузка данных для выпадающих списков
        private void LoadData()
        {
            var context =
                ShoeStore_SmolkinEntities4Entities.GetContext();

            cmbProduct.ItemsSource = context.Products
                .AsNoTracking()
                .OrderBy(x => x.articule)
                .ToList();

            cmbStatus.ItemsSource = context.status
                .AsNoTracking()
                .ToList();

            var pickupList = context.PickupPoints
                .AsNoTracking()
                .ToList();

            cmbPickup.ItemsSource = pickupList
                .Select(x => new
                {
                    Id = x.id,
                    Address =
                        (x.index.HasValue
                            ? x.index.Value + ", "
                            : "") +
                        (x.city ?? "") + ", " +
                        (x.street ?? "") + ", д. " +
                        (x.dom.HasValue
                            ? x.dom.Value.ToString()
                            : "")
                })
                .ToList();

            cmbPickup.DisplayMemberPath = "Address";
            cmbPickup.SelectedValuePath = "Id";
        }

        // Заполнение полей при редактировании
        private void LoadOrderData()
        {
            if (currentOrder == null)
                return;

            cmbStatus.SelectedValue = currentOrder.statusorder;
            cmbPickup.SelectedValue = currentOrder.address;

            dpOrderDate.SelectedDate = currentOrder.dateorder;
            dpDeliveryDate.SelectedDate = currentOrder.datedelivery;

            var context =
                ShoeStore_SmolkinEntities4Entities.GetContext();

            var firstItem = context.orderlist
                .FirstOrDefault(x => x.id_order == currentOrder.id);

            if (firstItem != null)
            {
                cmbProduct.SelectedValue =
                    firstItem.articuleid;
            }
        }

        // Сохранение заказа
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения полей
                if (cmbProduct.SelectedItem == null)
                {
                    MessageBox.Show("Выберите артикул товара!");
                    return;
                }

                if (cmbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус заказа!");
                    return;
                }

                if (cmbPickup.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пункт выдачи!");
                    return;
                }

                if (dpOrderDate.SelectedDate == null ||
                    dpDeliveryDate.SelectedDate == null)
                {
                    MessageBox.Show("Укажите обе даты!");
                    return;
                }

                if (dpDeliveryDate.SelectedDate.Value <
                    dpOrderDate.SelectedDate.Value)
                {
                    MessageBox.Show(
                        "Дата выдачи не может быть раньше даты заказа!");
                    return;
                }

                var context =
                    ShoeStore_SmolkinEntities4Entities.GetContext();

                Products product =
                    cmbProduct.SelectedItem as Products;

                status selectedStatus =
                    cmbStatus.SelectedItem as status;

                int pickupId =
                    (int)cmbPickup.SelectedValue;

                if (!isEdit)
                {
                    // Создание нового заказа
                    currentOrder = new order();

                    currentOrder.dateorder =
                        dpOrderDate.SelectedDate.Value;

                    currentOrder.datedelivery =
                        dpDeliveryDate.SelectedDate.Value;

                    currentOrder.address = pickupId;
                    currentOrder.statusorder = selectedStatus.id;

                    // Заказ создаётся без привязки к покупателю,
                    // поскольку в задании это поле не предусмотрено.

                    context.order.Add(currentOrder);
                    context.SaveChanges();

                    // Добавление товара в состав заказа
                    orderlist newItem = new orderlist();

                    newItem.id_order = currentOrder.id;
                    newItem.articuleid = product.id;
                    newItem.count = 1;

                    context.orderlist.Add(newItem);
                }
                else
                {
                    // Редактирование существующего заказа
                    var orderToEdit = context.order
                        .FirstOrDefault(x => x.id == currentOrder.id);

                    if (orderToEdit == null)
                    {
                        MessageBox.Show("Заказ не найден!");
                        return;
                    }

                    orderToEdit.dateorder =
                        dpOrderDate.SelectedDate.Value;

                    orderToEdit.datedelivery =
                        dpDeliveryDate.SelectedDate.Value;

                    orderToEdit.address = pickupId;
                    orderToEdit.statusorder = selectedStatus.id;

                    // Меняем первую позицию заказа
                    var firstItem = context.orderlist
                        .FirstOrDefault(x =>
                            x.id_order == orderToEdit.id);

                    if (firstItem == null)
                    {
                        firstItem = new orderlist();

                        firstItem.id_order = orderToEdit.id;
                        firstItem.count = 1;

                        context.orderlist.Add(firstItem);
                    }

                    firstItem.articuleid = product.id;
                }

                context.SaveChanges();

                MessageBox.Show(
                    isEdit
                        ? "Заказ успешно изменён!"
                        : "Заказ успешно добавлен!",
                    "Успешно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка сохранения заказа: " + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Отмена
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
