namespace Common.Setting;

partial class frmMain
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.btnDecryptEncrypt = new System.Windows.Forms.Button();
        this.btnLicense = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // btnDecryptEncrypt
        // 
        this.btnDecryptEncrypt.Location = new System.Drawing.Point(33, 25);
        this.btnDecryptEncrypt.Name = "btnDecryptEncrypt";
        this.btnDecryptEncrypt.Size = new System.Drawing.Size(97, 23);
        this.btnDecryptEncrypt.TabIndex = 0;
        this.btnDecryptEncrypt.Text = "Mã hóa";
        this.btnDecryptEncrypt.UseVisualStyleBackColor = true;
        this.btnDecryptEncrypt.Click += new System.EventHandler(this.btnDecryptEncrypt_Click);
        // 
        // btnLicense
        // 
        this.btnLicense.Location = new System.Drawing.Point(172, 25);
        this.btnLicense.Name = "btnLicense";
        this.btnLicense.Size = new System.Drawing.Size(97, 23);
        this.btnLicense.TabIndex = 1;
        this.btnLicense.Text = "Kích hoạt";
        this.btnLicense.UseVisualStyleBackColor = true;
        this.btnLicense.Click += new System.EventHandler(this.btnLicense_Click);
        // 
        // frmMain
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(304, 88);
        this.Controls.Add(this.btnLicense);
        this.Controls.Add(this.btnDecryptEncrypt);
        this.Name = "frmMain";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Common Settings - QLVN";
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Button btnDecryptEncrypt;
    private System.Windows.Forms.Button btnLicense;
}
