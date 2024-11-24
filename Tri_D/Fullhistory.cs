using LiveChartsCore.Themes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Tri_D.History;

//Test Push - Jim

namespace Tri_D
{
    public partial class Fullhistory : Form
    {
        Bitmap MemoryImage;
        private PrintDocument printDocument1 = new PrintDocument();
        private PrintPreviewDialog previewdlg = new PrintPreviewDialog();

        // Define the scaling factor to adjust the fit
        private float scaleFactor;

        public Fullhistory()
        {
            InitializeComponent();
            printDocument1.PrintPage += new PrintPageEventHandler(printdoc1_PrintPage);
        }

        // This method captures the panel to print
        public void GetPrintArea(Panel pnl)
        {
            MemoryImage = new Bitmap(pnl.Width, pnl.Height);
            pnl.DrawToBitmap(MemoryImage, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }

        // Override OnPaint to draw the captured image of the panel
        protected override void OnPaint(PaintEventArgs e)
        {
            if (MemoryImage != null)
            {
                e.Graphics.DrawImage(MemoryImage, 0, 0);
                base.OnPaint(e);
            }
        }

        // Adjust print page settings and calculate scale factor
        void printdoc1_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Get the page area (this will give the printable area of the page)
            Rectangle pagearea = e.PageBounds;

            // Scale the image to fit within the printable area
            float scaleWidth = pagearea.Width / (float)fulltbl.Width;
            float scaleHeight = pagearea.Height / (float)fulltbl.Height;
            scaleFactor = Math.Min(scaleWidth, scaleHeight);  // Ensure the table fits both width and height

            // Draw the image aligned to the top of the page instead of the center
            e.Graphics.DrawImage(MemoryImage,
                (pagearea.Width - fulltbl.Width * scaleFactor) / 2,  // Horizontally center the image
                0,  // Align to the top of the page (Y = 0)
                fulltbl.Width * scaleFactor,
                fulltbl.Height * scaleFactor);
        }

        // Method to print the panel (in this case, the fulltbl)
        public void Print(Panel pnl)
        {
            GetPrintArea(pnl);  // Capture the table image
            previewdlg.Document = printDocument1;  // Set the print document
            previewdlg.ShowDialog();  // Show the print preview
        }

        private void Fullhistory_Load(object sender, EventArgs e)
        {
            MySqlConnection connection = connectionDB.GetConnection();

            ownerID.Text = OwnerDetails.OwnerID;
            fullnameResultlbl.Text = OwnerDetails.OwnerName;
            typeResultlbl.Text = OwnerDetails.OwnerType;
            string queryHistory = @"SELECT v.type, h.plate_number, h.date, h.time_in AS timein, h.time_out AS timeout, h.duration, h.reason, h.slot_number, h.admin_name AS duty FROM history h LEFT JOIN vehicles v ON h.plate_number = v.plate_number LEFT JOIN students s ON h.owner_id = s.student_number LEFT JOIN employees e ON h.owner_id = e.employee_number WHERE h.owner_id = @OwnerID;";

            using (MySqlCommand cmd = new MySqlCommand(queryHistory, connection))
            {
                cmd.Parameters.AddWithValue("@OwnerID", ownerID.Text);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DateTime date = Convert.ToDateTime(reader["date"]);
                    TimeSpan timeIn = (TimeSpan)reader["timein"];

                    // Handle NULL time_out
                    TimeSpan? timeOut = reader["timeout"] != DBNull.Value ? (TimeSpan?)reader["timeout"] : null;
                    string duration = reader["duration"] != DBNull.Value ? reader["duration"].ToString() : "N/A";

                    // Add row to DataGridView (fulltbl)
                    fullHistoryTable.Rows.Add(
                        reader["type"].ToString(),
                        reader["plate_number"].ToString(),
                        date.ToString("yyyy-MM-dd"),
                        timeIn.ToString(@"hh\:mm"),
                        timeOut.HasValue ? timeOut.Value.ToString(@"hh\:mm") : "N/A",
                        duration,
                        reader["reason"].ToString(),
                        reader["slot_number"].ToString(),
                        reader["duty"].ToString()
                    );
                }

                reader.Close();
            }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        // Button click to go back to dashboard
        private void menuButton_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }

        // Button click to print the DataGridView
        private void printBtn_Click(object sender, EventArgs e)
        {
            printBtn.Visible = false;  // Hide the print button during the print process
            Print(this.fulltbl);  // Print the full table
        }

        // When the table (fulltbl) is clicked, print the table
        private void fulltbl_Click(object sender, EventArgs e)
        {
            Print(this.fulltbl);  // Print when the DataGridView is clicked
        }

        // Alternative button click for printing
        private void printButton_Click(object sender, EventArgs e)
        {
            Print(this.fulltbl);  // Print the full table
        }
    }
}
