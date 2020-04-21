using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;

namespace CourseWork
{
    public partial class MainWindow : Window
    {
        readonly List<string> operation = new List<string>() { "ADD", "SELECT", "UPDATE", "DELETE" };

        SqlConnection SqlConnection;
        string globalID = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseSingle_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string connection = @"Data Source=(localdb)\MSSQLLocalDB;
                                    Initial Catalog=FornitureManufacture;
                                    Integrated Security=True;
                                    Connect Timeout=30;Encrypt=False;
                                    TrustServerCertificate=False;
                                    ApplicationIntent=ReadWrite;
                                    MultiSubnetFailover=False";

                SqlConnection = new SqlConnection(connection);
                await DataInAllComboBox();
                await FillAllTable();
                await FillAllOperationComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task FillAllOperationComboBox()
        {
            CB_operationColors.ItemsSource = operation;
            CB_operationClient.ItemsSource = operation;
            CB_operationComponent.ItemsSource = operation;
            CB_operationDirector.ItemsSource = operation;
            CB_operationDirectorInfo.ItemsSource = operation;
            CB_operationOrder.ItemsSource = operation;
            CB_operationProduct.ItemsSource = operation;
            CB_operationStorage.ItemsSource = operation;
            CB_operationWorker.ItemsSource = operation;
            CB_operationWorkerInfo.ItemsSource = operation;
            CB_operationDepartament.ItemsSource = operation;
        }

        //ВСЕ ШО ЗВ'ЯЗАНО З COLORS_TABLE #########################################
        private async void ColorsDone(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            try
            {
                if (CB_operationColors.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Colors] (Name, Description)VALUES(@Name, @Description)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("Name", TB_nameColors.Text);
                    sqlCommand.Parameters.AddWithValue("Description", TB_specificationColors.Text);
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshColorsTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationColors.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Colors> colorsTable = new List<Colors>();
                    sqlCommand = new SqlCommand("SELECT * FROM [Colors] WHERE id = " + globalID, SqlConnection);
                    reader = await sqlCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        colorsTable.Add(new Colors()
                        {
                            id = Convert.ToInt32(globalID),
                            name = reader["Name"].ToString(),
                            specification = reader["Description"].ToString()
                        });
                    }
                    ColorsTable.ItemsSource = colorsTable;
                    reader.Close();

                    SqlConnection.Close();
                }
                else if (CB_operationColors.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Colors] SET [Name]=@Name, [Description]=@Description WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("Name", TB_nameColors.Text);
                    sqlCommand.Parameters.AddWithValue("Description", TB_specificationColors.Text);
                    sqlCommand.Parameters.AddWithValue("Id", globalID);

                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshColorsTable();
                    await DataInAllComboBox();

                }
                else if (CB_operationColors.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Colors] WHERE id = " + globalID, SqlConnection);

                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshColorsTable();
                    await DataInAllComboBox();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private async void CB_chooseColors_DropDownClosed(object sender, EventArgs e)
        {

            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_chooseColors.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT Name,Description FROM [Colors] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_nameColors.Text = reader[0].ToString();
                    TB_specificationColors.Text = reader[1].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }
        private async Task RefreshColorsTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            List<Colors> colorsTable = new List<Colors>();
            colorsTable.Clear();
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
            SqlConnection.Close();
        }
        //КІНЕЦЬ#################################################################

        //ВСЕ ШО ЗВ'ЯЗАНО З ORDERS_TABLE #########################################
        private async void OrderDone_BNT(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationOrder.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Orders] (Product_id, Amount,Cost_Order, Client_id)" +
                                                    "VALUES(@Product_id, @Amount, @Cost_Order, @Client_id)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("Product_id", Convert.ToInt32(GetIndexFromCombpBox(CB_productOrder.SelectedItem.ToString())));
                    sqlCommand.Parameters.AddWithValue("Amount", TB_amountProductOrder.Text);
                    sqlCommand.Parameters.AddWithValue("Cost_Order", CalculateOrder(CB_productOrder.SelectedItem.ToString(), TB_amountProductOrder.Text));
                    sqlCommand.Parameters.AddWithValue("Client_id", GetIndexFromCombpBox(CB_clientProductOrder.SelectedItem.ToString()));

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshOrderTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationOrder.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Orders> ordersTable = new List<Orders>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [Orders] WHERE id = " + globalID, SqlConnection);
                    reader = await sqlCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        ordersTable.Add(new Orders()
                        {
                            Id = reader["Id"].ToString(),
                            Product_id = reader["Product_id"].ToString(),
                            Amount = reader["Amount"].ToString(),
                            Client_id = reader["Client_id"].ToString(),
                            Cost_Order = reader["Cost_Order"].ToString()
                        });
                    }
                    OrdersTable.ItemsSource = ordersTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationOrder.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Orders] " +
                                                "SET [Product_id]=@Product_id, [Amount]=@Amount,[Cost_Order] = @Cost_Order, [Client_id]=@Client_id " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("Product_id", GetIndexFromCombpBox(CB_productOrder.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Amount", TB_amountProductOrder.Text);
                    sqlCommand.Parameters.AddWithValue("Cost_Order", CalculateOrder(CB_productOrder.SelectedItem.ToString(), TB_amountProductOrder.Text).ToString());
                    sqlCommand.Parameters.AddWithValue("Client_id", GetIndexFromCombpBox(CB_clientProductOrder.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Id", globalID);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshOrderTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationOrder.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Orders] WHERE id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshOrderTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private double CalculateOrder(string product, string amount_)
        {

            SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            int idProduct = Convert.ToInt32(GetIndexFromCombpBox(product));
            double price = 0;
            int amount = Convert.ToInt32(amount_);

            sqlCommand = new SqlCommand("SELECT Price FROM [Product] WHERE Id =" + idProduct.ToString(), SqlConnection);
            reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                price = Convert.ToDouble(reader[0].ToString());
            }
            reader.Close();
            SqlConnection.Close();
            return price * amount;
        }

        private async void DoneSelectCostrOrder_BTN_Click(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();

            SqlDataReader reader;
            SqlCommand sqlCommand;
            List<Orders> ordersTable = new List<Orders>();

            ordersTable.Clear();
            sqlCommand = new SqlCommand("SELECT * FROM [Orders] WHERE Cost_Order >= @fromCost AND Cost_Order <= @toCost", SqlConnection);
            sqlCommand.Parameters.AddWithValue("fromCost", TB_costFromOrders.Text);
            sqlCommand.Parameters.AddWithValue("toCost", TB_costToOrder.Text);
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
            OrdersTable.ItemsSource = ordersTable;
            reader.Close();

            SqlConnection.Close();
        }
        private async Task RefreshOrderTable()
        {
            await SqlConnection.OpenAsync();

            SqlDataReader reader;
            SqlCommand sqlCommand;
            List<Orders> ordersTable = new List<Orders>();

            ordersTable.Clear();
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
            OrdersTable.ItemsSource = ordersTable;
            reader.Close();

            SqlConnection.Close();
        }

        private async void CB_personOrder_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_chooseOrderID.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT Product_id, Amount, Client_id FROM [Orders] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_amountProductOrder.Text = reader["Amount"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }

        private async void CB_clientOrderByAVG_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_clientOrderByAVG.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT AVG(Cost_Order) FROM [Orders] WHERE Client_id = " + globalID, SqlConnection);
                AVG_valueByOrderClient.Content = await sqlCommand.ExecuteScalarAsync();

                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CB_productForCountOrder_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_productForCountOrder.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT COUNT(Product_id) FROM [Orders] WHERE Product_id = " + globalID, SqlConnection);
                InfoCountOrder.Content = "Count = " + await sqlCommand.ExecuteScalarAsync();

                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //КІНЕЦЬ#####################################################################

        //ВСЕ ШО ЗВ'ЯЗАНО З CLIENT_TABLE##############################################
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationClient.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Client] (FirstName,SecondName,LastName,Phone,E_mail)" +
                                                    "VALUES(@FirstName,@SecondName,@LastName,@Phone,@E_mail)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("FirstName", TB_nameClient.Text);
                    sqlCommand.Parameters.AddWithValue("SecondName", TB_clientSecondName.Text);
                    sqlCommand.Parameters.AddWithValue("LastName", TB_clientLastName.Text);
                    sqlCommand.Parameters.AddWithValue("Phone", TB_phoneClient.Text);
                    sqlCommand.Parameters.AddWithValue("E_mail", TB_EmailClient.Text);

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshClientTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationClient.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Client> clientTable = new List<Client>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [Client] WHERE id = " + globalID, SqlConnection);
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
                    ClientTable.ItemsSource = clientTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationClient.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Client] " +
                                                "SET [FirstName]=@FirstName, [SecondName]=@SecondName,[LastName] = @LastName, [Phone]=@Phone, [E_mail]=@E_mail " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("FirstName", TB_nameClient.Text);
                    sqlCommand.Parameters.AddWithValue("SecondName", TB_clientSecondName.Text);
                    sqlCommand.Parameters.AddWithValue("LastName", TB_clientLastName.Text);
                    sqlCommand.Parameters.AddWithValue("Phone", TB_phoneClient.Text);
                    sqlCommand.Parameters.AddWithValue("E_mail", TB_EmailClient.Text);
                    sqlCommand.Parameters.AddWithValue("Id", globalID);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshClientTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationClient.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Client] WHERE id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshClientTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshClientTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Client> clientTable = new List<Client>();

            clientTable.Clear();
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
            ClientTable.ItemsSource = clientTable;
            reader.Close();

            SqlConnection.Close();
        }

        private async void CB_personClient_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_personClient.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT * FROM [Client] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_nameClient.Text = reader["FirstName"].ToString();
                    TB_clientSecondName.Text = reader["SecondName"].ToString();
                    TB_clientLastName.Text = reader["LastName"].ToString();
                    TB_phoneClient.Text = reader["Phone"].ToString();
                    TB_EmailClient.Text = reader["E_mail"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }
        //КІНЕЦЬ######################################################################

        //ВСЕ ШО ПОВЄЯЗАНО З DEPARTAMENT_TABLE########################################
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationDepartament.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Departament] (Name, Storage_id)" +
                                                    "VALUES(@Name, @Storage_id)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("Name", TB_nameDepartament.Text);
                    sqlCommand.Parameters.AddWithValue("Storage_id", GetIndexFromCombpBox(CB_chooseStorageDepartament.SelectedItem.ToString()));

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshDepartamentTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationDepartament.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Departament> departamentTable = new List<Departament>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [Departament] WHERE id = " + globalID, SqlConnection);
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
                    DepartamentTable.ItemsSource = departamentTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationDepartament.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Departament] " +
                                                "SET [Name]=@Name, [Storage_id]=@Storage_id " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("Name", TB_nameDepartament.Text);
                    sqlCommand.Parameters.AddWithValue("Storage_id", GetIndexFromCombpBox(CB_chooseStorageDepartament.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Id", globalID);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshDepartamentTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationDepartament.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Departament] WHERE id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshDepartamentTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshDepartamentTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Departament> departamentTable = new List<Departament>();

            departamentTable.Clear();
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
            DepartamentTable.ItemsSource = departamentTable;
            reader.Close();

            SqlConnection.Close();
        }


        private async void CB_chooseDepartament_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_chooseDepartament.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT * FROM [Departament] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_nameDepartament.Text = reader["Name"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }

        //КІНЕЦЬ######################################################################

        //ВСЕ ШО ПОВ'ЯЗАНО З WORKERINFO_TABLE#########################################
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationWorkerInfo.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [WorkerInfo] (FirstName, SecondName, LastName, DateBorn, Phone, E_mail)" +
                                                    "VALUES(@FirstName, @SecondName, @LastName, @DateBorn, @Phone, @E_mail)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("FirstName", TB_nameWorkserInfo.Text);
                    sqlCommand.Parameters.AddWithValue("SecondName", TB_secondNameWorkerInfo.Text);
                    sqlCommand.Parameters.AddWithValue("LastName", TB_lastNameWorkerInfo.Text);
                    sqlCommand.Parameters.AddWithValue("DateBorn", TB_dateBornWorkerInfo.Text);
                    sqlCommand.Parameters.AddWithValue("Phone", TB_phoneWorkserInfo.Text);
                    sqlCommand.Parameters.AddWithValue("E_mail", TB_emaolWorkerInfo.Text);

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshWorkerInfoTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationWorkerInfo.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<WorkerInfo> workerInfoTable = new List<WorkerInfo>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [WorkerInfo] WHERE id = " + globalID, SqlConnection);
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
                    WorkerInfoTable.ItemsSource = workerInfoTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationWorkerInfo.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [WorkerInfo] " +
                                                "SET [FirstName]=@FirstName, [SecondName]=@SecondName,[LastName] = @LastName, [Phone]=@Phone, [E_mail]=@E_mail, [DateBorn]=@DateBorn " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("FirstName", TB_nameWorkserInfo.Text);
                    sqlCommand.Parameters.AddWithValue("SecondName", TB_secondNameWorkerInfo.Text);
                    sqlCommand.Parameters.AddWithValue("LastName", TB_lastNameWorkerInfo.Text);
                    sqlCommand.Parameters.AddWithValue("DateBorn", TB_dateBornWorkerInfo.Text);
                    sqlCommand.Parameters.AddWithValue("Phone", TB_phoneWorkserInfo.Text);
                    sqlCommand.Parameters.AddWithValue("E_mail", TB_emaolWorkerInfo.Text);

                    sqlCommand.Parameters.AddWithValue("Id", globalID);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshWorkerInfoTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationWorkerInfo.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [WorkerInfo] WHERE id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshWorkerInfoTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshWorkerInfoTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<WorkerInfo> workerInfoTable = new List<WorkerInfo>();

            workerInfoTable.Clear();
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
            WorkerInfoTable.ItemsSource = workerInfoTable;
            reader.Close();

            SqlConnection.Close();
        }
        private async void CB_personWorkerInfo_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_personWorkerInfo.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT * FROM [WorkerInfo] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_nameWorkserInfo.Text = reader["FirstName"].ToString();
                    TB_secondNameWorkerInfo.Text = reader["SecondName"].ToString();
                    TB_lastNameWorkerInfo.Text = reader["LastName"].ToString();
                    TB_phoneWorkserInfo.Text = reader["Phone"].ToString();
                    TB_dateBornWorkerInfo.Text = reader["DateBorn"].ToString();
                    TB_emaolWorkerInfo.Text = reader["E_mail"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }
        //КІНЕЦЬ######################################################################

        //ВСЕ ШО З ТАБЛИЦЕЮ PRODUCT_TABLE#############################################

        private async void ProductDone_BTN(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationProduct.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Product] (Name, Description, Color_id, Departament_id, Price)" +
                                                    "VALUES(@Name, @Description, @Color_id, @Departament_id, @Price)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("Name", TB_nemeProduct.Text);
                    sqlCommand.Parameters.AddWithValue("Description", TB_descriptionProduct.Text);
                    sqlCommand.Parameters.AddWithValue("Color_id", GetIndexFromCombpBox(CB_colorProduct.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Departament_id", GetIndexFromCombpBox(
                                                                         CB_departamentProduct.SelectedItem.ToString())
                                                                         );
                    sqlCommand.Parameters.AddWithValue("Price", TB_priceProduct.Text);

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshProductTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationProduct.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Product> productTable = new List<Product>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [Product] WHERE id = " + globalID, SqlConnection);
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
                    ProductTable.ItemsSource = productTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationProduct.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Product] " +
                                                "SET [Name] = @Name, [Description] = @Description, [Color_id] = @Color_id, [Departament_id] = @Departament_id, [Price] = @Price " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("Name", TB_nemeProduct.Text);
                    sqlCommand.Parameters.AddWithValue("Description", TB_descriptionProduct.Text);
                    sqlCommand.Parameters.AddWithValue("Color_id", GetIndexFromCombpBox(CB_colorProduct.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Departament_id", GetIndexFromCombpBox(
                                                                         CB_departamentProduct.SelectedItem.ToString())
                                                                         );
                    sqlCommand.Parameters.AddWithValue("Price", TB_priceProduct.Text);

                    sqlCommand.Parameters.AddWithValue("Id", globalID);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshProductTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationProduct.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Product] WHERE id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshProductTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshProductTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Product> productTable = new List<Product>();

            productTable.Clear();
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
            ProductTable.ItemsSource = productTable;
            reader.Close();

            SqlConnection.Close();
        }
        private async void CB_selectProduct_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_selectProduct.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT * FROM [Product] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_nemeProduct.Text = reader["Name"].ToString();
                    TB_descriptionProduct.Text = reader["Description"].ToString();
                    TB_priceProduct.Text = reader["Price"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }

        private async void SelectProductOR_BTN(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Product> productTable = new List<Product>();

            productTable.Clear();
            sqlCommand = new SqlCommand("SELECT * FROM [Product] WHERE Name = @NameFirst OR Name = @NameSecond", SqlConnection);
            sqlCommand.Parameters.AddWithValue("NameFirst", TB_firstProduct.Text);
            sqlCommand.Parameters.AddWithValue("NameSecond", TB_secondProduct.Text);
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
            ProductTable.ItemsSource = productTable;
            reader.Close();

            SqlConnection.Close();
        }

        private async void ProductLikeName_BTN_Click(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Product> productTable = new List<Product>();

            productTable.Clear();
            sqlCommand = new SqlCommand("SELECT * FROM [Product] WHERE Name LIKE @Name", SqlConnection);
            sqlCommand.Parameters.AddWithValue("Name", TB_nameProductLike.Text + "%");
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
            ProductTable.ItemsSource = productTable;
            reader.Close();

            SqlConnection.Close();
        }

        private async void doneBetweenPriceProduct_BTN_Click(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Product> productTable = new List<Product>();

            productTable.Clear();
            sqlCommand = new SqlCommand("SELECT * FROM [Product] WHERE Price BETWEEN @fromPrice AND @toPrice", SqlConnection);
            sqlCommand.Parameters.AddWithValue("fromPrice", TB_fromPriceProduct.Text);
            sqlCommand.Parameters.AddWithValue("toPrice", TB_toPriceProduct.Text);
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
            ProductTable.ItemsSource = productTable;
            reader.Close();

            SqlConnection.Close();
        }

        private async void MIN_MAX_priduct_BTN_Click(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlCommand sqlCommand;

            sqlCommand = new SqlCommand("SELECT MAX(Price) FROM [Product]", SqlConnection);
            Max_priceProduct.Content = await sqlCommand.ExecuteScalarAsync();

            sqlCommand = new SqlCommand("SELECT Min(Price) FROM [Product]", SqlConnection);
            Min_priceProduct.Content = await sqlCommand.ExecuteScalarAsync();

            SqlConnection.Close();
        }

        //КІНЕЦЬ######################################################################

        //ВСЕ ШО З ТАБЛИЦЕЮ WORKERS_TABLE#############################################

        private async void WorkersClick_BTN(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationWorker.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Workers] (WorkerInfo_id, Departament_id, Position)" +
                                                    "VALUES(@WorkerInfo_id, @Departament_id, @Position)", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("WorkerInfo_id", GetIndexFromCombpBox(CB_workerInfoWorker.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Departament_id", GetIndexFromCombpBox(CB_departamentWorker.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Position", TB_positionWorker.Text);


                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshWorkersTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationWorker.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Workers> workerTable = new List<Workers>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [Workers] WHERE id = " + globalID, SqlConnection);
                    reader = await sqlCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        workerTable.Add(new Workers()
                        {
                            Id = reader["Id"].ToString(),
                            WorkerInfo_id = reader["WorkerInfo_id"].ToString(),
                            Departament_id = reader["Departament_id"].ToString(),
                            Position = reader["Position"].ToString()
                        });
                    }
                    WorkersTable.ItemsSource = workerTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationWorker.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Workers] " +
                                                "SET [WorkerInfo_id] = @WorkerInfo_id, [Departament_id] = @Departament_id, [Position] = @Position " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("WorkerInfo_id", GetIndexFromCombpBox(CB_workerInfoWorker.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Departament_id", GetIndexFromCombpBox(CB_departamentWorker.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Position", TB_positionWorker.Text);
                    sqlCommand.Parameters.AddWithValue("Id", globalID);

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshWorkersTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationWorker.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Workers] WHERE id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshWorkersTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshWorkersTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Workers> workersTable = new List<Workers>();
            workersTable.Clear();
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
            WorkersTable.ItemsSource = workersTable;
            reader.Close();

            SqlConnection.Close();
        }
        private async void CB_chooseWorker_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_chooseWorker.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT * FROM [Workers] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_positionWorker.Text = reader["Position"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }
        //КІНІЕЦЬ#####################################################################

        //DIRECTORS###################################################################
        private async void DirectorsDone_BTN(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationDirector.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Directors] (DirectorInfo_id, Departament_id)" +
                                                    "VALUES(@DirectorInfo_id, @Departament_id)", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("DirectorInfo_id", GetIndexFromCombpBox(CB_secondNameDirector.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Departament_id", GetIndexFromCombpBox(CB_departamentDirector.SelectedItem.ToString()));


                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshDirectorsTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationDirector.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Directors> directorsTable = new List<Directors>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [Directors] WHERE id = " + globalID, SqlConnection);
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
                    DirectorTable.ItemsSource = directorsTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationDirector.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Directors] " +
                                                "SET [DirectorInfo_id] = @DirectorInfo_id, [Departament_id] = @Departament_id " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("DirectorInfo_id", GetIndexFromCombpBox(CB_secondNameDirector.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Departament_id", GetIndexFromCombpBox(CB_departamentDirector.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Id", globalID);

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshDirectorsTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationDirector.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Directors] WHERE id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshDirectorsTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshDirectorsTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Directors> directorsTable = new List<Directors>();

            directorsTable.Clear();
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
            DirectorTable.ItemsSource = directorsTable;
            reader.Close();

            SqlConnection.Close();
        }

        private void CB_chooseDirector_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                globalID = GetIndexFromCombpBox(CB_chooseDirector.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //THE END#####################################################################

        //DIRECTORINFO_TABLE##########################################################

        private async void DirectorInfoDone_BNT(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationDirectorInfo.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [DirectorInfo] (FirstName, SecondName, LastName, DateBorn, Phone, E_mail)" +
                                                    "VALUES(@FirstName, @SecondName, @LastName, @DateBorn, @Phone, @E_mail)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("FirstName", TB_nameDirectorInfo.Text);
                    sqlCommand.Parameters.AddWithValue("SecondName", TB_directorSecondName.Text);
                    sqlCommand.Parameters.AddWithValue("LastName", TB_DirectorLastName.Text);
                    sqlCommand.Parameters.AddWithValue("DateBorn", TB_dateBornDirectorInfo.Text);
                    sqlCommand.Parameters.AddWithValue("Phone", TB_phoneDorectorInfo.Text);
                    sqlCommand.Parameters.AddWithValue("E_mail", TB_EmailDirectorInfo.Text);

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshDirectorInfoTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationDirectorInfo.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<DirectorInfo> directorInfoTable = new List<DirectorInfo>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [DirectorInfo] WHERE id = " + globalID, SqlConnection);
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
                    DirectorInfoTable.ItemsSource = directorInfoTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationDirectorInfo.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [DirectorInfo] " +
                                                "SET [FirstName]=@FirstName, [SecondName]=@SecondName,[LastName] = @LastName, [Phone]=@Phone, [E_mail]=@E_mail, [DateBorn]=@DateBorn " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("FirstName", TB_nameDirectorInfo.Text);
                    sqlCommand.Parameters.AddWithValue("SecondName", TB_directorSecondName.Text);
                    sqlCommand.Parameters.AddWithValue("LastName", TB_DirectorLastName.Text);
                    sqlCommand.Parameters.AddWithValue("DateBorn", TB_dateBornDirectorInfo.Text);
                    sqlCommand.Parameters.AddWithValue("Phone", TB_phoneDorectorInfo.Text);
                    sqlCommand.Parameters.AddWithValue("E_mail", TB_EmailDirectorInfo.Text);

                    sqlCommand.Parameters.AddWithValue("Id", globalID);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshDirectorInfoTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationDirectorInfo.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [DirectorInfo] WHERE Id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshDirectorInfoTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshDirectorInfoTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<DirectorInfo> directorInfoTable = new List<DirectorInfo>();

            directorInfoTable.Clear();
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
            DirectorInfoTable.ItemsSource = directorInfoTable;
            reader.Close();

            SqlConnection.Close();
        }

        private async void CB_personDirectorInfo_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_personDirectorInfo.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT * FROM [DirectorInfo] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_nameDirectorInfo.Text = reader["FirstName"].ToString();
                    TB_directorSecondName.Text = reader["SecondName"].ToString();
                    TB_DirectorLastName.Text = reader["LastName"].ToString();
                    TB_phoneDorectorInfo.Text = reader["Phone"].ToString();
                    TB_dateBornDirectorInfo.Text = reader["DateBorn"].ToString();
                    TB_EmailDirectorInfo.Text = reader["E_mail"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }
        //THE END#####################################################################

        //STORAGE_TABLE###############################################################
        private async void StorageDone_BTN(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationStorage.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Storage] (Name)" +
                                                    "VALUES(@Name)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("Name", TB_nameStorage.Text);

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshStorageTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationStorage.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Storage> storageTable = new List<Storage>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [Storage] WHERE id = " + globalID, SqlConnection);
                    reader = await sqlCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        storageTable.Add(new Storage()
                        {
                            Id = reader["Id"].ToString(),
                            Name = reader["Name"].ToString(),

                        });
                    }
                    StorageTable.ItemsSource = storageTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationStorage.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Storage] " +
                                                "SET [Name]=@Name " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("Name", TB_nameStorage.Text);

                    sqlCommand.Parameters.AddWithValue("Id", globalID);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshStorageTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationStorage.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Storage] WHERE Id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshStorageTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshStorageTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Storage> storageTable = new List<Storage>();

            storageTable.Clear();
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
            StorageTable.ItemsSource = storageTable;
            reader.Close();

            SqlConnection.Close();
        }
        private async void CB_Storage_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_Storage.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT * FROM [Storage] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_nameStorage.Text = reader["Name"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }


        //THE END#####################################################################

        //COMPONENT_TABLE#############################################################

        private async void ComponentDone_BTN(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_operationComponent.SelectedItem.ToString() == "ADD")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("INSERT INTO [Component] (Name,Amount, Color_id, Type, Storage_id )" +
                                                    "VALUES(@Name,@Amount, @Color_id, @Type, @Storage_id)", SqlConnection);
                    sqlCommand.Parameters.AddWithValue("Name", TB_nameComponent.Text);
                    sqlCommand.Parameters.AddWithValue("Amount", TB_amountComponent.Text);
                    sqlCommand.Parameters.AddWithValue("Color_id", GetIndexFromCombpBox(CB_colorComponent.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Type", TB_typeComponent.Text);
                    sqlCommand.Parameters.AddWithValue("Storage_id", GetIndexFromCombpBox(CB_storageComponent.SelectedItem.ToString()));

                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();

                    await RefreshComponentTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationComponent.SelectedItem.ToString() == "SELECT")
                {
                    SqlDataReader reader;
                    SqlCommand sqlCommand;
                    List<Component> componentTable = new List<Component>();
                    await SqlConnection.OpenAsync();
                    sqlCommand = new SqlCommand("SELECT * FROM [Component] WHERE id = " + globalID, SqlConnection);
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
                    ComponentTable.ItemsSource = componentTable;
                    reader.Close();
                    SqlConnection.Close();
                }
                else if (CB_operationComponent.SelectedItem.ToString() == "UPDATE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("UPDATE [Component] " +
                                                "SET [Name]=@Name, [Amount]=@Amount, [Color_id]=@Color_id, [Type]=@Type, [Storage_id]= @Storage_id " +
                                                "WHERE [Id]=@Id", SqlConnection);

                    sqlCommand.Parameters.AddWithValue("Name", TB_nameComponent.Text);
                    sqlCommand.Parameters.AddWithValue("Amount", TB_amountComponent.Text);
                    sqlCommand.Parameters.AddWithValue("Color_id", GetIndexFromCombpBox(CB_colorComponent.SelectedItem.ToString()));
                    sqlCommand.Parameters.AddWithValue("Type", TB_typeComponent.Text);
                    sqlCommand.Parameters.AddWithValue("Storage_id", GetIndexFromCombpBox(CB_storageComponent.SelectedItem.ToString()));

                    sqlCommand.Parameters.AddWithValue("Id", globalID);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                    SqlConnection.Close();
                    await RefreshComponentTable();
                    await DataInAllComboBox();
                }
                else if (CB_operationComponent.SelectedItem.ToString() == "DELETE")
                {
                    SqlCommand sqlCommand;
                    sqlCommand = new SqlCommand("DELETE FROM [Component] WHERE Id = " + globalID, SqlConnection);
                    await SqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();

                    SqlConnection.Close();

                    await RefreshComponentTable();
                    await DataInAllComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task RefreshComponentTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Component> componentTable = new List<Component>();

            componentTable.Clear();
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
            ComponentTable.ItemsSource = componentTable;
            reader.Close();


            SqlConnection.Close();
        }

        private async void CB_itemComponent_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                globalID = GetIndexFromCombpBox(CB_itemComponent.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT * FROM [Component] WHERE Id = " + globalID, SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    TB_nameComponent.Text = reader["Name"].ToString();
                    TB_amountComponent.Text = reader["Amount"].ToString();
                    TB_typeComponent.Text = reader["Type"].ToString();
                }
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SqlConnection.Close();
        }

        private async void DoneDistinctTypeComponent_BTN_Click(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;

            List<Component> componentTable = new List<Component>();

            componentTable.Clear();
            sqlCommand = new SqlCommand("SELECT DISTINCT Type, Id, Name, Amount, Color_id, Storage_id FROM [Component]", SqlConnection);
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
            ComponentTable.ItemsSource = componentTable;
            reader.Close();


            SqlConnection.Close();
        }

        //THE END#####################################################################
        private async Task DataInAllComboBox()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader = null;
            SqlCommand sqlCommand;
            try
            {
                sqlCommand = new SqlCommand("SELECT Id FROM [Orders]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_chooseOrderID.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_chooseOrderID.Items.Add(reader[0].ToString());
                }
                reader.Close();

                sqlCommand = new SqlCommand("SELECT id,SecondName FROM [Client]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_personClient.Items.Clear();
                CB_clientProductOrder.Items.Clear();
                CB_clientOrderByAVG.Items.Clear();
                CB_client.Items.Clear();
                CB_chooseClientOrderForm.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_personClient.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_clientProductOrder.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_clientOrderByAVG.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_client.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_chooseClientOrderForm.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();

                sqlCommand = new SqlCommand("SELECT id,Name FROM [Departament]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_departamentProduct.Items.Clear();
                CB_chooseDepartament.Items.Clear();
                CB_departamentWorker.Items.Clear();
                CB_departamentDirector.Items.Clear();
                CB_DepartamentOnDirector.Items.Clear();
                CB_Departament.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_chooseDepartament.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_departamentProduct.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_departamentWorker.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_departamentDirector.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_Departament.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_DepartamentOnDirector.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();

                sqlCommand = new SqlCommand("SELECT id,SecondName FROM [WorkerInfo]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_personWorkerInfo.Items.Clear();
                CB_workerInfoWorker.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_personWorkerInfo.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_workerInfoWorker.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();

                sqlCommand = new SqlCommand("SELECT id,Name FROM [Product]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_selectProduct.Items.Clear();
                CB_productOrder.Items.Clear();
                CB_productForCountOrder.Items.Clear();
                CB_chooseProductForm.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_selectProduct.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_productOrder.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_productForCountOrder.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_chooseProductForm.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
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
                CB_secondNameDirector.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_personDirectorInfo.Items.Add(reader[0].ToString() + " " + reader[1].ToString());

                    CB_secondNameDirector.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();

                sqlCommand = new SqlCommand("SELECT id,Name FROM [Storage]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                CB_Storage.Items.Clear();
                CB_chooseStorageDepartament.Items.Clear();
                CB_storageComponent.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_Storage.Items.Add(reader[0].ToString() + " " + reader[1].ToString());

                    CB_chooseStorageDepartament.Items.Add(reader[0].ToString() + " " + reader[1].ToString());

                    CB_storageComponent.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
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
                CB_colorProduct.Items.Clear();
                CB_colorComponent.Items.Clear();
                while (await reader.ReadAsync())
                {
                    CB_chooseColors.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_colorProduct.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                    CB_colorComponent.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                SqlConnection.Close();
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

        private string GetIndexFromCombpBox(string item)
        {
            string id = null;
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i] != ' ')
                {
                    id += item[i];
                }
                else
                {
                    break;
                }
            }
            return id;
        }


        private async Task FillAllTable()
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
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
                colorsTable.Clear();
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
                ordersTable.Clear();
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
                OrdersTable.ItemsSource = ordersTable;
                reader.Close();
                //
                clientTable.Clear();
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
                ClientTable.ItemsSource = clientTable;
                reader.Close();
                //
                componentTable.Clear();
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
                ComponentTable.ItemsSource = componentTable;
                reader.Close();
                //
                departamentTable.Clear();
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
                DepartamentTable.ItemsSource = departamentTable;
                reader.Close();
                //
                directorInfoTable.Clear();
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
                DirectorInfoTable.ItemsSource = directorInfoTable;
                reader.Close();
                //
                directorsTable.Clear();
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
                DirectorTable.ItemsSource = directorsTable;
                reader.Close();
                //
                productTable.Clear();
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
                ProductTable.ItemsSource = productTable;
                reader.Close();
                //
                storageTable.Clear();
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
                StorageTable.ItemsSource = storageTable;
                reader.Close();
                //
                workerInfoTable.Clear();
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
                WorkerInfoTable.ItemsSource = workerInfoTable;
                reader.Close();
                //
                workersTable.Clear();
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
                WorkersTable.ItemsSource = workersTable;
                reader.Close();
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                reader = null;
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
        //ORDER_CLIENT#################################################################
        private async void CB_client_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {

                List<OrderByClient> temp = new List<OrderByClient>();
                globalID = GetIndexFromCombpBox(CB_client.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT Orders.Id, Orders.Product_id, Orders.Amount, Orders.Cost_order, " +
                    " Orders.Client_id, Client.SecondName, Client.FirstName, Client.E_mail FROM [Orders]" +
                    "INNER JOIN [Client] ON Orders.Client_id = @id AND Client.Id = @id", SqlConnection);
                sqlCommand.Parameters.AddWithValue("id", globalID);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    temp.Add(new OrderByClient
                    {
                        OrderId = reader[0].ToString(),
                        ProductId = reader[1].ToString(),
                        Amount = reader[2].ToString(),
                        CostOrder = reader[3].ToString(),
                        ClientId = reader[4].ToString(),
                        SecondName = reader[5].ToString(),
                        FirstName = reader[6].ToString(),
                        E_mail = reader[7].ToString()
                    });
                }
                OrderByClient_Table.ItemsSource = temp;
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AllOrderByClient_Click(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {
                List<OrderByClient> temp = new List<OrderByClient>();
                sqlCommand = new SqlCommand("SELECT Orders.Id, Orders.Product_id, Orders.Amount, Orders.Cost_order, " +
                    " Orders.Client_id, Client.SecondName, Client.FirstName, Client.E_mail FROM [Orders]" +
                    "INNER JOIN [Client] ON Client.Id = Orders.Client_id ", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    temp.Add(new OrderByClient
                    {
                        OrderId = reader[0].ToString(),
                        ProductId = reader[1].ToString(),
                        Amount = reader[2].ToString(),
                        CostOrder = reader[3].ToString(),
                        ClientId = reader[4].ToString(),
                        SecondName = reader[5].ToString(),
                        FirstName = reader[6].ToString(),
                        E_mail = reader[7].ToString()
                    });
                }
                OrderByClient_Table.ItemsSource = temp;
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //end##########################################################################

        //DEPARTAMENT_WORKER###########################################################
        private async void CB_Departament_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {

                List<WorkerByDepartament> temp = new List<WorkerByDepartament>();
                globalID = GetIndexFromCombpBox(CB_Departament.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT Departament.Id, Departament.Name, WorkerInfo.SecondName," +
                    "WorkerInfo.FirstName, Workers.Position, Workers.Id FROM [Departament] " +
                    "LEFT JOIN [Workers] ON Workers.Departament_id = @id " +
                    "LEFT JOIN [WorkerInfo] ON Workers.WorkerInfo_id = WorkerInfo.Id " +
                    "WHERE Departament.Id = @id ", SqlConnection);
                sqlCommand.Parameters.AddWithValue("id", globalID);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    temp.Add(new WorkerByDepartament
                    {
                        DepartamentId = reader[0].ToString(),
                        DepartamentName = reader[1].ToString(),
                        WorkerId = reader[5].ToString(),
                        SecondName = reader[2].ToString(),
                        FirstName = reader[3].ToString(),
                        Position = reader[4].ToString()
                    });
                }
                WorkerByDepartament_Table.ItemsSource = temp;
                SqlConnection.Close();
           }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AllWorkerByDepartament_Click(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {

                List<WorkerByDepartament> temp = new List<WorkerByDepartament>();
                sqlCommand = new SqlCommand("SELECT Departament.Id, Departament.Name, WorkerInfo.SecondName," +
                    "WorkerInfo.FirstName, Workers.Position, Workers.Id FROM [Departament] " +
                    "LEFT JOIN [Workers] ON Workers.Departament_id = Departament.Id " +
                    "LEFT JOIN [WorkerInfo] ON Workers.WorkerInfo_id = WorkerInfo.Id ", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    temp.Add(new WorkerByDepartament
                    {
                        DepartamentId = reader[0].ToString(),
                        DepartamentName = reader[1].ToString(),
                        WorkerId = reader[5].ToString(),
                        SecondName = reader[2].ToString(),
                        FirstName = reader[3].ToString(),
                        Position = reader[4].ToString()
                    });
                }
                WorkerByDepartament_Table.ItemsSource = temp;
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //END##########################################################################

        // Director_Departament########################################################
        private async void CB_DepartamentOnDirector_DropDownClosed(object sender, EventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {

                List<DirectorByDepartament> temp = new List<DirectorByDepartament>();
                globalID = GetIndexFromCombpBox(CB_DepartamentOnDirector.SelectedItem.ToString());
                sqlCommand = new SqlCommand("SELECT Departament.Id, Departament.Name, DirectorInfo.SecondName," +
                    "DirectorInfo.FirstName, Directors.Id FROM [Departament] " +
                    "LEFT JOIN [Directors] ON Directors.Departament_id = @id " +
                    "LEFT JOIN [DirectorInfo] ON Directors.DirectorInfo_id = DirectorInfo.Id " +
                    "WHERE Departament.Id = @id ", SqlConnection);
                sqlCommand.Parameters.AddWithValue("id", globalID);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    temp.Add(new DirectorByDepartament
                    {
                        DepartamentId = reader[0].ToString(),
                        DepartamentName = reader[1].ToString(),
                        DirectorId = reader[4].ToString(),
                        SecondName = reader[2].ToString(),
                        FirstName = reader[3].ToString(),
                    });
                }
                DirectorByDepartament_Table.ItemsSource = temp;
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AllDirectorByDepartament_Click(object sender, RoutedEventArgs e)
        {
            await SqlConnection.OpenAsync();
            SqlDataReader reader;
            SqlCommand sqlCommand;
            try
            {

                List<DirectorByDepartament> temp = new List<DirectorByDepartament>();
                sqlCommand = new SqlCommand("SELECT Departament.Id, Departament.Name, DirectorInfo.SecondName," +
                    "DirectorInfo.FirstName, Directors.Id FROM [Departament] " +
                    "RIGHT JOIN [Directors] ON Directors.Departament_id = Departament.Id " +
                    "RIGHT JOIN [DirectorInfo] ON Directors.DirectorInfo_id = DirectorInfo.Id ", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    temp.Add(new DirectorByDepartament
                    {
                        DepartamentId = reader[0].ToString(),
                        DepartamentName = reader[1].ToString(),
                        DirectorId = reader[4].ToString(),
                        SecondName = reader[2].ToString(),
                        FirstName = reader[3].ToString(),
                    });
                }
                DirectorByDepartament_Table.ItemsSource = temp;
                SqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RegistrationClient_BTN_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand("INSERT INTO [Client] (FirstName,SecondName,LastName,Phone,E_mail)" +
                                            "VALUES(@FirstName,@SecondName,@LastName,@Phone,@E_mail)", SqlConnection);
            sqlCommand.Parameters.AddWithValue("FirstName", TB_FirstNameRegistrtion.Text);
            sqlCommand.Parameters.AddWithValue("SecondName", TB_SecondNameRegistrtion.Text);
            sqlCommand.Parameters.AddWithValue("LastName", TB_LastNameRegistrtion.Text);
            sqlCommand.Parameters.AddWithValue("Phone", TB_PhoneRegistrtion.Text);
            sqlCommand.Parameters.AddWithValue("E_mail", TB_EmailRegistrtion.Text);

            await SqlConnection.OpenAsync();
            await sqlCommand.ExecuteNonQueryAsync();
            SqlConnection.Close();

            await DataInAllComboBox();
        }

        private async void AddOrderForm_BTN_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand sqlCommand;
            sqlCommand = new SqlCommand("INSERT INTO [Orders] (Product_id, Amount,Cost_Order, Client_id)" +
                                            "VALUES(@Product_id, @Amount, @Cost_Order, @Client_id)", SqlConnection);
            sqlCommand.Parameters.AddWithValue("Product_id", Convert.ToInt32(GetIndexFromCombpBox(CB_chooseProductForm.SelectedItem.ToString())));
            sqlCommand.Parameters.AddWithValue("Amount", TB_AmountForm.Text);
            sqlCommand.Parameters.AddWithValue("Cost_Order", CalculateOrder(CB_chooseProductForm.SelectedItem.ToString(), TB_AmountForm.Text));
            sqlCommand.Parameters.AddWithValue("Client_id", GetIndexFromCombpBox(CB_chooseClientOrderForm.SelectedItem.ToString()));

            await SqlConnection.OpenAsync();
            await sqlCommand.ExecuteNonQueryAsync();
            SqlConnection.Close();

            await RefreshOrderTable();
            await DataInAllComboBox();
        }

        //END##########################################################################
    }
}
