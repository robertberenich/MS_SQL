using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace MS_SQL_FINAL
{
    public partial class Form1 : Form
    {
        SqlConnection sqlConnection2 = null;
        SqlConnection sqlConnection = null;
        private List<string[]> rows = null;
        private List<string[]> filtered_list = null;

        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand(
                $"INSERT INTO [Students] (Name, Surname, Date_of_Birth) VALUES (N'{textBox1.Text}',N'{textBox2.Text}','{textBox3.Text}')",
                sqlConnection);

            MessageBox.Show("Complete!");
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["testDB"].ConnectionString);
            sqlConnection2 = new SqlConnection(ConfigurationManager.ConnectionStrings["Nord_DB"].ConnectionString);
            sqlConnection.Open();
            sqlConnection2.Open();

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select * from Products", sqlConnection2);
            DataSet dataSet = new DataSet();
            sqlDataAdapter.Fill(dataSet);
            dataGridView2.DataSource = dataSet.Tables[0];

            SqlDataReader sql_reader = null;
            SqlCommand sql_command = new SqlCommand("Select ProductName, QuantityPerUnit, UnitPrice from Products", sqlConnection2);
            sql_reader = sql_command.ExecuteReader();

            string[] row = null;
            rows = new List<string[]>();
            try
            {
                while (sql_reader.Read())
                {
                    row = new string[]
                    {
                    Convert.ToString(sql_reader[0]),
                    Convert.ToString(sql_reader[1]),
                    Convert.ToString(sql_reader[2])
                    };

                    rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if(sql_reader != null && !sql_reader.IsClosed)
                {
                    sql_reader.Close();
                }
            }
            refreshAndLoadList(rows);
        }
        private void refreshAndLoadList(List<string[]> list)
        {
            listView2.Items.Clear();

            foreach (var item in list)
            {
                listView2.Items.Add(new ListViewItem(item));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            SqlDataAdapter dataAdapter = new SqlDataAdapter(textBox7.Text,sqlConnection2);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlDataReader dataReader = null;

            try
            {
                SqlCommand sqlCommand = new SqlCommand(
                    "Select ProductName, QuantityPerUnit, UnitPrice From Products", sqlConnection2);

                dataReader = sqlCommand.ExecuteReader();

                ListViewItem listView = null;

                while(dataReader.Read())
                {
                    listView = new ListViewItem(new string[] { Convert.ToString(dataReader["ProductName"]),
                        Convert.ToString(dataReader["QuantityPerUnit"]),
                        Convert.ToString(dataReader["UnitPrice"])});
                    listView1.Items.Add(listView);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (dataReader != null && !dataReader.IsClosed)
                {
                    dataReader.Close();
                }
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            var f = $"ProductName LIKE '%{textBox8.Text}%'";
            (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = f;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var f = $"ProductName LIKE '%{textBox8.Text}%'";
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"{f} and UnitsInStock <= 10";
                    break;
                case 1:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter =
                        $"{f} and UnitsInStock <= 100 and UnitsInStock >= 50";
                    break;
                case 2:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter =
                        $"{f} and UnitsInStock >= 100";
                    break;
                case 3:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter =
                        "";
                    break;
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            filtered_list = rows.Where(r => r[0].ToLower().Contains(textBox9.Text.ToLower()))
                .ToList();
            refreshAndLoadList(filtered_list);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    filtered_list = rows.Where(r => Convert.ToDouble(r[2]) <= 10).ToList();
                    refreshAndLoadList(filtered_list);
                    break;
                case 1:
                    filtered_list = rows.Where(r => Convert.ToDouble(r[2]) >= 11 && Convert.ToDouble(r[2]) <=50).ToList();
                    refreshAndLoadList(filtered_list);
                    break;
                case 2:
                    filtered_list = rows.Where(r => Convert.ToDouble(r[2]) >= 51 && Convert.ToDouble(r[2]) <= 100).ToList();
                    refreshAndLoadList(filtered_list);
                    break;
                case 3:
                    filtered_list = rows.Where(r => Convert.ToDouble(r[2]) >= 101).ToList();
                    refreshAndLoadList(filtered_list);
                    break;
                case 4:
                    refreshAndLoadList(rows);
                    break;
            }
        }
    }
}
