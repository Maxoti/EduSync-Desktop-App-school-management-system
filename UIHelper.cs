using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EduSync
{
    internal class UIHelper
    {
       

    
        
            public static void StyleGrid(DataGridView dgv)
            {
                dgv.DefaultCellStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                dgv.RowHeadersVisible = false;
            }
        
    }

}

