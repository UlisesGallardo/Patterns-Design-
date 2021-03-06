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

namespace Store
{
    public partial class NuevoPedido : UserControl
    {
        public System.Data.DataTable dt { get; set; }
        public string previous { get; set; }

        public List<Producto> _lista_productos;
        public Tienda _tienda = null;

        public Tienda NuevaTienda
        {
            set
            {
                _tienda = value;
            }
        }
        

        public NuevoPedido()
        {
            InitializeComponent();
        }

        private void AgregarProducto_Click(object sender, EventArgs e)
        {
            if (NombreProducto.Text.Length <= 0)
            {
                DialogResult dialogResult = MessageBox.Show("El nombre del producto está vacío.!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (Form forms in Application.OpenForms)
            {
                string name = forms.Name;
                if (name == "WriteLogs")
                {
                    WriteLogs form = (WriteLogs)Application.OpenForms["WriteLogs"];
                    form.logs.execute("Agregando Producto ");
                    break;
                }
            }

            Producto p = new Producto();

            p.ID_Producto = (dt.Rows.Count + 1).ToString();
            p.NombreProducto = NombreProducto.Text;
            p.Cantidad = numericUpDown1.Value.ToString();
            _lista_productos.Add(p);

            DataRow dr = dt.NewRow();
            dr["ID"] = dt.Rows.Count + 1;
            dr["Producto"] = NombreProducto.Text;
            dr["Cantidad"] = numericUpDown1.Value;
            dt.Rows.Add(dr);
        }

        private void Guardar_Click(object sender, EventArgs e)
        {
            if (_lista_productos.Count <= 0)
            {
                DialogResult dialogResult = MessageBox.Show("No se ha agregado ningun producto.!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nombreTienda01.Text.Length <= 0)
            {
                DialogResult dialogResult = MessageBox.Show("El nombre de la tienda está vacío.!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Tienda t = new Tienda();

            DataTable dtFromGrid = new DataTable();
            dtFromGrid = (dataGridView1.DataSource as DataTable).Copy();


            List<Producto> listaProducto = new List<Producto>();
            for (int i = 0; i < dtFromGrid.Rows.Count; i++)
            {
                Producto p = new Producto();
                p.ID_Producto = dtFromGrid.Rows[i]["ID"].ToString();
                p.NombreProducto = dtFromGrid.Rows[i]["Producto"].ToString();
                p.Cantidad = dtFromGrid.Rows[i]["Cantidad"].ToString();
                listaProducto.Add(p);
            }

            t.NombreTienda = nombreTienda01.Text;
            t.Productos = listaProducto;

            string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Pedidos", "*.*", SearchOption.AllDirectories);

            bool flag = false;

            for (int i=0; i<allfiles.Length; i++)
            {
                string []words = allfiles[i].Split("\\");
                string name = words[words.Length-1]; 
                if(name == t.NombreTienda+".png")
                {
                    flag = true;
                    break;
                }
            }

            if (flag && previous != "surtir")
            {
                    DialogResult dialogResult = MessageBox.Show("En el directorio actual, el nombre de la tienda está repetido!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;  
            }

            StoreTo2Dcode adapter_to_QR = new _2DCodeAdapter("QR");
            adapter_to_QR.Convert(t);
            adapter_to_QR.Save2dCode(Directory.GetCurrentDirectory() + "\\Pedidos\\" + t.NombreTienda + ".png");

  
            if (previous == "general")
            {
                foreach (Form forms in Application.OpenForms)
                {
                    string name = forms.Name;
                    if (name == "WriteLogs")
                    {
                        WriteLogs form = (WriteLogs)Application.OpenForms["WriteLogs"];
                        form.logs.execute("Finalizar pedido ");
                        break;
                    }
                }

                this.ParentForm.Hide();
                Form1 frm = new Form1();
                frm.Show();
            }
            else if(previous == "surtir")
            {
                foreach (Form forms in Application.OpenForms)
                {
                    string name = forms.Name;
                    if (name == "WriteLogs")
                    {
                        WriteLogs form = (WriteLogs)Application.OpenForms["WriteLogs"];
                        form.logs.execute("Pedido Actualizado ");
                        break;
                    }
                }
                SurtirPedido a =  (SurtirPedido)this.ParentForm;
                a.UpdateImages();   
            }
        }

        private void NuevoPedido_Load(object sender, EventArgs e)
        {
            if (previous == "general")
            {
                foreach (Form forms in Application.OpenForms)
                {
                    string name = forms.Name;
                    if (name == "WriteLogs")
                    {
                        WriteLogs form = (WriteLogs)Application.OpenForms["WriteLogs"];
                        form.logs.execute("Crear nuevo pedido ");
                        break;
                    }
                }
            }
            else
            {
                nombreTienda01.Visible = false;
                NombreTienda.Visible = false;
                foreach (Form forms in Application.OpenForms)
                {
                    string name = forms.Name;
                    if (name == "WriteLogs")
                    {
                        WriteLogs form = (WriteLogs)Application.OpenForms["WriteLogs"];
                        form.logs.execute("Actualizando Pedido ");
                        break;
                    }
                }
            }

            dt = new DataTable();
            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("Producto", typeof(string)));
            dt.Columns.Add(new DataColumn("Cantidad", typeof(int)));
            dataGridView1.DataSource = dt;

            var deleteButton = new DataGridViewButtonColumn();
            deleteButton.Name = "dataGridViewDeleteButton";
            deleteButton.HeaderText = "Delete";
            deleteButton.Text = "Delete";
            deleteButton.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(deleteButton);

            _lista_productos = new List<Producto>();

            if (_tienda != null)
            {
                _lista_productos = _tienda.Productos;
                nombreTienda01.Text = _tienda.NombreTienda;

                for (int i = 0; i < _lista_productos.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = dt.Rows.Count + 1;
                    dr["Producto"] = _lista_productos[i].NombreProducto;
                    dr["Cantidad"] = _lista_productos[i].Cantidad;
                    dt.Rows.Add(dr);
                }
            }
            
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.RowIndex == dataGridView1.NewRowIndex || e.RowIndex < 0)
                return;

            if (e.ColumnIndex == dataGridView1.Columns["dataGridViewDeleteButton"].Index)
            {
                _lista_productos.RemoveAt(e.RowIndex);
                dt.Rows.RemoveAt(e.RowIndex);
                Console.WriteLine(e.RowIndex);
            }   
        }
    }
}
