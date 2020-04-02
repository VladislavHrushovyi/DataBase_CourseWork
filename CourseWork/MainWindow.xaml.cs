using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using NpgsqlTypes;

namespace CourseWork
{
    public partial class MainWindow : Window
    {
        SqlConnection SqlConnection;
        SqlDataReader reader;
        SqlCommand sqlCommand;
        List<string> operation = new List<string>() { "ADD", "SELECT", "INSERT", "DELETE" };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void choseSingle_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FornitureManufacture;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                
                SqlConnection = new SqlConnection(connection);
                await SqlConnection.OpenAsync();
                await DataInAllComboBox();
                await FillAllTable();
                await FillAllOperationComboBox();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,ex.Source,MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        private async Task FillAllOperationComboBox()
        {
            CB_operationColors.ItemsSource = operation;
        }

        private async void ColorsDone(object sender, RoutedEventArgs e)
        {
            if (CB_operationColors.SelectedItem.ToString() == "ADD")
            {
                await FillAllTable();
                await DataInAllComboBox();
            }
            else if (CB_operationColors.SelectedItem.ToString() == "SELECT")
            {
            }
            else if (CB_operationColors.SelectedItem.ToString() == "UPDATE")
            {

            }
            else if (CB_operationColors.SelectedItem.ToString() == "DELETE")
            {

            }
        }

        private async void RefreshColors()
        {
            
        }

        /*private async Task ClearAllComboBox()
        {
            CB_personOrder.Items.Clear();
            CB_personClient.Items.Clear();
            CB_chooseDepartament.Items.Clear();
            CB_personWorkerInfo.Items.Clear();
            CB_selectProduct.Items.Clear();
            CB_chooseWorker.Items.Clear();
            CB_chooseDirector.Items.Clear();
            CB_personDirectorInfo.Items.Clear();
            CB_Storage.Items.Clear();
            CB_itemComponent.Items.Clear();
        }*/
        private async Task DataInAllComboBox()
        {
            try
            {
                sqlCommand = new SqlCommand("SELECT Client_id FROM [Orders]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_personOrder.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_personOrder.Items.Add(reader[0].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,SecondName FROM [Client]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_personClient.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_personClient.Items.Add(reader[0].ToString()+" "+reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Departament]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_chooseDepartament.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_chooseDepartament.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,SecondName FROM [WorkerInfo]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_personWorkerInfo.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_personWorkerInfo.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Product]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_selectProduct.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_selectProduct.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id FROM [Workers]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_chooseWorker.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_chooseWorker.Items.Add(reader[0].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id FROM [Directors]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_chooseDirector.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_chooseDirector.Items.Add(reader[0].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,SecondName FROM [DirectorInfo]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_personDirectorInfo.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_personDirectorInfo.Items.Add(reader[0].ToString()+" "+reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Storage]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_Storage.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_Storage.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Component]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_itemComponent.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_itemComponent.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Colors]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_chooseColors.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_chooseColors.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private async void CB_chooseColors_DropDownClosed(object sender, EventArgs e)
        {
            
        }
        private string GetNameSelectItem(string selectItem)
        {
            return " ";
        }
        private string GetIndexFromCombpBox(string item)
        {
            return " ";
        }

        private async Task FillAllTable()
        {
            List<Colors> colorsTable = new List<Colors>();
            List<Orders> ordersTable = new List<Orders>();
            List<Client> clientTable = new List<Client>();
            List<Component> componentTable = new List<Component>();
            List<Departament> departamentTable = new List<Departament>();
            List<DirectorInfo> directorInfoTable = new List<DirectorInfo>();
            List<Directors> directorsTable = new List<Directors>();
            List<Product> productTable = new List<Product>();
            List<Storage> storageTable = new List<Storage>();
            List<WorkerInfo> workerInfoTable = new List<WorkerInfo>();
            List<Workers> workersTable = new List<Workers>();
            try
            {
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Colors]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    colorsTable.Add(new Colors()
                    {
                        id = Convert.ToInt32(reader["Id"]),
                        name = reader["Name"].ToString(),
                        specification = reader["Description"].ToString()
                    });
                }
                ColorsTable.ItemsSource = colorsTable;
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Orders]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ordersTable.Add(new Orders()
                    {
                        Id = reader["Id"].ToString(),
                        Product_id = reader["Product_id"].ToString(),
                        Amount = reader["Amount"].ToString(),
                        Cost_Order = reader["Cost_order"].ToString(),
                        Client_id = reader["Client_id"].ToString()
                    });
                }
                OrdersTable.Items.Add(ordersTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Client]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    clientTable.Add(new Client()
                    {
                        Id = reader["Id"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        SecondName = reader["SecondName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        E_mail = reader["E_mail"].ToString(),
                        Phone = reader["Phone"].ToString()

                    });
                }
                ClientTable.Items.Add(clientTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Component]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    componentTable.Add(new Component()
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Amount = reader["Amount"].ToString(),
                        Color_id = reader["Color_id"].ToString(),
                        Type = reader["Type"].ToString(),
                        Storage_id = reader["Storage_id"].ToString()
                    });
                }
                ComponentTable.Items.Add(componentTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Departament]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    departamentTable.Add(new Departament()
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Storage_id = reader["Storage_id"].ToString()
                    });
                }
                DepartamentTable.Items.Add(departamentTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [DirectorInfo]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    directorInfoTable.Add(new DirectorInfo()
                    {
                        Id = reader["Id"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        SecondName = reader["SecondName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        E_mail = reader["E_mail"].ToString(),
                        DateBorn = reader["DateBorn"].ToString()
                    });
                }
                DirectorInfoTable.Items.Add(directorInfoTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Directors]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    directorsTable.Add(new Directors()
                    {
                        Id = reader["Id"].ToString(),
                        DirectorInfo_id = reader["DirectorInfo_id"].ToString(),
                        Departament_id = reader["Departament_id"].ToString()
                    }); ;
                }
                DirectorTable.Items.Add(directorsTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Product]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    productTable.Add(new Product()
                    {
                        Id = reader["Id"].ToString(),
                        Departament_id = reader["Departament_id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Color_id = reader["Color_id"].ToString(),
                        Price = reader["Price"].ToString()
                    });
                }
                ProductTable.Items.Add(productTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Storage]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    storageTable.Add(new Storage()
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),

                    });
                }
                StorageTable.Items.Add(storageTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [WorkerInfo]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    workerInfoTable.Add(new WorkerInfo()
                    {
                        Id = reader["Id"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        SecondName = reader["SecondName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        E_mail = reader["E_mail"].ToString(),
                        DateBorn = reader["DateBorn"].ToString()
                    });
                }
                WorkerInfoTable.Items.Add(workerInfoTable);
                reader.Close();
                //
                sqlCommand = new SqlCommand("SELECT * FROM [Workers]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    workersTable.Add(new Workers()
                    {
                        Id = reader["Id"].ToString(),
                        WorkerInfo_id = reader["WorkerInfo_id"].ToString(),
                        Departament_id = reader["Departament_id"].ToString(),
                        Position = reader["Position"].ToString()
                    });
                }
                WorkersTable.Items.Add(workersTable);
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SqlConnection != null && SqlConnection.State != System.Data.ConnectionState.Closed)
            {
                SqlConnection.Close();
            }
        }
    }
}
