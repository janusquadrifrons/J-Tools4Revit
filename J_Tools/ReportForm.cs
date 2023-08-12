using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static J_Tools.Command_12_ModelChecker;



namespace J_Tools
{
    public partial class ReportForm : Form
    { 

        // --- Model check report class
        private CheckReport report;

        public ReportForm(CheckReport report)
        {
            InitializeComponent();

            // --- Store reference to report
            this.report = report;

            // --- Populate text box

            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical; 
            textBox1.Text += "Model Checker Report" + Environment.NewLine;
            textBox1.Text += "---------------------" + Environment.NewLine;
            textBox1.Text += "All views list : " + report.AllViewsString + Environment.NewLine;
            textBox1.Text += "Views without templates : " + report.ViewsWithoutTemplates + Environment.NewLine;
            textBox1.Text += "Adopted views with sheets : " + report.AdoptedViews + Environment.NewLine;
            textBox1.Text += "Adopted view names : " + report.ViewsListString + Environment.NewLine;
            textBox1.Text += "Adopted view sheet numbers : " + report.SheetNumbersString + Environment.NewLine;
            textBox1.Text += "---------------------" + Environment.NewLine;
            textBox1.Text += "Elements far from origin : " + report.ElementsFarFromOrigin + Environment.NewLine;
            textBox1.Text += "Elements far from origin Ids : " + report.ElementsFarFromOriginIds + Environment.NewLine;
        }

        // --- Event handler for form load
        private void ReportForm_Load(object sender, EventArgs e)
        {

        }
    }
}
