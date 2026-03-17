namespace InvoiceGenerator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            panel1 = new Panel();
            panel11 = new Panel();
            pictureBox3 = new PictureBox();
            label50 = new Label();
            label49 = new Label();
            panel10 = new Panel();
            label48 = new Label();
            cbTransportMode = new ComboBox();
            tbxRemark = new TextBox();
            lblTotalValueWord = new Label();
            btnSave = new Button();
            btnPrint = new Button();
            label45 = new Label();
            label44 = new Label();
            label24 = new Label();
            label43 = new Label();
            panel5 = new Panel();
            tbxVN = new TextBox();
            tbxDC = new TextBox();
            tbxPON = new TextBox();
            dtDC = new DateTimePicker();
            dtPOD = new DateTimePicker();
            DTds = new DateTimePicker();
            dtID = new DateTimePicker();
            lblID = new Label();
            label23 = new Label();
            label22 = new Label();
            label21 = new Label();
            label20 = new Label();
            label19 = new Label();
            label18 = new Label();
            label17 = new Label();
            label16 = new Label();
            label42 = new Label();
            label41 = new Label();
            label40 = new Label();
            label39 = new Label();
            label38 = new Label();
            label37 = new Label();
            label34 = new Label();
            tbxTotalValue = new TextBox();
            tbxIGST = new TextBox();
            tbxSGST = new TextBox();
            tbxCGST = new TextBox();
            txtTaxableValue = new TextBox();
            dgvItems = new DataGridView();
            panel9 = new Panel();
            lblSTGSTIN = new Label();
            lblSTS = new Label();
            lblSTSC = new Label();
            lblSTAdd = new Label();
            cbST = new ComboBox();
            label36 = new Label();
            label35 = new Label();
            label33 = new Label();
            label30 = new Label();
            panel8 = new Panel();
            lblITPT = new Label();
            lblITGSTIN = new Label();
            lblITS = new Label();
            cbIT = new ComboBox();
            lblITSC = new Label();
            lblITAdd = new Label();
            label32 = new Label();
            label31 = new Label();
            label29 = new Label();
            label28 = new Label();
            label27 = new Label();
            panel7 = new Panel();
            label26 = new Label();
            panel6 = new Panel();
            label25 = new Label();
            panel4 = new Panel();
            label15 = new Label();
            label14 = new Label();
            label13 = new Label();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            panel3 = new Panel();
            label1 = new Label();
            panel2 = new Panel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            sqlCommand2 = new Microsoft.Data.SqlClient.SqlCommand();
            imageList1 = new ImageList(components);
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel1.SuspendLayout();
            panel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            panel10.SuspendLayout();
            panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            panel9.SuspendLayout();
            panel8.SuspendLayout();
            panel7.SuspendLayout();
            panel6.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1370, 749);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.AutoScroll = true;
            tabPage1.Controls.Add(panel1);
            tabPage1.Location = new Point(4, 5);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1362, 740);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Invoice Details";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(panel11);
            panel1.Controls.Add(panel10);
            panel1.Controls.Add(cbTransportMode);
            panel1.Controls.Add(tbxRemark);
            panel1.Controls.Add(lblTotalValueWord);
            panel1.Controls.Add(btnSave);
            panel1.Controls.Add(btnPrint);
            panel1.Controls.Add(label45);
            panel1.Controls.Add(label44);
            panel1.Controls.Add(label24);
            panel1.Controls.Add(label43);
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(label42);
            panel1.Controls.Add(label41);
            panel1.Controls.Add(label40);
            panel1.Controls.Add(label39);
            panel1.Controls.Add(label38);
            panel1.Controls.Add(label37);
            panel1.Controls.Add(label34);
            panel1.Controls.Add(tbxTotalValue);
            panel1.Controls.Add(tbxIGST);
            panel1.Controls.Add(tbxSGST);
            panel1.Controls.Add(tbxCGST);
            panel1.Controls.Add(txtTaxableValue);
            panel1.Controls.Add(dgvItems);
            panel1.Controls.Add(panel9);
            panel1.Controls.Add(panel8);
            panel1.Controls.Add(panel7);
            panel1.Controls.Add(panel6);
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1339, 749);
            panel1.TabIndex = 2;
            // 
            // panel11
            // 
            panel11.BorderStyle = BorderStyle.FixedSingle;
            panel11.Controls.Add(pictureBox3);
            panel11.Controls.Add(label50);
            panel11.Controls.Add(label49);
            panel11.Location = new Point(669, 629);
            panel11.Name = "panel11";
            panel11.Size = new Size(668, 76);
            panel11.TabIndex = 57;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(264, 14);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(157, 57);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 50;
            pictureBox3.TabStop = false;
            // 
            // label50
            // 
            label50.AutoSize = true;
            label50.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label50.Location = new Point(518, 58);
            label50.Name = "label50";
            label50.Size = new Size(145, 16);
            label50.TabIndex = 49;
            label50.Text = "Authorised Signatory";
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label49.Location = new Point(457, 3);
            label49.Name = "label49";
            label49.Size = new Size(206, 16);
            label49.TabIndex = 49;
            label49.Text = " For , CodNow Technologies";
            // 
            // panel10
            // 
            panel10.BorderStyle = BorderStyle.FixedSingle;
            panel10.Controls.Add(label48);
            panel10.Location = new Point(0, 629);
            panel10.Name = "panel10";
            panel10.Size = new Size(668, 76);
            panel10.TabIndex = 56;
            // 
            // label48
            // 
            label48.AutoSize = true;
            label48.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label48.Location = new Point(4, 0);
            label48.Name = "label48";
            label48.Size = new Size(185, 16);
            label48.TabIndex = 47;
            label48.Text = "Customer's Seal & Signature";
            // 
            // cbTransportMode
            // 
            cbTransportMode.FormattingEnabled = true;
            cbTransportMode.Location = new Point(130, 548);
            cbTransportMode.Name = "cbTransportMode";
            cbTransportMode.Size = new Size(121, 23);
            cbTransportMode.TabIndex = 55;
            // 
            // tbxRemark
            // 
            tbxRemark.BackColor = SystemColors.ActiveCaption;
            tbxRemark.Location = new Point(81, 590);
            tbxRemark.MaxLength = 200;
            tbxRemark.Multiline = true;
            tbxRemark.Name = "tbxRemark";
            tbxRemark.Size = new Size(366, 36);
            tbxRemark.TabIndex = 54;
            // 
            // lblTotalValueWord
            // 
            lblTotalValueWord.AutoSize = true;
            lblTotalValueWord.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalValueWord.Location = new Point(88, 531);
            lblTotalValueWord.Name = "lblTotalValueWord";
            lblTotalValueWord.Size = new Size(64, 16);
            lblTotalValueWord.TabIndex = 53;
            lblTotalValueWord.Text = "Number";
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(40, 167, 69);
            btnSave.Location = new Point(485, 707);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 23);
            btnSave.TabIndex = 52;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnPrint
            // 
            btnPrint.BackColor = Color.FromArgb(0, 123, 255);
            btnPrint.Location = new Point(775, 707);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(100, 23);
            btnPrint.TabIndex = 51;
            btnPrint.Text = "Print";
            btnPrint.UseVisualStyleBackColor = false;
            btnPrint.Click += btnPrint_Click;
            // 
            // label45
            // 
            label45.AutoSize = true;
            label45.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label45.Location = new Point(0, 590);
            label45.Name = "label45";
            label45.Size = new Size(75, 16);
            label45.TabIndex = 50;
            label45.Text = "Remark : ";
            // 
            // label44
            // 
            label44.AutoSize = true;
            label44.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label44.Location = new Point(0, 574);
            label44.Name = "label44";
            label44.Size = new Size(309, 16);
            label44.TabIndex = 49;
            label44.Text = "Transporter Name : CodNow Technologies";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label24.Location = new Point(0, 550);
            label24.Name = "label24";
            label24.Size = new Size(133, 16);
            label24.TabIndex = 48;
            label24.Text = "Transport Mode : ";
            // 
            // label43
            // 
            label43.AutoSize = true;
            label43.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label43.Location = new Point(0, 531);
            label43.Name = "label43";
            label43.Size = new Size(85, 16);
            label43.TabIndex = 47;
            label43.Text = "In Words : ";
            // 
            // panel5
            // 
            panel5.BackColor = SystemColors.AppWorkspace;
            panel5.Controls.Add(tbxVN);
            panel5.Controls.Add(tbxDC);
            panel5.Controls.Add(tbxPON);
            panel5.Controls.Add(dtDC);
            panel5.Controls.Add(dtPOD);
            panel5.Controls.Add(DTds);
            panel5.Controls.Add(dtID);
            panel5.Controls.Add(lblID);
            panel5.Controls.Add(label23);
            panel5.Controls.Add(label22);
            panel5.Controls.Add(label21);
            panel5.Controls.Add(label20);
            panel5.Controls.Add(label19);
            panel5.Controls.Add(label18);
            panel5.Controls.Add(label17);
            panel5.Controls.Add(label16);
            panel5.Location = new Point(669, 74);
            panel5.Name = "panel5";
            panel5.Size = new Size(667, 180);
            panel5.TabIndex = 3;
            // 
            // tbxVN
            // 
            tbxVN.Location = new Point(222, 154);
            tbxVN.Name = "tbxVN";
            tbxVN.Size = new Size(200, 23);
            tbxVN.TabIndex = 17;
            // 
            // tbxDC
            // 
            tbxDC.Location = new Point(222, 108);
            tbxDC.Name = "tbxDC";
            tbxDC.Size = new Size(200, 23);
            tbxDC.TabIndex = 16;
            // 
            // tbxPON
            // 
            tbxPON.Location = new Point(222, 62);
            tbxPON.Name = "tbxPON";
            tbxPON.Size = new Size(200, 23);
            tbxPON.TabIndex = 15;
            // 
            // dtDC
            // 
            dtDC.Location = new Point(222, 131);
            dtDC.Name = "dtDC";
            dtDC.Size = new Size(200, 23);
            dtDC.TabIndex = 14;
            // 
            // dtPOD
            // 
            dtPOD.Location = new Point(222, 85);
            dtPOD.Name = "dtPOD";
            dtPOD.Size = new Size(200, 23);
            dtPOD.TabIndex = 13;
            // 
            // DTds
            // 
            DTds.Location = new Point(222, 39);
            DTds.Name = "DTds";
            DTds.Size = new Size(200, 23);
            DTds.TabIndex = 12;
            // 
            // dtID
            // 
            dtID.Location = new Point(222, 16);
            dtID.Name = "dtID";
            dtID.Size = new Size(200, 23);
            dtID.TabIndex = 11;
            // 
            // lblID
            // 
            lblID.AutoSize = true;
            lblID.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblID.Location = new Point(222, 1);
            lblID.Name = "lblID";
            lblID.Size = new Size(84, 16);
            lblID.TabIndex = 10;
            lblID.Text = "Invoice No";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label23.Location = new Point(124, 156);
            label23.Name = "label23";
            label23.Size = new Size(92, 16);
            label23.TabIndex = 9;
            label23.Text = "Vehicle No :";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label22.Location = new Point(142, 134);
            label22.Name = "label22";
            label22.Size = new Size(74, 16);
            label22.TabIndex = 8;
            label22.Text = "DC Date :";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label21.Location = new Point(157, 112);
            label21.Name = "label21";
            label21.Size = new Size(59, 16);
            label21.TabIndex = 7;
            label21.Text = "DC No :";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label20.Location = new Point(68, 89);
            label20.Name = "label20";
            label20.Size = new Size(148, 16);
            label20.TabIndex = 6;
            label20.Text = "Purchase Order Dt :";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label19.Location = new Point(65, 65);
            label19.Name = "label19";
            label19.Size = new Size(151, 16);
            label19.TabIndex = 5;
            label19.Text = "Purchase Order No :";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label18.Location = new Point(95, 41);
            label18.Name = "label18";
            label18.Size = new Size(121, 16);
            label18.TabIndex = 4;
            label18.Text = "Date of Supply :";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label17.Location = new Point(108, 21);
            label17.Name = "label17";
            label17.Size = new Size(108, 16);
            label17.TabIndex = 3;
            label17.Text = "Invoice Date :";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label16.Location = new Point(123, 1);
            label16.Name = "label16";
            label16.Size = new Size(93, 16);
            label16.TabIndex = 2;
            label16.Text = "Invoice No :";
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label42.Location = new Point(0, 469);
            label42.Name = "label42";
            label42.Size = new Size(351, 64);
            label42.TabIndex = 32;
            label42.Text = "                 BANK NAME  : IDBI BANK\r\n                   A/C NAME  :CODNOW TECHNOLOGIES\r\n                A/C NUMBER  :0076102000048736\r\n                  IFSC CODE  : IBKL0000076";
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.Font = new Font("Verdana", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label41.Location = new Point(0, 452);
            label41.Name = "label41";
            label41.Size = new Size(251, 18);
            label41.TabIndex = 31;
            label41.Text = "NEFT / RTGS BANK DETAILS:";
            label41.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label40.Location = new Point(1087, 543);
            label40.Name = "label40";
            label40.Size = new Size(88, 16);
            label40.TabIndex = 30;
            label40.Text = "Total Value";
            // 
            // label39
            // 
            label39.AutoSize = true;
            label39.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label39.Location = new Point(1079, 520);
            label39.Name = "label39";
            label39.Size = new Size(96, 16);
            label39.TabIndex = 29;
            label39.Text = "IGST @ 18%";
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label38.Location = new Point(1085, 497);
            label38.Name = "label38";
            label38.Size = new Size(90, 16);
            label38.TabIndex = 28;
            label38.Text = "SGST @ 9%";
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label37.Location = new Point(1084, 481);
            label37.Name = "label37";
            label37.Size = new Size(91, 16);
            label37.TabIndex = 27;
            label37.Text = "CGST @ 9%";
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label34.Location = new Point(1066, 458);
            label34.Name = "label34";
            label34.Size = new Size(109, 16);
            label34.TabIndex = 26;
            label34.Text = "Taxable Value";
            // 
            // tbxTotalValue
            // 
            tbxTotalValue.BorderStyle = BorderStyle.FixedSingle;
            tbxTotalValue.Location = new Point(1181, 543);
            tbxTotalValue.Name = "tbxTotalValue";
            tbxTotalValue.Size = new Size(155, 23);
            tbxTotalValue.TabIndex = 13;
            // 
            // tbxIGST
            // 
            tbxIGST.BorderStyle = BorderStyle.FixedSingle;
            tbxIGST.Location = new Point(1181, 520);
            tbxIGST.Name = "tbxIGST";
            tbxIGST.Size = new Size(155, 23);
            tbxIGST.TabIndex = 12;
            // 
            // tbxSGST
            // 
            tbxSGST.BorderStyle = BorderStyle.FixedSingle;
            tbxSGST.Location = new Point(1181, 497);
            tbxSGST.Name = "tbxSGST";
            tbxSGST.Size = new Size(155, 23);
            tbxSGST.TabIndex = 11;
            // 
            // tbxCGST
            // 
            tbxCGST.BorderStyle = BorderStyle.FixedSingle;
            tbxCGST.Location = new Point(1181, 474);
            tbxCGST.Name = "tbxCGST";
            tbxCGST.Size = new Size(155, 23);
            tbxCGST.TabIndex = 10;
            // 
            // txtTaxableValue
            // 
            txtTaxableValue.BorderStyle = BorderStyle.FixedSingle;
            txtTaxableValue.Location = new Point(1181, 451);
            txtTaxableValue.Name = "txtTaxableValue";
            txtTaxableValue.Size = new Size(155, 23);
            txtTaxableValue.TabIndex = 9;
            // 
            // dgvItems
            // 
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.Location = new Point(0, 370);
            dgvItems.Name = "dgvItems";
            dgvItems.RowHeadersVisible = false;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.Size = new Size(1336, 81);
            dgvItems.TabIndex = 8;
            // 
            // panel9
            // 
            panel9.BackColor = SystemColors.AppWorkspace;
            panel9.Controls.Add(lblSTGSTIN);
            panel9.Controls.Add(lblSTS);
            panel9.Controls.Add(lblSTSC);
            panel9.Controls.Add(lblSTAdd);
            panel9.Controls.Add(cbST);
            panel9.Controls.Add(label36);
            panel9.Controls.Add(label35);
            panel9.Controls.Add(label33);
            panel9.Controls.Add(label30);
            panel9.Location = new Point(669, 276);
            panel9.Name = "panel9";
            panel9.Size = new Size(667, 93);
            panel9.TabIndex = 7;
            // 
            // lblSTGSTIN
            // 
            lblSTGSTIN.AutoSize = true;
            lblSTGSTIN.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSTGSTIN.Location = new Point(93, 60);
            lblSTGSTIN.Name = "lblSTGSTIN";
            lblSTGSTIN.Size = new Size(89, 16);
            lblSTGSTIN.TabIndex = 39;
            lblSTGSTIN.Text = "Maharashtra";
            // 
            // lblSTS
            // 
            lblSTS.AutoSize = true;
            lblSTS.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSTS.Location = new Point(290, 43);
            lblSTS.Name = "lblSTS";
            lblSTS.Size = new Size(89, 16);
            lblSTS.TabIndex = 38;
            lblSTS.Text = "Maharashtra";
            // 
            // lblSTSC
            // 
            lblSTSC.AutoSize = true;
            lblSTSC.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSTSC.Location = new Point(130, 43);
            lblSTSC.Name = "lblSTSC";
            lblSTSC.Size = new Size(89, 16);
            lblSTSC.TabIndex = 37;
            lblSTSC.Text = "Maharashtra";
            // 
            // lblSTAdd
            // 
            lblSTAdd.AutoSize = true;
            lblSTAdd.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSTAdd.Location = new Point(106, 27);
            lblSTAdd.Name = "lblSTAdd";
            lblSTAdd.Size = new Size(89, 16);
            lblSTAdd.TabIndex = 30;
            lblSTAdd.Text = "Maharashtra";
            // 
            // cbST
            // 
            cbST.FormattingEnabled = true;
            cbST.Location = new Point(191, 0);
            cbST.Name = "cbST";
            cbST.Size = new Size(302, 23);
            cbST.TabIndex = 36;
            cbST.SelectedIndexChanged += cbST_SelectedIndexChanged;
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label36.Location = new Point(23, 27);
            label36.Name = "label36";
            label36.Size = new Size(79, 16);
            label36.TabIndex = 25;
            label36.Text = "Address : ";
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label35.Location = new Point(24, 60);
            label35.Name = "label35";
            label35.Size = new Size(63, 16);
            label35.TabIndex = 26;
            label35.Text = "GSTIN : ";
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label33.Location = new Point(225, 43);
            label33.Name = "label33";
            label33.Size = new Size(59, 16);
            label33.TabIndex = 28;
            label33.Text = "State : ";
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label30.Location = new Point(24, 43);
            label30.Name = "label30";
            label30.Size = new Size(100, 16);
            label30.TabIndex = 29;
            label30.Text = "State Code : ";
            // 
            // panel8
            // 
            panel8.BackColor = SystemColors.AppWorkspace;
            panel8.Controls.Add(lblITPT);
            panel8.Controls.Add(lblITGSTIN);
            panel8.Controls.Add(lblITS);
            panel8.Controls.Add(cbIT);
            panel8.Controls.Add(lblITSC);
            panel8.Controls.Add(lblITAdd);
            panel8.Controls.Add(label32);
            panel8.Controls.Add(label31);
            panel8.Controls.Add(label29);
            panel8.Controls.Add(label28);
            panel8.Controls.Add(label27);
            panel8.Location = new Point(0, 276);
            panel8.Name = "panel8";
            panel8.Size = new Size(667, 93);
            panel8.TabIndex = 6;
            // 
            // lblITPT
            // 
            lblITPT.AutoSize = true;
            lblITPT.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblITPT.Location = new Point(153, 74);
            lblITPT.Name = "lblITPT";
            lblITPT.Size = new Size(89, 16);
            lblITPT.TabIndex = 29;
            lblITPT.Text = "Maharashtra";
            // 
            // lblITGSTIN
            // 
            lblITGSTIN.AutoSize = true;
            lblITGSTIN.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblITGSTIN.Location = new Point(90, 58);
            lblITGSTIN.Name = "lblITGSTIN";
            lblITGSTIN.Size = new Size(89, 16);
            lblITGSTIN.TabIndex = 28;
            lblITGSTIN.Text = "Maharashtra";
            // 
            // lblITS
            // 
            lblITS.AutoSize = true;
            lblITS.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblITS.Location = new Point(287, 42);
            lblITS.Name = "lblITS";
            lblITS.Size = new Size(89, 16);
            lblITS.TabIndex = 27;
            lblITS.Text = "Maharashtra";
            // 
            // cbIT
            // 
            cbIT.FormattingEnabled = true;
            cbIT.Location = new Point(183, 5);
            cbIT.Name = "cbIT";
            cbIT.Size = new Size(302, 23);
            cbIT.TabIndex = 25;
            cbIT.SelectedIndexChanged += cbIT_SelectedIndexChanged_1;
            // 
            // lblITSC
            // 
            lblITSC.AutoSize = true;
            lblITSC.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblITSC.Location = new Point(127, 42);
            lblITSC.Name = "lblITSC";
            lblITSC.Size = new Size(89, 16);
            lblITSC.TabIndex = 26;
            lblITSC.Text = "Maharashtra";
            // 
            // lblITAdd
            // 
            lblITAdd.AutoSize = true;
            lblITAdd.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblITAdd.Location = new Point(98, 26);
            lblITAdd.Name = "lblITAdd";
            lblITAdd.Size = new Size(89, 16);
            lblITAdd.TabIndex = 14;
            lblITAdd.Text = "Maharashtra";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label32.Location = new Point(18, 42);
            label32.Name = "label32";
            label32.Size = new Size(100, 16);
            label32.TabIndex = 7;
            label32.Text = "State Code : ";
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label31.Location = new Point(222, 42);
            label31.Name = "label31";
            label31.Size = new Size(59, 16);
            label31.TabIndex = 6;
            label31.Text = "State : ";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label29.Location = new Point(18, 74);
            label29.Name = "label29";
            label29.Size = new Size(126, 16);
            label29.TabIndex = 4;
            label29.Text = "Payment Term : ";
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label28.Location = new Point(18, 59);
            label28.Name = "label28";
            label28.Size = new Size(63, 16);
            label28.TabIndex = 3;
            label28.Text = "GSTIN : ";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label27.Location = new Point(18, 26);
            label27.Name = "label27";
            label27.Size = new Size(79, 16);
            label27.TabIndex = 2;
            label27.Text = "Address : ";
            // 
            // panel7
            // 
            panel7.BackColor = SystemColors.AppWorkspace;
            panel7.Controls.Add(label26);
            panel7.Location = new Point(669, 255);
            panel7.Name = "panel7";
            panel7.Size = new Size(667, 20);
            panel7.TabIndex = 5;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label26.Location = new Point(281, 2);
            label26.Name = "label26";
            label26.Size = new Size(94, 16);
            label26.TabIndex = 1;
            label26.Text = " Shipping To";
            label26.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel6
            // 
            panel6.BackColor = SystemColors.AppWorkspace;
            panel6.Controls.Add(label25);
            panel6.Location = new Point(0, 255);
            panel6.Name = "panel6";
            panel6.Size = new Size(667, 20);
            panel6.TabIndex = 4;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label25.Location = new Point(287, 2);
            label25.Name = "label25";
            label25.Size = new Size(82, 16);
            label25.TabIndex = 1;
            label25.Text = "Invoice To";
            label25.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.AppWorkspace;
            panel4.Controls.Add(label15);
            panel4.Controls.Add(label14);
            panel4.Controls.Add(label13);
            panel4.Controls.Add(label12);
            panel4.Controls.Add(label11);
            panel4.Controls.Add(label10);
            panel4.Controls.Add(label9);
            panel4.Controls.Add(label8);
            panel4.Controls.Add(label7);
            panel4.Controls.Add(label6);
            panel4.Controls.Add(label5);
            panel4.Controls.Add(label4);
            panel4.Controls.Add(label3);
            panel4.Controls.Add(label2);
            panel4.Location = new Point(1, 74);
            panel4.Name = "panel4";
            panel4.Size = new Size(667, 180);
            panel4.TabIndex = 2;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label15.Location = new Point(314, 131);
            label15.Name = "label15";
            label15.Size = new Size(89, 16);
            label15.TabIndex = 13;
            label15.Text = "Maharashtra";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label14.Location = new Point(245, 131);
            label14.Name = "label14";
            label14.Size = new Size(63, 16);
            label14.TabIndex = 12;
            label14.Text = " State : ";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label13.Location = new Point(127, 131);
            label13.Name = "label13";
            label13.Size = new Size(23, 16);
            label13.TabIndex = 11;
            label13.Text = "27";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label12.Location = new Point(127, 108);
            label12.Name = "label12";
            label12.Size = new Size(211, 16);
            label12.TabIndex = 10;
            label12.Text = "www.codnowtechnologies.com";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label11.Location = new Point(127, 85);
            label11.Name = "label11";
            label11.Size = new Size(210, 16);
            label11.TabIndex = 9;
            label11.Text = "info@codnowtechnologies.com";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.Location = new Point(127, 62);
            label10.Name = "label10";
            label10.Size = new Size(263, 16);
            label10.TabIndex = 8;
            label10.Text = "+91 9923 8888 91 / +91 7972 3815 80";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(127, 17);
            label9.Name = "label9";
            label9.Size = new Size(356, 48);
            label9.TabIndex = 7;
            label9.Text = " Plot No.32, Gut No.83, Renuka Nagar, Opp. Vinayak \r\nPark, Deolai Road, Chhatrapati, Sambhajinagar \r\n- 431 001";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(18, 154);
            label8.Name = "label8";
            label8.Size = new Size(389, 16);
            label8.TabIndex = 6;
            label8.Text = "GSTIN : 27FAEPS7663Q1ZK ,           PAN : FAEPS7663Q";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(18, 131);
            label7.Name = "label7";
            label7.Size = new Size(100, 16);
            label7.TabIndex = 5;
            label7.Text = "State Code : ";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(18, 108);
            label6.Name = "label6";
            label6.Size = new Size(79, 16);
            label6.TabIndex = 4;
            label6.Text = "Website : ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(18, 88);
            label5.Name = "label5";
            label5.Size = new Size(63, 16);
            label5.TabIndex = 3;
            label5.Text = "E-mail :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(18, 61);
            label4.Name = "label4";
            label4.Size = new Size(65, 16);
            label4.TabIndex = 2;
            label4.Text = "Phone : ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(18, 17);
            label3.Name = "label3";
            label3.Size = new Size(75, 16);
            label3.TabIndex = 1;
            label3.Text = "Address :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(127, 1);
            label2.Name = "label2";
            label2.Size = new Size(199, 16);
            label2.TabIndex = 0;
            label2.Text = "M/S CodNow Technologies";
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ActiveCaption;
            panel3.Controls.Add(label1);
            panel3.Location = new Point(0, 51);
            panel3.Name = "panel3";
            panel3.Size = new Size(1336, 22);
            panel3.TabIndex = 1;
            panel3.Tag = "iuu";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Courier New", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(616, 0);
            label1.Name = "label1";
            label1.Size = new Size(131, 21);
            label1.TabIndex = 0;
            label1.Text = "TAX INVOICE";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.AppWorkspace;
            panel2.Controls.Add(pictureBox2);
            panel2.Controls.Add(pictureBox1);
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(1336, 51);
            panel2.TabIndex = 0;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(465, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(399, 50);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(37, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(84, 50);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // sqlCommand2
            // 
            sqlCommand2.CommandTimeout = 30;
            sqlCommand2.EnableOptimizedParameterBinding = false;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.RosyBrown;
            imageList1.Images.SetKeyName(0, "Signature.png");
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1370, 749);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load_1;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel11.ResumeLayout(false);
            panel11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private Panel panel1;
        private Label label42;
        private Label label41;
        private Label label40;
        private Label label39;
        private Label label38;
        private Label label37;
        private Label label34;
        private TextBox tbxTotalValue;
        private TextBox tbxIGST;
        private TextBox tbxSGST;
        private TextBox tbxCGST;
        private TextBox txtTaxableValue;
        private DataGridView dgvItems;
        private Panel panel9;
        private Label lblSTGSTIN;
        private Label lblSTS;
        private Label lblSTSC;
        private Label lblSTAdd;
        private ComboBox cbST;
        private Label label36;
        private Label label35;
        private Label label33;
        private Label label30;
        private Panel panel8;
        private Label lblITPT;
        private Label lblITGSTIN;
        private Label lblITS;
        private Label lblITSC;
        private Label lblITAdd;
        private ComboBox cbIT;
        private Label label32;
        private Label label31;
        private Label label29;
        private Label label28;
        private Label label27;
        private Panel panel7;
        private Label label26;
        private Panel panel6;
        private Label label25;
        private Panel panel5;
        private TextBox tbxVN;
        private TextBox tbxDC;
        private TextBox tbxPON;
        private DateTimePicker dtDC;
        private DateTimePicker dtPOD;
        private DateTimePicker DTds;
        private DateTimePicker dtID;
        private Label lblID;
        private Label label23;
        private Label label22;
        private Label label21;
        private Label label20;
        private Label label19;
        private Label label18;
        private Label label17;
        private Label label16;
        private Panel panel4;
        private Label label15;
        private Label label14;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Panel panel3;
        private Label label1;
        private Panel panel2;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand2;
        private ImageList imageList1;
        private Label label45;
        private Label label44;
        private Label label24;
        private Label label43;
        private ComboBox cbTransportMode;
        private TextBox tbxRemark;
        private Label lblTotalValueWord;
        private Button btnSave;
        private Button btnPrint;
        private Panel panel11;
        private PictureBox pictureBox3;
        private Label label50;
        private Label label49;
        private Panel panel10;
        private Label label48;
    }
}
