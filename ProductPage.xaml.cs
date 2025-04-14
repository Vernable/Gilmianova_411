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
    /// </summary>
    public partial class ProductPage : Page
    {
        List<Product> TableList;
        
        public ProductPage()
        {
            InitializeComponent();
            var currentProduct = Gilmianova_41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;
            ComboDiscount.SelectedIndex = 0;
        }

        private void UpdateProduct()
        {
            var currentProduct = Gilmianova_41Entities.GetContext().Product.ToList();
            currentProduct = currentProduct.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if (ComboDiscount.SelectedIndex == 1)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 0 && Convert.ToDouble(p.ProductDiscountAmount) <= 9.99)).ToList();
            }
            if (ComboDiscount.SelectedIndex == 2)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 10 && Convert.ToDouble(p.ProductDiscountAmount) <= 14.99)).ToList();
            }
            if (ComboDiscount.SelectedIndex == 3)
            {
                currentProduct = currentProduct.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 15)).ToList();
            }


            if (RButtonDown.IsChecked == true)
            {
                currentProduct = currentProduct.OrderByDescending(p => p.ProductCost).ToList();
            }
            else if (RButtonUp.IsChecked == true)
            {
                currentProduct = currentProduct.OrderBy(p => p.ProductCost).ToList();
            }


            ProductListView.ItemsSource = currentProduct;

            TableList = currentProduct;
            
        }


        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void ComboDiscount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }
    }
   
}
