using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Практика2курс
{
    enum Rowstate
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }

    public partial class Form1 : Form
    {
        DataBase DataBase = new DataBase();
        int selectedRow;

        public Form1()
        {
            InitializeComponent();
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns.Add("type_of", "Тип товара");
            dataGridView1.Columns.Add("count_of", "Количество");
            dataGridView1.Columns.Add("postavka", "Поставщик");
            dataGridView1.Columns.Add("price", "Цена");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns[5].Visible = false;
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetInt32(2), record.GetString(3), record.GetInt32(4), Rowstate.ModifiedNew);
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = "SELECT * FROM test1";

            SQLiteCommand command = new SQLiteCommand(queryString, DataBase.getConnection());
            DataBase.openConnection();

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
            DataBase.closeConnection();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataBase.openConnection();

            var type = textBox1.Text;
            var count = textBox2.Text;
            var postav = textBox3.Text;
            int price;

            if (int.TryParse(textBox4.Text, out price))
            {
                string addQuery = "INSERT INTO test1 (type_of, count_of, postavka, price) VALUES (@type, @count, @postav, @price)";
                using (SQLiteCommand command = new SQLiteCommand(addQuery, DataBase.getConnection()))
                {
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@count", count);
                    command.Parameters.AddWithValue("@postav", postav);
                    command.Parameters.AddWithValue("@price", price);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                MessageBox.Show("Цена должна быть числом!");
            }

            DataBase.closeConnection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[index].Visible = false;
            dataGridView1.Rows[index].Cells[5].Value = Rowstate.Deleted;
        }

        private void Update()
        {
            DataBase.openConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (Rowstate)dataGridView1.Rows[index].Cells[5].Value;

                if (rowState == Rowstate.Existed)
                    continue;

                if (rowState == Rowstate.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    string deleteQuery = "DELETE FROM test1 WHERE id = @id";
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, DataBase.getConnection()))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }

                if (rowState == Rowstate.Modified)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var type = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var count = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var postavka = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var price = Convert.ToInt32(dataGridView1.Rows[index].Cells[4].Value);

                    string changeQuery = "UPDATE test1 SET type_of = @type, count_of = @count, postavka = @postavka, price = @price WHERE id = @id";
                    using (SQLiteCommand command = new SQLiteCommand(changeQuery, DataBase.getConnection()))
                    {
                        command.Parameters.AddWithValue("@type", type);
                        command.Parameters.AddWithValue("@count", count);
                        command.Parameters.AddWithValue("@postavka", postavka);
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }

            DataBase.closeConnection();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы уверены?",
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

            if (result == DialogResult.Yes)
            {
                deleteRow();
                Update();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                textBox5.Text = row.Cells[0].Value.ToString();
                textBox1.Text = row.Cells[1].Value.ToString();
                textBox2.Text = row.Cells[2].Value.ToString();
                textBox3.Text = row.Cells[3].Value.ToString();
                textBox4.Text = row.Cells[4].Value.ToString();
            }
        }

        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
            var id = textBox5.Text;
            var type = textBox1.Text;
            var count = textBox2.Text;
            var postav = textBox3.Text;
            int price;

            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBox4.Text, out price))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(id, type, count, postav, price);
                    dataGridView1.Rows[selectedRowIndex].Cells[5].Value = Rowstate.Modified;
                }
                else
                {
                    MessageBox.Show("Цена должна быть целым числом");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Change();
            Update();
        }
    }
}