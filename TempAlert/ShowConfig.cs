using System.Text;
using System.Windows.Forms;

namespace TempAlert.Client
{
    internal partial class ShowConfig : Form
    {
        private ConfigSet cs;
        public ShowConfig(ConfigSet c)
        {
            InitializeComponent();
            cs = c;

            var sb = new StringBuilder();

            sb.AppendLine("Valid configuration file found: " + cs.FileExists.ToString());
            sb.AppendLine("Sync to monitoring server enabled: " + cs.Syncs.ToString());
            sb.AppendLine("Monitoring server URL: " + cs.ServerUrl);
            sb.AppendLine("Update frequency: " + cs.UpdateInterval.ToString());
            sb.AppendLine("Upper threshold: " + cs.UpperThreshold + " celcius");
            sb.AppendLine("Critical threshold: " + cs.CriticalThreshold + " celcius");

            textBox1.Text = sb.ToString();
        }

        private void 
            ReloadButton_Click(object sender, System.EventArgs e)
        {
            cs.Reload();
        }
    }
}
