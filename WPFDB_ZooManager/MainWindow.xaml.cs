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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPFDB_ZooManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {



            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["WPFDB_ZooManager.Properties.Settings.TutorialsDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            showZoos();
            showAnimals();
        }

        public void showZoos()
        {
            try
            {
                string query = "select * from Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();
                    sqlDataAdapter.Fill(zooTable);

                    // Welche Informationen der Tabelle in unserem Datatable sollen in unserer Listbox angezeigt werden
                    listZoos.DisplayMemberPath = "Location";
                    // Welcher Wert soll gegeben werden, wenn eines unsere Items von der Listbox ausgewählt wird
                    listZoos.SelectedValuePath = "Id";
                    //
                    // listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
        }

        public void showAssociatedAnimals()
        {
            if (listZoos.SelectedValue == null)
            {
                return;
            }
            try
            {
                string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId where za.ZooId = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);

                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    // Welche Informationen der Tabelle in unserem Datatable sollen in unserer Listbox angezeigt werden
                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    // Welcher Wert soll gegeben werden, wenn eines unsere Items von der Listbox ausgewählt wird
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    // 
                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
        }

        public void showAnimals()
        {
            try
            {
                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalsTable = new DataTable();
                    sqlDataAdapter.Fill(animalsTable);

                    // Namen des Tieres
                    listAnimals.DisplayMemberPath = "Name";
                    // Item Source hinter dem Namen z.b Wolf = 1
                    listAnimals.SelectedValuePath = "Id";
                    // Item Default Source
                    listAnimals.ItemsSource = animalsTable.DefaultView;
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showAssociatedAnimals();
            ShowSelectedZooInTextBox();
        }

        private void listAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedAnimalInTextBox();
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Zoo where id = @ZooID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooID", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                // Zoos updatend
                showZoos();
            }
        }

        public void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                // führt den sqlcommand code aus
                sqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler beim hinzufügen");
            }
            finally
            {
                sqlConnection.Close();
                showZoos();
            }
        }

        public void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            if (listZoos.SelectedValue == null || listAnimals.SelectedValue == null)
            {
                return;
            }
            try
            {
                string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler beim Einfügen eines Tieres in einen Zoo");

            }
            finally
            {
                sqlConnection.Close();
                showAssociatedAnimals();
            }
        }

        public void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            if (myTextBox == null)
            {
                return;
            }

            try
            {
                string query = "insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {

                MessageBox.Show("Fehler beim hinzufügen von Tieren");
            }
            finally
            {
                sqlConnection.Close();
                showAnimals();
            }
        }

        public void DeleteAssociatedAnimal_Click(object sender, RoutedEventArgs e)
        {
            if (listZoos.SelectedValue == null || listAssociatedAnimals.SelectedValue == null)
            {
                return;
            }

            try
            {
                string query = "delete from ZooAnimal where ZooId = @ZooId AND AnimalId = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAssociatedAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {

                MessageBox.Show("Fehler beim entfernen eines Tieres aus einem Zoo");
            }
            finally
            {
                sqlConnection.Close();
                showAssociatedAnimals();
            }
        }

        public void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {
            if (listAnimals.SelectedValue == null)
            {
                return;
            }

            try
            {
                string query = "delete from Animal where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {

                MessageBox.Show("Fehler beim löschen eines Tieres");
            }
            finally
            {
                sqlConnection.Close();
                showAnimals();
                showAssociatedAnimals();
            }
        }

        private void ShowSelectedZooInTextBox()
        {
            if (listZoos.SelectedValue == null)
            {
                return;
            }
            try
            {
                string query = "select location from Zoo where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                    DataTable zooDataTable = new DataTable();
                    sqlDataAdapter.Fill(zooDataTable);

                    myTextBox.Text = zooDataTable.Rows[0]["Location"].ToString();
                }

            }
            catch (Exception)
            {

                MessageBox.Show("Fehler beim Anzeigen eines Zoos in TextBox");
            }
        }

        private void ShowSelectedAnimalInTextBox()
        {
            if (listAnimals.SelectedValue == null)
            {
                return;
            }
            try
            {
                string qery = "select name from Animal where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(qery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    DataTable animalDataTable = new DataTable();
                    sqlDataAdapter.Fill(animalDataTable);

                    myTextBox.Text = animalDataTable.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception e)
            {

                //MessageBox.Show("Fehler beim Anzeigen eines Tieres in TextBox");
                MessageBox.Show(e.ToString());
            }
        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            if (listZoos.SelectedValue == null || myTextBox.Text == null || myTextBox.Text == "")
            {
                return;
            }
            try
            {
                string query = "update Zoo Set Location = @Location where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler beim updaten eines Zoos");
            }
            finally
            {
                sqlConnection.Close();
                showZoos();
            }
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            if (listAnimals.SelectedValue == null || myTextBox.Text == null || myTextBox.Text == "")
            {
                return;
            }
            try
            {
                string query = "update Animal Set Name = @Name where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler beim updaten eines Animals");
            }
            finally
            {
                sqlConnection.Close();
                showAnimals();
                showAssociatedAnimals();
            }
        }

    }
}
