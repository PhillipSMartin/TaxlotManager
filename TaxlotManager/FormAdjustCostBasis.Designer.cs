namespace TaxlotManager
{
    partial class FormAdjustCostBasis
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
            this.textBoxNewCostBasis = new System.Windows.Forms.TextBox();
            this.dateTimePickerAdjustmentDate = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOldCostBasis = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonCurrentCostBasis = new System.Windows.Forms.RadioButton();
            this.radioButtonNumberOfShares = new System.Windows.Forms.RadioButton();
            this.textBoxTradeNote = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxNewCostBasis
            // 
            this.textBoxNewCostBasis.Location = new System.Drawing.Point(114, 48);
            this.textBoxNewCostBasis.Name = "textBoxNewCostBasis";
            this.textBoxNewCostBasis.Size = new System.Drawing.Size(200, 20);
            this.textBoxNewCostBasis.TabIndex = 0;
            this.textBoxNewCostBasis.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // dateTimePickerAdjustmentDate
            // 
            this.dateTimePickerAdjustmentDate.Location = new System.Drawing.Point(114, 85);
            this.dateTimePickerAdjustmentDate.Name = "dateTimePickerAdjustmentDate";
            this.dateTimePickerAdjustmentDate.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerAdjustmentDate.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(255, 324);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(379, 324);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "New Cost Basis:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Date of Adjustment:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Old Cost Basis:";
            // 
            // textBoxOldCostBasis
            // 
            this.textBoxOldCostBasis.Enabled = false;
            this.textBoxOldCostBasis.Location = new System.Drawing.Point(114, 14);
            this.textBoxOldCostBasis.Name = "textBoxOldCostBasis";
            this.textBoxOldCostBasis.Size = new System.Drawing.Size(200, 20);
            this.textBoxOldCostBasis.TabIndex = 7;
            this.textBoxOldCostBasis.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(142, 280);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(424, 30);
            this.label4.TabIndex = 8;
            this.label4.Text = "N.B.: New cost basis will not be reflected until the day after the adjustment dat" +
                "e.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonCurrentCostBasis);
            this.groupBox1.Controls.Add(this.radioButtonNumberOfShares);
            this.groupBox1.Location = new System.Drawing.Point(114, 169);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 87);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Apportion adjustment by";
            // 
            // radioButtonCurrentCostBasis
            // 
            this.radioButtonCurrentCostBasis.AutoSize = true;
            this.radioButtonCurrentCostBasis.Location = new System.Drawing.Point(26, 49);
            this.radioButtonCurrentCostBasis.Name = "radioButtonCurrentCostBasis";
            this.radioButtonCurrentCostBasis.Size = new System.Drawing.Size(109, 17);
            this.radioButtonCurrentCostBasis.TabIndex = 1;
            this.radioButtonCurrentCostBasis.Text = "Current cost basis";
            this.radioButtonCurrentCostBasis.UseVisualStyleBackColor = true;
            // 
            // radioButtonNumberOfShares
            // 
            this.radioButtonNumberOfShares.AutoSize = true;
            this.radioButtonNumberOfShares.Checked = true;
            this.radioButtonNumberOfShares.Location = new System.Drawing.Point(26, 26);
            this.radioButtonNumberOfShares.Name = "radioButtonNumberOfShares";
            this.radioButtonNumberOfShares.Size = new System.Drawing.Size(108, 17);
            this.radioButtonNumberOfShares.TabIndex = 0;
            this.radioButtonNumberOfShares.TabStop = true;
            this.radioButtonNumberOfShares.Text = "Number of shares";
            this.radioButtonNumberOfShares.UseVisualStyleBackColor = true;
            // 
            // textBoxTradeNote
            // 
            this.textBoxTradeNote.Location = new System.Drawing.Point(114, 126);
            this.textBoxTradeNote.Name = "textBoxTradeNote";
            this.textBoxTradeNote.Size = new System.Drawing.Size(557, 20);
            this.textBoxTradeNote.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(79, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Note:";
            // 
            // FormAdjustCostBasis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 368);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxTradeNote);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxOldCostBasis);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePickerAdjustmentDate);
            this.Controls.Add(this.textBoxNewCostBasis);
            this.Name = "FormAdjustCostBasis";
            this.Text = "Adjust Cost Basis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAdjustCostBasis_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxNewCostBasis;
        private System.Windows.Forms.DateTimePicker dateTimePickerAdjustmentDate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxOldCostBasis;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonCurrentCostBasis;
        private System.Windows.Forms.RadioButton radioButtonNumberOfShares;
        private System.Windows.Forms.TextBox textBoxTradeNote;
        private System.Windows.Forms.Label label5;
    }
}