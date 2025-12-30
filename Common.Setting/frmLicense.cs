using Common.Library.Helper;

namespace Common.Setting;

public partial class frmLicense : Form
{
    public frmLicense()
    {
        InitializeComponent();
    }

    private void frmLicense_Load(object sender, EventArgs e)
    {

    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
        using OpenFileDialog fileDialog = new OpenFileDialog
        {
            Filter = "License Files|*.lic"
        };

        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string filePath = fileDialog.FileName;
                if (File.Exists(filePath))
                {
                    string value = File.ReadAllText(fileDialog.FileName);
                    string decrypt = CryptorEngineHelper.Decrypt(value);
                    string[] decrypts = decrypt.Split(' ');
                    txtID.Text = CryptorEngineHelper.Decrypt(decrypts[0])[2..]; // Remove "ID" prefix
                    txtName.Text = CryptorEngineHelper.Decrypt(decrypts[1])[2..]; // Remove "CT" prefix
                    txtServerIP.Text = CryptorEngineHelper.Decrypt(decrypts[2])[2..]; // Remove "SV" prefix
                    txtDatabaseName.Text = CryptorEngineHelper.Decrypt(decrypts[3])[2..]; // Remove "DB" prefix
                    txtUserName.Text = CryptorEngineHelper.Decrypt(decrypts[4])[2..]; // Remove "US" prefix
                    txtPassword.Text = CryptorEngineHelper.Decrypt(decrypts[5])[2..]; // Remove "PS" prefix
                    txtMaSoFL.Text = CryptorEngineHelper.Decrypt(decrypts[6])[2..]; // Remove "FL" prefix
                }
                else
                {
                    MessageBox.Show("File không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đọc file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void btnEncrypt_Click(object sender, EventArgs e)
    {
        try
        {
            string result = CryptorEngineHelper.Encrypt("ID" + txtID.Text);
            result += " " + CryptorEngineHelper.Encrypt("CT" + txtName.Text);
            result += " " + CryptorEngineHelper.Encrypt("SV" + txtServerIP.Text);
            result += " " + CryptorEngineHelper.Encrypt("DB" + txtDatabaseName.Text.Trim());
            result += " " + CryptorEngineHelper.Encrypt("US" + txtUserName.Text);
            result += " " + CryptorEngineHelper.Encrypt("PS" + txtPassword.Text);
            result += " " + CryptorEngineHelper.Encrypt("FL" + txtMaSoFL.Text.Trim());
            result += " " + CryptorEngineHelper.Encrypt("SL" + numSLMayCan.Value);
            string encrypt = CryptorEngineHelper.Encrypt(result);

            using SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"License-{txtID.Text}-{DateTime.Now:ddMMyyyy}.lic",
                Filter = "License Files|*.lic"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, encrypt);
                MessageBox.Show("Tạo mã thành công, gửi file cho khách hàng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tạo license: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}