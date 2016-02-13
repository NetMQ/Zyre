using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SamplePeer
{
    public class Utility
    {
        /// <summary>
        /// Provide generic error handling for a DataGridView error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void HandleDataGridViewError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var dgv = (DataGridView)sender;
            var senderName = dgv.Name;
            var senderError = senderName + "_DataError()";
            MessageBox.Show("Error happened " + e.Context.ToString() + "\n" + e.Exception, senderError);

            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error", senderError);
            }
            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Cell change", senderError);
            }
            if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("Parsing error", senderError);
            }
            if (e.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("Leave control error", senderError);
            }

            if ((e.Exception) is System.Data.ConstraintException)
            {
                var view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }
    }
}
