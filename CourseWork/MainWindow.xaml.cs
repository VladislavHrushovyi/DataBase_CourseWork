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
                //string connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FornitureManufacture;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                string connection = @"Data Source=SSQLLocalDB;Initial Catalog=FornitureManufacture;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                SqlConnection = new SqlConnection(connection);
                await SqlConnection.OpenAsync();
                await DataInAllComboBox();
            }
            catch
            {
                MessageBox.Show("Далбайоб в тебе не заробе, де моя лаба по веб-прогрумаванні","Саня хуй",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        private async void ColorsDone(object sender, RoutedEventArgs e)
        {
            if (CB_operationColors.SelectedItem.ToString() == "ADD")
            {

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

        private async Task ClearAllComboBox()
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
        }
        private async Task DataInAllComboBox()
        {
            try
            {
                await ClearAllComboBox();
                sqlCommand = new SqlCommand("SELECT Client_id FROM [Orders]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_personOrder.Items.Add(reader[0].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,SecondName FROM [Client]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_personClient.Items.Add(reader[0].ToString()+" "+reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Departament]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_chooseDepartament.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,SecondName FROM [WorkerInfo]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_personWorkerInfo.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Product]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_selectProduct.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id FROM [Workers]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_chooseWorker.Items.Add(reader[0].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id FROM [Directors]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_chooseDirector.Items.Add(reader[0].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,SecondName FROM [DirectorInfo]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_personDirectorInfo.Items.Add(reader[0].ToString()+" "+reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Storage]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_Storage.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Component]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    CB_itemComponent.Items.Add(reader[0].ToString() + " " + reader[1].ToString());
                }
                reader.Close();
                sqlCommand = new SqlCommand("SELECT id,Name FROM [Colors]", SqlConnection);
                reader = await sqlCommand.ExecuteReaderAsync();
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

        private async void FillAllTable()
        {
            
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
