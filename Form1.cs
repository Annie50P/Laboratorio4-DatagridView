using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio4_DatagridView
{


    public partial class Form1 : Form
    {
        DateTimePicker eleccionTiempo = new DateTimePicker();
        Rectangle cellRectangle;

        public Form1()
        {
            InitializeComponent();
            dgvReservas.AllowUserToAddRows = false;
            dgvReservas.Controls.Add(eleccionTiempo);
            eleccionTiempo.Visible = false;
            eleccionTiempo.Format = DateTimePickerFormat.Short;
            eleccionTiempo.MinDate = DateTime.Today; 
            eleccionTiempo.TextChanged += new EventHandler(eleccionTiempo_TextChange);
            dgvReservas.CellClick += new DataGridViewCellEventHandler(dgvReservas_CellClick);
            dgvReservas.CellEndEdit += new DataGridViewCellEventHandler(dgvReservas_CellEndEdit);
        }

        private void eleccionTiempo_TextChange(Object sender, EventArgs e)
        {
            if (dgvReservas.CurrentCell != null)
            {
                
                dgvReservas.CurrentCell.Value = eleccionTiempo.Value.ToString("dd/MM/yyyy");

                if (dgvReservas.CurrentCell.OwningColumn.Name == "FechaSalida")
                {
                    DateTime fechaEntrada;
                    if (DateTime.TryParse(dgvReservas.Rows[dgvReservas.CurrentCell.RowIndex].Cells["FechaEntrada"].Value?.ToString(), out fechaEntrada))
                    {
                       
                        if (eleccionTiempo.Value <= fechaEntrada)
                        {
                            MessageBox.Show("La fecha de salida no puede ser anterior a la fecha de entrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dgvReservas.CurrentCell.Value = null; 
                        }
                        else
                        {
                            // Calcular días de estadía
                            int diasEstadia = (eleccionTiempo.Value - fechaEntrada).Days;
                            dgvReservas.Rows[dgvReservas.CurrentCell.RowIndex].Cells["DiasEstadia"].Value = diasEstadia;

                            // Calcular el costo total
                            CalcularCostoTotal(dgvReservas.CurrentCell.RowIndex);
                        }
                    }
                }
                else if (dgvReservas.CurrentCell.OwningColumn.Name == "FechaEntrada")
                {
                    dgvReservas.Rows[dgvReservas.CurrentCell.RowIndex].Cells["DiasEstadia"].Value = null;
                    dgvReservas.Rows[dgvReservas.CurrentCell.RowIndex].Cells["CostoTotal"].Value = null;
                }
            }
        }

        private void CalcularCostoTotal(int rowIndex)
        {
            var tipoHabitacion = dgvReservas.Rows[rowIndex].Cells["TipoHabitacion"].Value?.ToString();
            int diasEstadia = Convert.ToInt32(dgvReservas.Rows[rowIndex].Cells["DiasEstadia"].Value ?? 0);
            double tarifaPorNoche = 0;
            double costoTotal = 0;


            if (tipoHabitacion == "Individual")
                tarifaPorNoche = 50;
            else if (tipoHabitacion == "Doble")
                tarifaPorNoche = 75;
            else if (tipoHabitacion == "Suite")
                tarifaPorNoche = 120;

            // Calcular costo 
            costoTotal = tarifaPorNoche * diasEstadia;

            // Calcular ITBMS
            double CostoConItbms = costoTotal * 0.07;
            costoTotal =costoTotal+ CostoConItbms; 

            
            dgvReservas.Rows[rowIndex].Cells["CostoTotal"].Value = costoTotal;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void btagregar_Click(object sender, EventArgs e)
        {
            string[] nombresColumnas = { "Nombre Huésped", "Fecha de entrada", "Fecha de salida", "Tipo de habitación", "Días de estadía", "Costo total" };
            int[] anchosColumnas = { 150, 80, 80, 80, 80, 80 };

            
            if (dgvReservas.Columns.Count == 0)
            {
                //columna "Nombre Huésped"
                DataGridViewTextBoxColumn columnaNombre = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Nombre",
                    Name = "Nombre",
                    Width = anchosColumnas[0]
                };
                dgvReservas.Columns.Add(columnaNombre);

                // columna "Fecha de entrada"
                DataGridViewTextBoxColumn columnaFechaEntrada = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Fecha de entrada",
                    Name = "FechaEntrada",
                    Width = anchosColumnas[1]
                };
                dgvReservas.Columns.Add(columnaFechaEntrada);

                //columna "Fecha de salida"
                DataGridViewTextBoxColumn columnaFechaSalida = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Fecha de salida",
                    Name = "FechaSalida",
                    Width = anchosColumnas[2]
                };
                dgvReservas.Columns.Add(columnaFechaSalida);

                //columna "Tipo de habitación"
                DataGridViewComboBoxColumn columnaTipoHabitacion = new DataGridViewComboBoxColumn
                {
                    HeaderText = "Tipo de habitación",
                    Name = "TipoHabitacion",
                    Width = anchosColumnas[3],
                    Items = { "Individual", "Doble", "Suite" }
                };
                dgvReservas.Columns.Add(columnaTipoHabitacion);

                // columnas "Días de estadía" y "Costo total"
                DataGridViewTextBoxColumn columnaDiasEstadia = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Días de estadía",
                    Name = "DiasEstadia",
                    Width = anchosColumnas[4]
                };
                dgvReservas.Columns.Add(columnaDiasEstadia);

                DataGridViewTextBoxColumn columnaCostoTotal = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Costo total",
                    Name = "CostoTotal",
                    Width = anchosColumnas[5]
                };
                dgvReservas.Columns.Add(columnaCostoTotal);
            }

            // Deshabilitar el botón al presionarlo jejje
            btagregar.Enabled = false;
        }

        private void dgvReservas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
            if (e.ColumnIndex == dgvReservas.Columns["FechaEntrada"].Index || e.ColumnIndex == dgvReservas.Columns["FechaSalida"].Index)
            {
                
                cellRectangle = dgvReservas.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                eleccionTiempo.Size = new Size(cellRectangle.Width, cellRectangle.Height);
                eleccionTiempo.Location = new Point(cellRectangle.X, cellRectangle.Y);

                if (dgvReservas.CurrentCell.Value != null)
                {
                    DateTime tempDate;
                    if (DateTime.TryParse(dgvReservas.CurrentCell.Value.ToString(), out tempDate))
                    {
                        eleccionTiempo.Value = tempDate;
                    }
                    else
                    {
                        eleccionTiempo.Value = DateTime.Now; 
                    }
                }

                
                eleccionTiempo.Visible = true;
            }
            else
            {
                eleccionTiempo.Visible = false;
            }
        }

        private void dgvReservas_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            eleccionTiempo.Visible = false;
        }

        private void btAgregarReserva_Click(object sender, EventArgs e)
        {
            dgvReservas.Rows.Add();
        }
    }
}