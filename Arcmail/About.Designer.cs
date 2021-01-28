namespace Arcmail
{
    partial class About
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxFocus = new System.Windows.Forms.TextBox();
            this.textBoxContributors = new System.Windows.Forms.TextBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(94, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "About Program";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(71, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Developed by: Murat Kurbanov";
            // 
            // textBoxFocus
            // 
            this.textBoxFocus.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxFocus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxFocus.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBoxFocus.Font = new System.Drawing.Font("Arial", 1.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxFocus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBoxFocus.Location = new System.Drawing.Point(12, -30);
            this.textBoxFocus.Multiline = true;
            this.textBoxFocus.Name = "textBoxFocus";
            this.textBoxFocus.ReadOnly = true;
            this.textBoxFocus.Size = new System.Drawing.Size(257, 36);
            this.textBoxFocus.TabIndex = 1;
            this.textBoxFocus.Text = " ";
            this.textBoxFocus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxContributors
            // 
            this.textBoxContributors.AcceptsReturn = true;
            this.textBoxContributors.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxContributors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxContributors.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBoxContributors.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxContributors.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBoxContributors.Location = new System.Drawing.Point(15, 94);
            this.textBoxContributors.Multiline = true;
            this.textBoxContributors.Name = "textBoxContributors";
            this.textBoxContributors.ReadOnly = true;
            this.textBoxContributors.Size = new System.Drawing.Size(257, 81);
            this.textBoxContributors.TabIndex = 3;
            this.textBoxContributors.Text = "The program is developed with help of: \r\nDöwlet Kömekow\r\nWepa Kulhanow\r\nUmyt Mere" +
    "dow\r\nNazar Seýitmyradow";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDescription.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBoxDescription.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxDescription.ForeColor = System.Drawing.SystemColors.ControlText;
            this.textBoxDescription.Location = new System.Drawing.Point(15, 43);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.Size = new System.Drawing.Size(257, 36);
            this.textBoxDescription.TabIndex = 4;
            this.textBoxDescription.Text = "ArcMail Helper is developed for Processing Center of the Halkbank in 2021.";
            this.textBoxDescription.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 230);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxContributors);
            this.Controls.Add(this.textBoxFocus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "About";
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFocus;
        private System.Windows.Forms.TextBox textBoxContributors;
        private System.Windows.Forms.TextBox textBoxDescription;
    }
}