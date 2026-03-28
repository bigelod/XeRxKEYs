using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XeRxKEYs
{
    public partial class SelectInput : Form
    {
        private SendableInput selectedInput;

        private Action<SendableInput> returnAct;

        public SelectInput()
        {
            InitializeComponent();
        }

        private void SelectInput_Load(object sender, EventArgs e)
        {
            selectedInput = null;
            FormClosing += SelectInput_FormClosing;

            lstSendableInputList.Items.Clear();
            foreach (SendableInput input in InputHelper.AllSendableInputs)
            {
                lstSendableInputList.Items.Add(input.Type.ToString() + ": " + input.Name);
            }
        }

        private void SelectInput_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (returnAct != null)
            {
                returnAct.Invoke(selectedInput);
            }
        }

        public void SendInputTo(Action<SendableInput> act)
        {
            returnAct = act;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            selectedInput = null;
            Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (lstSendableInputList.SelectedIndex >= 0)
            {
                selectedInput = InputHelper.AllSendableInputs[lstSendableInputList.SelectedIndex];
            }
            Close();
        }

        private void lstSendableInputList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSendableInputList.SelectedIndex >= 0)
            {
                btnSelect.Enabled = true;
            }
            else
            {
                btnSelect.Enabled = false;
            }
        }
    }
}
