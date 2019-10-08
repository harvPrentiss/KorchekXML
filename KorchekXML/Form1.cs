using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace KorchekXML
{
    public partial class Form1 : Form
    {
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;

        private bool dataChanged;

        private bool dataError;

        public Form1()
        {
            InitializeComponent();
            openFileDialog1 = new OpenFileDialog()
            {
                FileName = "Select an XML file",
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Open XML file"
            };

            saveFileDialog1 = new SaveFileDialog()
            {
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Save XML file"
            };

            dataChanged = false;
            dataError = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(dataChanged)
            {
                if(MessageBox.Show("Loading a new file will cause you to lose your current changes. Do you want to continue?", "Changes Will Be Lost", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        DataSet xmlDataSet = new DataSet();
                        try
                        {
                            xmlDataSet.ReadXml(openFileDialog1.FileName);
                            dataGridView1.DataSource = xmlDataSet.Tables[1];
                            label1.ForeColor = Color.Green;
                            label1.Text = "File loaded successfully.";
                            button2.Visible = true;
                        }
                        catch (Exception ex)
                        {
                            label1.ForeColor = Color.Red;
                            label1.Text = "There was an error reading the XML file.";
                        }
                    }
                }
            } 
            else
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    DataSet xmlDataSet = new DataSet();
                    try
                    {
                        xmlDataSet.ReadXml(openFileDialog1.FileName);
                        dataGridView1.DataSource = xmlDataSet.Tables["ExampleObject"];
                        label1.ForeColor = Color.Green;
                        label1.Text = "File loaded successfully.";
                        button2.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        label1.ForeColor = Color.Red;
                        label1.Text = "There was an error reading the XML file.";
                    }
                }
            }
        }

        private void dataGridView1_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if(e.Column.HeaderText == "DOB")
            {
                e.Column.DefaultCellStyle.Format = "d";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(!dataError)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    XmlWriter xWriter = XmlWriter.Create(saveFileDialog1.FileName);
                    try
                    {
                        

                        xWriter.WriteStartDocument();
                        xWriter.WriteStartElement("Objects");

                        foreach(DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[0].Value == null) continue;
                            xWriter.WriteStartElement("ExampleObject");
                            for(int i = 0; i < row.Cells.Count; i++)
                            {
                                string col = dataGridView1.Columns[i].Name;
                                string cellVal = row.Cells[i].Value.ToString();
                                xWriter.WriteAttributeString(col, cellVal);
                            }
                            xWriter.WriteEndElement();
                        }

                        xWriter.WriteEndElement();
                        xWriter.WriteEndDocument();

                        label1.ForeColor = Color.Green;
                        label1.Text = "File saved successfully";
                        xWriter.Close();
                    }
                    catch (Exception ex)
                    {
                        label1.ForeColor = Color.Red;
                        label1.Text = "There was an error saving the XML file.";
                        xWriter.Close();
                    }
                }
            }
            else
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Cannot save until errors are corrected.";
            }
            
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.dataChanged = true;
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string colText = dataGridView1.Columns[e.ColumnIndex].HeaderText;

            bool DOBError = false;
            bool widgetError = false;

            if(colText == "DOB")
            {
                DateTime temp;
                if(! DateTime.TryParse(e.FormattedValue.ToString(), out temp))
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "The string entered is not a valid date.";
                    e.Cancel = true;
                    label1.ForeColor = Color.Red;
                    label1.Text = "The string entered is not a valid date.";
                   DOBError = true;
                }
            }
            if(colText == "NumberOfWidgets")
            {
                int testInt;
                if(! int.TryParse(e.FormattedValue.ToString(), out testInt))
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "Content must be numeric";
                    e.Cancel = true;
                    label1.ForeColor = Color.Red;
                    label1.Text = "Content must be numeric";
                    widgetError = true;
                }
            }

            if(widgetError || DOBError)
            {
                dataError = true;
            }
            else
            {
                dataError = false;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ErrorText = String.Empty;
        }
    }
}
