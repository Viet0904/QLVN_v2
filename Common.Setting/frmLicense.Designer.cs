namespace Common.Setting;

partial class frmLicense
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
        this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        this.txtMaSoFL = new System.Windows.Forms.TextBox();
        this.txtPassword = new System.Windows.Forms.TextBox();
        this.txtUserName = new System.Windows.Forms.TextBox();
        this.txtDatabaseName = new System.Windows.Forms.TextBox();
        this.txtServerIP = new System.Windows.Forms.TextBox();
        this.txtName = new System.Windows.Forms.TextBox();
        this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
        this.btnLoad = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        this.btnEncrypt = new System.Windows.Forms.Button();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.label6 = new System.Windows.Forms.Label();
        this.label7 = new System.Windows.Forms.Label();
        this.label8 = new System.Windows.Forms.Label();
        this.label9 = new System.Windows.Forms.Label();
        this.txtID = new System.Windows.Forms.TextBox();
        this.numSLMayCan = new System.Windows.Forms.NumericUpDown();
        this.tableLayoutPanel1.SuspendLayout();
        this.tableLayoutPanel2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numSLMayCan)).BeginInit();
        this.SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        this.tableLayoutPanel1.ColumnCount = 3;
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
        this.tableLayoutPanel1.Controls.Add(this.txtMaSoFL, 1, 6);
        this.tableLayoutPanel1.Controls.Add(this.txtPassword, 1, 5);
        this.tableLayoutPanel1.Controls.Add(this.txtUserName, 1, 4);
        this.tableLayoutPanel1.Controls.Add(this.txtDatabaseName, 1, 3);
        this.tableLayoutPanel1.Controls.Add(this.txtServerIP, 1, 2);
        this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 1);
        this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 8);
        this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
        this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
        this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
        this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
        this.tableLayoutPanel1.Controls.Add(this.label6, 0, 4);
        this.tableLayoutPanel1.Controls.Add(this.label7, 0, 5);
        this.tableLayoutPanel1.Controls.Add(this.label8, 0, 6);
        this.tableLayoutPanel1.Controls.Add(this.label9, 0, 7);
        this.tableLayoutPanel1.Controls.Add(this.txtID, 1, 0);
        this.tableLayoutPanel1.Controls.Add(this.numSLMayCan, 1, 7);
        this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        this.tableLayoutPanel1.RowCount = 10;
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.tableLayoutPanel1.Size = new System.Drawing.Size(364, 340);
        this.tableLayoutPanel1.TabIndex = 0;
        // 
        // txtMaSoFL
        // 
        this.txtMaSoFL.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtMaSoFL.Location = new System.Drawing.Point(112, 213);
        this.txtMaSoFL.Name = "txtMaSoFL";
        this.txtMaSoFL.Size = new System.Drawing.Size(230, 20);
        this.txtMaSoFL.TabIndex = 17;
        // 
        // txtPassword
        // 
        this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtPassword.Enabled = false;
        this.txtPassword.Location = new System.Drawing.Point(112, 178);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.PasswordChar = '*';
        this.txtPassword.Size = new System.Drawing.Size(230, 20);
        this.txtPassword.TabIndex = 16;
        // 
        // txtUserName
        // 
        this.txtUserName.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtUserName.Enabled = false;
        this.txtUserName.Location = new System.Drawing.Point(112, 143);
        this.txtUserName.Name = "txtUserName";
        this.txtUserName.Size = new System.Drawing.Size(230, 20);
        this.txtUserName.TabIndex = 15;
        // 
        // txtDatabaseName
        // 
        this.txtDatabaseName.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtDatabaseName.Location = new System.Drawing.Point(112, 108);
        this.txtDatabaseName.Name = "txtDatabaseName";
        this.txtDatabaseName.Size = new System.Drawing.Size(230, 20);
        this.txtDatabaseName.TabIndex = 14;
        // 
        // txtServerIP
        // 
        this.txtServerIP.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtServerIP.Enabled = false;
        this.txtServerIP.Location = new System.Drawing.Point(112, 73);
        this.txtServerIP.Name = "txtServerIP";
        this.txtServerIP.Size = new System.Drawing.Size(230, 20);
        this.txtServerIP.TabIndex = 13;
        // 
        // txtName
        // 
        this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtName.Enabled = false;
        this.txtName.Location = new System.Drawing.Point(112, 38);
        this.txtName.Name = "txtName";
        this.txtName.Size = new System.Drawing.Size(230, 20);
        this.txtName.TabIndex = 12;
        // 
        // tableLayoutPanel2
        // 
        this.tableLayoutPanel2.ColumnCount = 3;
        this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
        this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
        this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
        this.tableLayoutPanel2.Controls.Add(this.btnLoad, 0, 0);
        this.tableLayoutPanel2.Controls.Add(this.btnCancel, 2, 0);
        this.tableLayoutPanel2.Controls.Add(this.btnEncrypt, 1, 0);
        this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableLayoutPanel2.Location = new System.Drawing.Point(112, 283);
        this.tableLayoutPanel2.Name = "tableLayoutPanel2";
        this.tableLayoutPanel2.RowCount = 1;
        this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel2.Size = new System.Drawing.Size(230, 34);
        this.tableLayoutPanel2.TabIndex = 0;
        // 
        // btnLoad
        // 
        this.btnLoad.Location = new System.Drawing.Point(3, 3);
        this.btnLoad.Name = "btnLoad";
        this.btnLoad.Size = new System.Drawing.Size(69, 23);
        this.btnLoad.TabIndex = 2;
        this.btnLoad.Text = "Chọn file";
        this.btnLoad.UseVisualStyleBackColor = true;
        this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new System.Drawing.Point(155, 3);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(70, 23);
        this.btnCancel.TabIndex = 1;
        this.btnCancel.Text = "Hủy";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        // 
        // btnEncrypt
        // 
        this.btnEncrypt.Location = new System.Drawing.Point(79, 3);
        this.btnEncrypt.Name = "btnEncrypt";
        this.btnEncrypt.Size = new System.Drawing.Size(69, 23);
        this.btnEncrypt.TabIndex = 0;
        this.btnEncrypt.Text = "Xuất license";
        this.btnEncrypt.UseVisualStyleBackColor = true;
        this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
        // 
        // label2
        // 
        this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
        this.label2.Location = new System.Drawing.Point(3, 0);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(103, 35);
        this.label2.TabIndex = 2;
        this.label2.Text = "Id:";
        this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label3
        // 
        this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
        this.label3.Location = new System.Drawing.Point(3, 35);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(103, 35);
        this.label3.TabIndex = 3;
        this.label3.Text = "Tên công ty:";
        this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label4
        // 
        this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
        this.label4.Location = new System.Drawing.Point(3, 70);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(103, 35);
        this.label4.TabIndex = 4;
        this.label4.Text = "Server IP:";
        this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label5
        // 
        this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
        this.label5.Location = new System.Drawing.Point(3, 105);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(103, 35);
        this.label5.TabIndex = 5;
        this.label5.Text = "Database Name:";
        this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label6
        // 
        this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
        this.label6.Location = new System.Drawing.Point(3, 140);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(103, 35);
        this.label6.TabIndex = 6;
        this.label6.Text = "Database User:";
        this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label7
        // 
        this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
        this.label7.Location = new System.Drawing.Point(3, 175);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(103, 35);
        this.label7.TabIndex = 7;
        this.label7.Text = "Password:";
        this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label8
        // 
        this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
        this.label8.Location = new System.Drawing.Point(3, 210);
        this.label8.Name = "label8";
        this.label8.Size = new System.Drawing.Size(103, 35);
        this.label8.TabIndex = 8;
        this.label8.Text = "Mã số FL:";
        this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // label9
        // 
        this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
        this.label9.Location = new System.Drawing.Point(3, 245);
        this.label9.Name = "label9";
        this.label9.Size = new System.Drawing.Size(103, 35);
        this.label9.TabIndex = 9;
        this.label9.Text = "SL Máy Cân:";
        this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // txtID
        // 
        this.txtID.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtID.Enabled = false;
        this.txtID.Location = new System.Drawing.Point(112, 3);
        this.txtID.Name = "txtID";
        this.txtID.Size = new System.Drawing.Size(230, 20);
        this.txtID.TabIndex = 11;
        // 
        // numSLMayCan
        // 
        this.numSLMayCan.Dock = System.Windows.Forms.DockStyle.Fill;
        this.numSLMayCan.Location = new System.Drawing.Point(112, 248);
        this.numSLMayCan.Maximum = new decimal(new int[] {
        999,
        0,
        0,
        0});
        this.numSLMayCan.Name = "numSLMayCan";
        this.numSLMayCan.Size = new System.Drawing.Size(230, 20);
        this.numSLMayCan.TabIndex = 18;
        // 
        // frmLicense
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(364, 340);
        this.Controls.Add(this.tableLayoutPanel1);
        this.Name = "frmLicense";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "License Generator - QLVN";
        this.Load += new System.EventHandler(this.frmLicense_Load);
        this.tableLayoutPanel1.ResumeLayout(false);
        this.tableLayoutPanel1.PerformLayout();
        this.tableLayoutPanel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.numSLMayCan)).EndInit();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnEncrypt;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox txtID;
    private System.Windows.Forms.TextBox txtMaSoFL;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.TextBox txtUserName;
    private System.Windows.Forms.TextBox txtDatabaseName;
    private System.Windows.Forms.TextBox txtServerIP;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.NumericUpDown numSLMayCan;
    private System.Windows.Forms.Button btnLoad;
}
