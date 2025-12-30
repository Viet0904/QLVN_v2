using Common.Library.Helper;

namespace Common.Setting;

public partial class frmDecryptEncrypt : Form
{
    public frmDecryptEncrypt()
    {
        InitializeComponent();
    }

    private void btnEncrypt_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtData.Text))
        {
            txtValue.Text = CryptorEngineHelper.Encrypt(txtData.Text);
        }
    }

    private void btnDecrypt_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtData.Text))
        {
            txtValue.Text = CryptorEngineHelper.Decrypt(txtData.Text);
        }
    }
}
