using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapControlApplication1
{
    public partial class DataBoard : Form
    {
        public DataBoard(String sDataName,DataTable dataTable)
        {
            //初始化窗体及控件
            InitializeComponent();
            //设置文本框中的文本和数据网络视图的数据源
            tbDataName.Text = sDataName;
            dataGridView1.DataSource = dataTable;
            
            /*checkedListBox1.DataSource = dataTable;
            checkedListBox1.ValueMember = "student_id";
            checkedListBox1.DisplayMember = "student_name";*/
        }
        int m_number;
        GeometryOperator m_operator;
        public DataBoard(String sDataName,DataTable dataTable,GeometryOperator geometryOperator,int number)
        {
            InitializeComponent();
            tbDataName.Text = sDataName;
            dataGridView1.DataSource = dataTable;
            if (geometryOperator != null)
            {
                m_operator = geometryOperator;
            }
            m_number = number;
            label1.Text = "Point: "+ "/" + Convert.ToString(m_number);
        }
        
        //内容点击响应函数
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_operator != null) 
            {
                m_operator.HighLight(e.RowIndex);
                label1.Text = "Point: "+Convert.ToString(e.RowIndex+1)+"/"+Convert.ToString(m_number);
            }
        }
        //内容值改变响应函数
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_operator != null) 
            {
                double newvalue = Convert.ToDouble(dataGridView1.CurrentCell.Value);
                m_operator.Update(newvalue, e.RowIndex, e.ColumnIndex);
            }
            miFinish.Enabled = true;
        }

        private void DataBoard_FormClosed(object sender, FormClosedEventArgs e)
        {
           // m_operator.EndShow();
            //m_operator.m_form.setButton_miShowTime(true);
        }

        private void miFinish_Click(object sender, EventArgs e)
        {
            m_operator.EndShow();
            dataGridView1.ReadOnly = true;//修改完成之后，属性变为只读
            miFinish.Enabled = false;
            miUpdate.Enabled = true;
        }

        private void miUpdate_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;//点击开始修改，属性可以修改
            miUpdate.Enabled = false;
            miFinish.Enabled = true;
        }

        private void dataGridView1_CursorChanged(object sender, EventArgs e)
        {
            if (m_operator != null)
            {           
                m_operator.HighLight(Convert.ToInt32(dataGridView1.CurrentRow.Index.ToString()));
                label1.Text = "Point: " + Convert.ToString(dataGridView1.CurrentRow.Index.ToString() + 1) + "/" + Convert.ToString(m_number);
            }
        }

    }
}
