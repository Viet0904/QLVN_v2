namespace Common.Setting;

public partial class frmMain : Form
{
    public frmMain()
    {
        InitializeComponent();
    }

    private void btnDecryptEncrypt_Click(object sender, EventArgs e)
    {
        using frmDecryptEncrypt frm = new frmDecryptEncrypt();
        frm.ShowDialog();
    }

    private void btnLicense_Click(object sender, EventArgs e)
    {
        using frmLicense frm = new frmLicense();
        frm.ShowDialog();
    }
}