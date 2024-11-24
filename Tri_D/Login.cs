using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Engines;
using System.Data.Common;
using Tri_D;

//Test Push - Jim Maido awdawdawdawdawd

namespace Tri_D
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
      
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string username = userTextbox.Text;
            string password = passwordTextbox.Text;

            MySqlConnection connection = connectionDB.GetConnection();

            string query = "SELECT COUNT(*) FROM `account` WHERE `username` = @username AND `password` = @password";
            MySqlCommand commandSql = new MySqlCommand(query, connection);

            commandSql.Parameters.AddWithValue("@username", username);
            commandSql.Parameters.AddWithValue("@password", password);

            int count = Convert.ToInt32(commandSql.ExecuteScalar());
            if (count > 0)
            {
                // Successful login, proceed to the dashboard
                Dashboard dashboard = new Dashboard();
                dashboard.Show();
                /*Fullhistory fullhistory = new Fullhistory();
                fullhistory.Show();*/
                this.Hide(); // Hide the login form
            }
            else
            {
                // Login failed
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

        private void passwordTextbox_DoubleClick(object sender, EventArgs e)
        {
           
        }

        private void Login_Enter(object sender, EventArgs e)
        {

        }

        private void passwordTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void passwordTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                loginButton.PerformClick();
                // these last two lines will stop the beep sound
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
    }
}
