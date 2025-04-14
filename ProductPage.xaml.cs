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

namespace Gilmianova_41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml

    public partial class ProductPage : Page
    {
         User currentUser;
        int newOrderID;
        private User _user = null;

        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        public ProductPage(User user)
        {
            InitializeComponent();
            //скрываем кнопку показа заказов
            if (selectedProducts.Count == 0)
                ViewOrderButton.Visibility = Visibility.Collapsed;

            currentUser = user;

            if (user != null)
            {
                NameTextBlock.Text = user.UserName + " " + user.UserSurname + " " + user.UserPatronymic;
                switch (user.UserRole)
                {
                    case 1:
                        RoleTextBlock.Text = "Клиент";
                        break;
                    case 2:
                        RoleTextBlock.Text = "Менеджер";
                        break;
                    case 3:
                        RoleTextBlock.Text = "Администратор";
                        break;


                }
            }
            else
            {
                NameTextBlock.Text = "гость";
                RoleTextBlock.Text = " ";
                Role.Visibility = Visibility.Collapsed;
            }

            var currentProduct = Gilmianova_41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;


            newOrderID = Gilmianova_41Entities.GetContext().Order.ToList().Select(p => p.OrderID).Max() + 1;

            ComboFilter.SelectedIndex = 0;

            int ProductMaxRecords = 0;
            foreach (Product product in currentProduct)
                ProductMaxRecords++;
            TBProductCountMaxRecords.Text = ProductMaxRecords.ToString();

            UpdateProducts();
        }

        private void UpdateProducts()
        {
            var currentProduct = Gilmianova_41Entities.GetContext().Product.ToList();

            //фильтрация
            if (ComboFilter.SelectedIndex == 0)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0)).ToList();
            }
            if (ComboFilter.SelectedIndex == 1)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && Convert.ToInt32(p.ProductDiscountAmount) <= 9.99)).ToList();
            }
            if (ComboFilter.SelectedIndex == 2)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 10 && Convert.ToInt32(p.ProductDiscountAmount) <= 14.99)).ToList();
            }
            if (ComboFilter.SelectedIndex == 3)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 15)).ToList();
            }
            currentProduct = currentProduct.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ProductListView.ItemsSource = currentProduct.ToList();

            //сортировка
            if (RButtonDown.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentProduct.OrderByDescending(p => p.ProductCost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentProduct.OrderBy(p => p.ProductCost).ToList();
            }

            int ProductcountRecords = 0;
            foreach (Product product in currentProduct)
                ProductcountRecords++;
            TBProductCountRecords.Text = ProductcountRecords.ToString();

            if (selectedProducts.Count == 0)
            {
                ViewOrderButton.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        int newOrderId;
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedIndex >= 0)
            {
                List<Order> allOrder = Gilmianova_41Entities.GetContext().Order.ToList();
                List<int> allOrderId = new List<int>();
                foreach (var p in allOrder.Select(x => $"{x.OrderID}").ToList())
                {
                    allOrderId.Add(Convert.ToInt32(p));
                }

                newOrderId = allOrderId.Max() + 1;
                var prod = ProductListView.SelectedItem as Product;

                //int newOrderID = selectedOrderProducts.Last().Order.OrderID;
                var newOrderProd = new OrderProduct();
                newOrderProd.OrderID = newOrderId;

                newOrderProd.ProductArticleNumber = prod.ProductArticleNumber;
                newOrderProd.ProductCount = 1;
                var selOP = selectedOrderProducts.Where(p => Equals(p.ProductArticleNumber, prod.ProductArticleNumber));

                if (selOP.Count() == 0)
                {
                    selectedOrderProducts.Add(newOrderProd);
                    selectedProducts.Add(prod);
                }
                else
                {
                    foreach (OrderProduct p in selectedOrderProducts)
                    {
                        if (p.ProductArticleNumber == prod.ProductArticleNumber)
                            p.ProductCount++;
                    }
                }

                ViewOrderButton.Visibility = Visibility.Visible;
                ProductListView.SelectedIndex = -1;

                UpdateProducts();

            }
        }


        private void ViewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            //selectedProducts = selectedProducts.Distinct().ToList();
            ViewOrderWindow orderWindow = new ViewOrderWindow(selectedOrderProducts, selectedProducts, currentUser);
            orderWindow.ShowDialog();
            UpdateProducts();
        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }
    }
}

