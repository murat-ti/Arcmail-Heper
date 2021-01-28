using System;
using System.Drawing;
using System.Windows.Forms;

namespace Arcmail
{
    public partial class ClientList : Form
    {

        private Panel buttonPanel = new Panel();
        private DataGridView clientsDataGridView = new DataGridView();
        private Button addNewRowButton = new Button();
        private Button deleteRowButton = new Button();

        /*public ClientList()
        {
            InitializeComponent();
        }*/

        public ClientList()
        {
            this.Text = "Clients";
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(System.Object sender, System.EventArgs e)
        {
            SetupLayout();
            SetupDataGridView();
            PopulateDataGridView();
        }

        private void clientsDataGridView_CellFormatting(object sender,
            System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {
            if (e != null)
            {
                /*if (this.clientsDataGridView.Columns[e.ColumnIndex].Name == "Release Date")
                {
                    if (e.Value != null)
                    {
                        try
                        {
                            e.Value = DateTime.Parse(e.Value.ToString())
                                .ToLongDateString();
                            e.FormattingApplied = true;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("{0} is not a valid date.", e.Value.ToString());
                        }
                    }
                }*/
            }
        }

        private void addNewRowButton_Click(object sender, EventArgs e)
        {
            this.clientsDataGridView.Rows.Add();
        }

        private void deleteRowButton_Click(object sender, EventArgs e)
        {
            if (this.clientsDataGridView.SelectedRows.Count > 0 &&
                this.clientsDataGridView.SelectedRows[0].Index !=
                this.clientsDataGridView.Rows.Count - 1)
            {
                this.clientsDataGridView.Rows.RemoveAt(
                    this.clientsDataGridView.SelectedRows[0].Index);
            }
        }

        private void SetupLayout()
        {
            this.Size = new Size(520, 300);

            /*addNewRowButton.Text = "Add Row";
            addNewRowButton.Location = new Point(10, 10);
            addNewRowButton.Click += new EventHandler(addNewRowButton_Click);

            deleteRowButton.Text = "Delete Row";
            deleteRowButton.Location = new Point(100, 10);
            deleteRowButton.Click += new EventHandler(deleteRowButton_Click);

            buttonPanel.Controls.Add(addNewRowButton);
            buttonPanel.Controls.Add(deleteRowButton);*/
            buttonPanel.Height = 50;
            buttonPanel.Dock = DockStyle.Bottom;

            this.Controls.Add(this.buttonPanel);
        }

        private void SetupDataGridView()
        {
            this.Controls.Add(clientsDataGridView);

            clientsDataGridView.ColumnCount = 5;

            clientsDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            clientsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            clientsDataGridView.ColumnHeadersDefaultCellStyle.Font =
                new Font(clientsDataGridView.Font, FontStyle.Bold);

            clientsDataGridView.Name = "clientsDataGridView";
            clientsDataGridView.Location = new Point(8, 8);
            clientsDataGridView.Size = new Size(500, 250);
            clientsDataGridView.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            clientsDataGridView.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.Single;
            clientsDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            clientsDataGridView.GridColor = Color.Black;
            clientsDataGridView.RowHeadersVisible = false;

            clientsDataGridView.Columns[0].Name = "Bank";
            clientsDataGridView.Columns[1].Name = "Code";
            clientsDataGridView.Columns[2].Name = "FTP IN";
            clientsDataGridView.Columns[3].Name = "FTP OUT";
            clientsDataGridView.Columns[4].Name = "Proc. code";

            clientsDataGridView.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            clientsDataGridView.MultiSelect = false;
            clientsDataGridView.Dock = DockStyle.Fill;

            /*clientsDataGridView.CellFormatting += new
                DataGridViewCellFormattingEventHandler(
                clientsDataGridView_CellFormatting);*/
        }

        private void PopulateDataGridView()
        {
            FileHelper fh = new FileHelper();
            string[][] data = fh.readClientsToArray();

            if (data.Length > 0)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    string[] row = data[i];
                    clientsDataGridView.Rows.Add(row);
                    //Console.Write(data[i][0]+"\n");
                }
            }


            /*string[] row0 = { "BPC", "0000", "kartadmin/in", "kartadmin/out" };
            string[] row1 = { "DAYHANBANK", "1409", "dbank_1409/in", "dbank_1409/out" };
            string[] row2 = { "TURKMENBASI", "1304", "tbashy_1304/in", "tbashy_1304/out" };
            string[] row3 = { "TURKMENISTAN", "1506", "tbank_1506/in", "tbank_1506/out" };
            string[] row4 = { "RYSGALBANK", "1738", "rbank_1738/in", "rbank_1738/out" };
            string[] row5 = { "SENAGATBANK", "1706", "sbank_1706/in", "sbank_1706/out" };
            string[] row6 = { "TURKMENTURK", "1731", "tturk_1731/in", "tturk_1731/out" };*/

            /*clientsDataGridView.Rows.Add(row0);
            clientsDataGridView.Rows.Add(row1);
            clientsDataGridView.Rows.Add(row2);
            clientsDataGridView.Rows.Add(row3);
            clientsDataGridView.Rows.Add(row4);
            clientsDataGridView.Rows.Add(row5);
            clientsDataGridView.Rows.Add(row6);*/

            /*clientsDataGridView.Columns[0].DisplayIndex = 3;
            clientsDataGridView.Columns[1].DisplayIndex = 4;
            clientsDataGridView.Columns[2].DisplayIndex = 0;
            clientsDataGridView.Columns[3].DisplayIndex = 1;
            clientsDataGridView.Columns[4].DisplayIndex = 2;*/
        }

        /*public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }


            return dt;
        }*/
    }
}
