using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imposto.Core.Domain;
using Imposto.Core.Service;
using Imposto.Core.Data;
using System.Data.SqlClient;

namespace TesteImposto
{
    public partial class FormImposto : Form
    {
        TESTEEntities db = new TESTEEntities();

        public FormImposto()
        {
            InitializeComponent();
            dataGridViewPedidos.AutoGenerateColumns = true;                       
            dataGridViewPedidos.DataSource = GetTablePedidos();
            ResizeColumns();
        }

        private void ResizeColumns()
        {
            double mediaWidth = dataGridViewPedidos.Width / dataGridViewPedidos.Columns.GetColumnCount(DataGridViewElementStates.Visible);

            for (int i = dataGridViewPedidos.Columns.Count - 1; i >= 0; i--)
            {
                var coluna = dataGridViewPedidos.Columns[i];
                coluna.Width = Convert.ToInt32(mediaWidth);
            }   
        }
        private object GetTablePedidos()
        {
            DataTable table = new DataTable("Pedidos");
            table.Columns.Add(new DataColumn("Nome", typeof(string)));
            table.Columns.Add(new DataColumn("Codigo", typeof(string)));
            table.Columns.Add(new DataColumn("Valor", typeof(decimal)));
            table.Columns.Add(new DataColumn("Brinde", typeof(bool)));

            return table;
        }
        private void buttonGerarNotaFiscal_Click(object sender, EventArgs e)
        {
            NotaFiscalService service = new NotaFiscalService();
            Pedido pedido = new Pedido();
            DataTable table = (DataTable)dataGridViewPedidos.DataSource;

            pedido.EstadoOrigem = cmbEstadoOrigem.Text;
            pedido.EstadoDestino = cmbEstadoDestino.Text;
            pedido.NomeCliente = txtNomeCliente.Text;

            foreach (DataRow row in table.Rows)
            {
                PedidoItem item = new PedidoItem();
                item.NomeProduto = row["Nome"].ToString();
                item.CodigoProduto = row["Codigo"].ToString();
                item.ValorItemPedido = Convert.ToDouble(row["Valor"].ToString());
                if(row["Brinde"].Equals("false")){
                    item.Brinde = false;
                }else{
                    item.Brinde = true;
                }
                pedido.ItensDoPedido.Add(item);
                item = null;
           }

            if (validar(pedido))
            {
                if (service.GerarNotaFiscal(pedido))
                    MessageBox.Show("Operação efetuada com sucesso");
                limparCampos();
            }


        }
        public void limparCampos() {
            txtNomeCliente.Clear();
            cmbEstadoOrigem.SelectedIndex = -1;
            cmbEstadoDestino.SelectedIndex = -1;
            dataGridViewPedidos.DataSource = null;
            dataGridViewPedidos.DataSource = GetTablePedidos();
            ResizeColumns();
        }
        private bool validar(Pedido pedido) {
            string campos = "Por favor preencha os campos: \n";
            int i = 0;

            if (String.IsNullOrEmpty(pedido.NomeCliente)) {
                campos = campos + "- Nome \n";
                i++;
            }

            if (String.IsNullOrEmpty(pedido.EstadoOrigem))
            {
                campos = campos + "- Estado Origem \n";
                i++;
            }
            if (String.IsNullOrEmpty(pedido.EstadoDestino))
            {
                campos = campos + "- Estado Destino \n";
                i++;
            }
            if (pedido.ItensDoPedido.Count < 1)
            {
                campos = campos + "- Preencha ao menos um item da nota \n";
                i++;
            }

            if (i > 0)
            {
                MessageBox.Show(campos);
                return false;
            }
            return true;
        }
        private void btnCarrega_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Por favor, configurar a String de Conexão.");
            //Sugestão futura selecionar o id e carregar os dados da nota. 
            
            this.dataGridNota.DataSource = db.NotaFiscalItems.ToList();
        }

        private void dataGridViewPedidos_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            MessageBox.Show("Ocorreu um errro: " + anError.Context.ToString());

            if (anError.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Erro ao inserir. /n");
            }
            if (anError.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Verifique alteração de campo");
            }
            if (anError.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("Verifique o formato do campo /n");
            }
            if (anError.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("leave control error");
            }

            if ((anError.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[anError.RowIndex].ErrorText = "an error";
                view.Rows[anError.RowIndex].Cells[anError.ColumnIndex].ErrorText = "an error";

                anError.ThrowException = false;
            }
        }

     


    }
}
