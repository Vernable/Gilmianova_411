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

namespace Gilmianova_41
{
    /// <summary>
    /// Логика взаимодействия для ViewOrderWindow.xaml
    /// </summary>
    public partial class ViewOrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();

        private Order currentOrder = new Order();
        //private OrderProduct currentOrderProduct = new OrderProduct();

        User _user = null;

        public ViewOrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, User user)
        {

            InitializeComponent();

            _user = user;
            var currentPickUpPoints = Gilmianova_41Entities.GetContext().PickUpPoint.ToList();

            PickupCombo.ItemsSource = currentPickUpPoints;
            PickupCombo.SelectedIndex = 0;

            //OrderIDTB.Text = selectedOrderProducts.First().OrderID.ToString();
            int currentID = selectedOrderProducts.First().OrderID;
            currentOrder.OrderID = currentID; //записываем айди в БД

            if (_user == null)
            {
                currentOrder.OrderClient = null;
                ClientNameTB.Text = "Гость";
            }
            else
            {
                currentOrder.OrderClient= user.UserID;
                ClientNameTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
            }
            foreach (Product p in selectedProducts)
            {
                p.Quantity = 1;//Quantity - столбец таблицы product которой нет в бд
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                    {
                        p.Quantity = q.ProductCount;
                    }
                }
            }


            ProductListView.ItemsSource = selectedProducts;

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            OrderDate.Text = DateTime.Now.ToString();
            OrderIDTB.Text = currentID.ToString();

            SetDeliveryDate();
            SumOfOrder(selectedProducts);
        }

        private void SetDeliveryDate()
        {

            bool hasLowStock = false; // Флаг для проверки наличия товаров <3 шт.

            // Проверяем каждый продукт в заказе
            foreach (var product in selectedProducts)
            {
                if (product.ProductQuantityInStock < 3) // Если количество на складе <3
                {
                    hasLowStock = true;
                    break; // Выходим из цикла при первом нарушении
                }
            }

            DateTime deliveryDate = OrderDate.SelectedDate.Value;
            deliveryDate = hasLowStock
                ? deliveryDate.AddDays(6) // Если есть товары <3 шт. → +6 дней
                : deliveryDate.AddDays(3); // В противном случае → +3 дня

            OrderDeliveryDateTB.SelectedDate = deliveryDate;
        }

        private void SumOfOrder(List<Product> products)
        {
            double total = 0;
            for (int i = 0; i < products.Count; i++)
            {
                total += (Convert.ToDouble(products[i].ProductCost) - Convert.ToDouble(products[i].ProductCost)
                    * Convert.ToDouble(products[i].ProductDiscountAmount) / 100) * products[i].Quantity;
            }
            PriceText.Text = total.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            currentOrder.OrderDate = DateTime.Now;
            currentOrder.OrderDeliveryDate = OrderDeliveryDateTB.SelectedDate.Value;
            currentOrder.OrderPickupPoint = PickupCombo.SelectedIndex + 1;
            currentOrder.OrderStatus = "Новый";

            for (int i = 0; i < selectedProducts.Count; i++)
            {
                if (selectedProducts[i].ProductQuantityInStock >= selectedOrderProducts[i].ProductCount)
                    selectedProducts[i].ProductQuantityInStock -= selectedOrderProducts[i].ProductCount;
                else
                    selectedProducts[i].ProductQuantityInStock = 0;
            }

            foreach (var p in selectedOrderProducts)
            {
                Gilmianova_41Entities.GetContext().OrderProduct.Add(p);
            }

            Gilmianova_41Entities.GetContext().Order.Add(currentOrder);



            List<Order> allOrderCodes = Gilmianova_41Entities.GetContext().Order.ToList();
            List<int> OrderCodes = new List<int>();
            //записываем в массив инт все имеющиеся коды
            foreach (var p in allOrderCodes.Select(x => $"{x.OrderCode}").ToList())
            {
                OrderCodes.Add(Convert.ToInt32(p));
            }
            //обьект рандом
            Random random = new Random();

            while (true)
            {
                int num = random.Next(100, 1000);
                //проверяем сгенерированный код на уникальность
                if (!OrderCodes.Contains(num))
                {
                    currentOrder.OrderCode = num;
                    break;
                }
            }



            foreach (var op in selectedOrderProducts)
            {
                op.OrderID = currentOrder.OrderID;
                Gilmianova_41Entities.GetContext().OrderProduct.Add(op);
            }
            try
            {

                Gilmianova_41Entities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                selectedProducts.Clear();
                selectedOrderProducts.Clear();
                OrderIDTB.Text = currentOrder.OrderID.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            this.Close();
        }



        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity++;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].ProductCount++;
            ProductListView.Items.Refresh();

            SetDeliveryDate();
            SumOfOrder(selectedProducts);

        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {

            var prod = (sender as Button).DataContext as Product;
            prod.Quantity--;
            var selectedOP = selectedProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedProducts.IndexOf(selectedOP);

            if (prod.Quantity == 0)
            {
                selectedOrderProducts[index].ProductCount = 0;
                var pr = ProductListView.SelectedItem as Product;
                selectedOrderProducts.RemoveAt(index);
                selectedProducts.RemoveAt(index);
                if (ProductListView.Items.Count == 0)
                {
                    this.Close();
                }
            }
            else
            {
                selectedOrderProducts[index].ProductCount--;
            }
            SetDeliveryDate();
            SumOfOrder(selectedProducts); // Пересчитываем сумму заказа
            ProductListView.Items.Refresh();

            if (ProductListView.Items.Count == 0)
                this.Close();
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity = 0;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].ProductCount = 0;
            var pr = ProductListView.SelectedItem as Product;
            selectedOrderProducts.RemoveAt(index);
            selectedProducts.RemoveAt(index);
            ProductListView.Items.Refresh();
            SetDeliveryDate();

            SumOfOrder(selectedProducts);
            if (ProductListView.Items.Count == 0)
            {
                this.Close();
            }
        }
    }
}

