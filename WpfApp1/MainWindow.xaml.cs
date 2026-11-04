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
using System.Xml.Linq;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = ShoeStore_SmolkinEntities4Entities.GetContext();
                var userobj = context.Users.FirstOrDefault(x => x.login == txtLogin.Text && x.password == Password.Password);

                if (userobj == null)
                {
                    MessageBox.Show("Такого пользователя нет!", "Ошибка Авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var dbFio = context.Database.SqlQuery<string>("SELECT fio FROM Users WHERE id = " + userobj.id).FirstOrDefault();
                    string finalFio = string.IsNullOrEmpty(dbFio) ? "Пользователь" : dbFio;

                    switch (userobj.roleid)
                    {
                        case 1:
                            MessageBox.Show("Здравствуйте Администратор " + finalFio + "!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                            tovar adminPage = new tovar(finalFio);
                            this.Close();
                            adminPage.ShowDialog();
                            break;
                        case 2:
                            MessageBox.Show("Здравствуйте Менеджер " + finalFio + "!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                            tovar2 managerPage = new tovar2(finalFio);
                            this.Close();
                            managerPage.ShowDialog();
                            break;
                        case 3:
                            MessageBox.Show("Здравствуйте Авторизированный клиент " + finalFio + "!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                            tovar3 clientPage = new tovar3(finalFio);
                            this.Close();
                            clientPage.ShowDialog();
                            break;
                        default:
                            MessageBox.Show("Данные не обнаружены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ошибка " + Ex.Message.ToString() + " Критическая работа приложения!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tovar3 guestPage = new tovar3("Гость");
            this.Close();
            guestPage.ShowDialog();
        }
    }
}

            
    