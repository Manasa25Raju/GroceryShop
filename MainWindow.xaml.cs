using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using GroceryShop.classes;

namespace GroceryShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ProductDetails> products;
        ObservableCollection<DeliveryDetails> delivery;
        ObservableCollection<SalesDetails> sale;

        private bool storeData;

        public MainWindow()
        {
            InitializeComponent();

        }

        private ObservableCollection<ProductDetails> GenerateProductDetails(int cnt)
        {
            var lst = new ObservableCollection<ProductDetails>();
            for (int i = 0; i < cnt; i++)
            {
                lst.Add(new ProductDetails { productId = i, productName = "name", stockAvailable = 0, minStock = 0, price = 0});
            }
            return lst;
        }

        private ObservableCollection<DeliveryDetails> GenerateDeliveryDetails(int cnt)
        {
            var lst = new ObservableCollection<DeliveryDetails>();
            for (int i = 0; i < cnt; i++)
            {
                lst.Add(new DeliveryDetails { deliveryId = i, date = DateTime.Now, supplierName = "", quantityReceived = 0 });
            }
            return lst;
        }

        private ObservableCollection<SalesDetails> GenerateSalesDetails(int cnt)
        {
            var lst = new ObservableCollection<SalesDetails>();
            for (int i = 0; i < cnt; i++)
            {
                lst.Add(new SalesDetails { salesId = i, quantityRequired = 0, totalPrice = 0 });
            }
            return lst;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            delivery = DataStorage.ReadXml<ObservableCollection<DeliveryDetails>>("DeliveryDataTable.xml");
            products = DataStorage.ReadXml<ObservableCollection<ProductDetails>>("ProductDataTable.xml");
            sale = DataStorage.ReadXml<ObservableCollection<SalesDetails>>("SalesDataTable.xml");

            Lbx_products.ItemsSource = products;
            Lbx_product.ItemsSource = products;
        }

        private void Btn_add_Click(object sender, RoutedEventArgs e)
        {
            int c = products.Count - 1;
            var product = new ProductDetails {productId = c +1, productName = "new product", stockAvailable = 0, minStock = 5, price = 0};
            products.Add(product);
            Lbx_products.SelectedItem = product;
            Lbx_products.ScrollIntoView(product);
            DataStorage.WriteXml<ObservableCollection<ProductDetails>>(products, "ProductDataTable.xml");

        }

        private void Tbx_filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = Tbx_filter.Text.ToLower();
            var results = from s in products where s.productName.ToLower().Contains(filter) select s;
            Lbx_products.ItemsSource = results;

        }

        private void Tbx_filterProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = Tbx_filterProduct.Text.ToLower();
            var results = from s in products where s.productName.ToLower().Contains(filter) select s;
            Lbx_product.ItemsSource = results;

        }

        private void Btn_addDelivery_Click(object sender, RoutedEventArgs e)
        {
            if ((Tbx_productName.Text != "") && (Tbx_stockAvailable.Text != "") && (Tbx_minStock.Text != "") && (Tbx_price.Text != "") && (Tbx_supplier.Text != "") && (Tbx_qtyReceived.Text != ""))
            {
                deliveryList();
            }
            else
            {
                if ((Tbx_productName.Text == "") && (Tbx_stockAvailable.Text == "") && (Tbx_minStock.Text == "") && (Tbx_price.Text == "") && (Tbx_supplier.Text == "") && (Tbx_qtyReceived.Text == ""))
                {
                    MessageBox.Show("Please fill all the fields", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                    Tbx_productName.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_productName.BorderThickness = new Thickness(2);

                    Tbx_minStock.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_minStock.BorderThickness = new Thickness(2);

                    Tbx_price.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_price.BorderThickness = new Thickness(2);

                    Tbx_stockAvailable.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_stockAvailable.BorderThickness = new Thickness(2);

                    Tbx_supplier.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_supplier.BorderThickness = new Thickness(2);

                    Tbx_qtyReceived.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_qtyReceived.BorderThickness = new Thickness(2);

                }

                else if (Tbx_price.Text == "0")
                {
                    MessageBox.Show("Price is not set!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Tbx_price.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else if((Tbx_supplier.Text == "") && (Tbx_qtyReceived.Text == ""))
                {
                    MessageBox.Show("Delivery details not set!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Tbx_supplier.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_qtyReceived.BorderBrush = new SolidColorBrush(Colors.Red);

                }
            }
        }

        private void deliveryList()
        {
            int c = delivery.Count - 1;
            int addStock;

            string prdName = Tbx_productName.Text;

            int prdId = (from prd in products where ((prd.productName.Equals(prdName))) select prd.productId).First();

            var del = new DeliveryDetails { deliveryId = c + 1, productName = Tbx_productName.Text, date = DateTime.Now, supplierName = Tbx_supplier.Text, quantityReceived = int.Parse(Tbx_qtyReceived.Text)};
            delivery.Add(del);
            Lbx_delivery.SelectedItem = del;
            Lbx_delivery.ItemsSource = delivery;

            DataStorage.WriteXml<ObservableCollection<DeliveryDetails>>(delivery, "DeliveryDataTable.xml");

            int getstockAvailable = (from prd in products where ((prd.productId.Equals(prdId))) select prd.stockAvailable).First();

            addStock = getstockAvailable + int.Parse(Tbx_qtyReceived.Text);

            var updateStock = from prd in products
                              where prd.productId == prdId
                              select prd;
            foreach (var add in updateStock)
                add.stockAvailable = addStock;


            Tbx_productName.Text = "";
            Tbx_stockAvailable.Text = "";
            Tbx_minStock.Text = "";
            Tbx_price.Text = "";
            Tbx_supplier.Text = "";
            Tbx_qtyReceived.Text = "";
            storeData = true;

        }

        private void Btn_addCart_Click(object sender, RoutedEventArgs e)
        {
            if ((Tbx_stkAvailable.Text != "") && (Tbx_pricePerUnit.Text != "") && (Tbx_qtyRequired.Text != "") && (Tbx_totalPrice.Text != ""))
            {
                salesList();
            }
            else
            {
                if ((Tbx_productName.Text == "") && (Tbx_stkAvailable.Text == "") && (Tbx_pricePerUnit.Text == "") && (Tbx_qtyRequired.Text == "") && (Tbx_totalPrice.Text == ""))
                {
                    MessageBox.Show("Please fill all the fields", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                    Tbx_qtyRequired.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_qtyRequired.BorderThickness = new Thickness(2);

                    Tbx_totalPrice.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_totalPrice.BorderThickness = new Thickness(2);

                }
                else if ((Tbx_qtyRequired.Text == "") && (Tbx_totalPrice.Text == ""))
                {
                    MessageBox.Show("Sales details not set!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Tbx_qtyRequired.BorderBrush = new SolidColorBrush(Colors.Red);
                    Tbx_totalPrice.BorderBrush = new SolidColorBrush(Colors.Red);

                }
            }
        }
        private void salesList()
        {
            int c = sale.Count - 1;
            int addStock;
            string prdName = Tbl_prdName.Text;

            int prdId = (from prd in products where ((prd.productName.Equals(prdName))) select prd.productId).First();

            var sales = new SalesDetails { salesId = c + 1, productName = Tbl_prdName.Text, quantityRequired = int.Parse(Tbx_qtyRequired.Text), totalPrice = int.Parse(Tbx_totalPrice.Text)};
            sale.Add(sales);
            Lbx_sales.SelectedItem = sales;
            Lbx_sales.ItemsSource = sale;
            DataStorage.WriteXml<ObservableCollection<SalesDetails>>(sale, "SalesDataTable.xml");

            int getstockAvailable = (from prd in products where ((prd.productId.Equals(prdId))) select prd.stockAvailable).First();

            addStock = getstockAvailable - int.Parse(Tbx_qtyRequired.Text);

            var updateStock = from prd in products
                              where prd.productId == prdId
                              select prd;
            foreach (var add in updateStock)
                add.stockAvailable = addStock;

            Tbx_qtyRequired.Text = "";
            Tbx_totalPrice.Text = "";

            storeData = true;
        }
        private void Tbx_qtyRequired_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                int val1 = int.Parse(Tbx_qtyRequired.Text);
                int val2 = int.Parse(Tbx_pricePerUnit.Text);
                int val3 = val1 * val2;
                Tbx_totalPrice.Text = val3.ToString();
            }
        }

        private void Btn_deleteDelivery_Click(object sender, RoutedEventArgs e)
        {
            if (Lbx_delivery.SelectedItem == null)
            {
                MessageBox.Show("please select an item to be deleted!!", "error");
                return;
            }

            if (MessageBox.Show("Are you sure want to delete?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                delivery.Remove(Lbx_delivery.SelectedItem as DeliveryDetails);
            }
        }

        private void Btn_deleteSales_Click(object sender, RoutedEventArgs e)
        {
            if (Lbx_sales.SelectedItem == null)
            {
                MessageBox.Show("please select an item to be deleted!!", "error");
                return;
            }

            if (MessageBox.Show("Are you sure want to delete? ", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                sale.Remove(Lbx_sales.SelectedItem as SalesDetails);
            }
        }

        private void Tbx_stockAvailable_TextChanged(object sender, TextChangedEventArgs e)
        {
            storeData = true;
        }

        private void Tbx_minStock_TextChanged(object sender, TextChangedEventArgs e)
        {
            storeData = true;
            if (System.Text.RegularExpressions.Regex.IsMatch(Tbx_minStock.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                Tbx_minStock.Text = Tbx_minStock.Text.Remove(Tbx_minStock.Text.Length - 1);
            }

        }
        private void Tbx_qtyRequired_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Tbx_qtyRequired.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                Tbx_qtyRequired.Text = Tbx_qtyRequired.Text.Remove(Tbx_qtyRequired.Text.Length - 1);
            }
            if (int.Parse(Tbx_stkAvailable.Text) == 0)
            {
                MessageBox.Show("No stock!!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Tbx_price_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Tbx_price.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                Tbx_price.Text = Tbx_price.Text.Remove(Tbx_price.Text.Length - 1);
            }
        }

        private void Tbx_qtyReceived_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Tbx_qtyReceived.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                Tbx_qtyReceived.Text = Tbx_qtyReceived.Text.Remove(Tbx_qtyReceived.Text.Length - 1);
            }
        }

        private void Tbx_productName_TextChanged(object sender, TextChangedEventArgs e)
        {
            storeData = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (storeData)
            {
                DataStorage.WriteXml<ObservableCollection<ProductDetails>>(products, "ProductDataTable.xml");
                DataStorage.WriteXml<ObservableCollection<DeliveryDetails>>(delivery, "DeliveryDataTable.xml");
                DataStorage.WriteXml<ObservableCollection<SalesDetails>>(sale, "SalesDataTable.xml");
            }
        }
    }
}
