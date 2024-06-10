using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProjectDB.User
{
    public partial class SignUp : System.Web.UI.Page
    {
        int GeneratedOTP;
        SqlConnection con;
        SqlCommand cmd;
        string str = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            ErrEmail.Visible = ErrConfirmPassword.Visible = ErrPassword.Visible = ErrPhone.Visible = ErrUsername.Visible = false;
            try
            {
                string userEmail = Email.Text.Trim();
                string password = Password.Text.Trim();
                string confirmPass = ConfirmPassword.Text.Trim();
                string inputUsername = username.Text.Trim();
                string address = Address.Text.Trim();
                string phone = Phone.Text.Trim();

                con = new SqlConnection(str);

                string Query = "Select Count(*) from Users where Username = @Username";

                cmd = new SqlCommand(Query, con);
                cmd.Parameters.AddWithValue("@Username", inputUsername);

                con.Open();

                int Count = (int)cmd.ExecuteScalar();

                if (Count > 0)
                {
                    con.Close();
                    ErrUsername.Visible = true;
                    ErrUsername.Text = "Username already Exists! Try Using " + distinctUsername(inputUsername);
                    username.Text = String.Empty;
                    return;
                }

                con = new SqlConnection(str);

                Query = "Select Count(*) from Users where Email = @Email";

                cmd = new SqlCommand(Query, con);

                cmd.Parameters.AddWithValue("@Email", userEmail);
                con.Open();

                Count = (int)cmd.ExecuteScalar();

                if (Count > 0)
                {
                    con.Close();
                    ErrEmail.Visible = true;
                    ErrEmail.Text = "Email already Registered!";
                    return;
                }
                else if (confirmPass != password)
                {
                    ErrConfirmPassword.Visible = ErrPassword.Visible = true;
                    ErrConfirmPassword.Text = ErrPassword.Text = "Passwords do not match!";
                }
                else if (password.Length < 6)
                {
                    ErrPassword.Visible = true;
                    ErrPassword.Text = "Password must be at least 6 characters long!";
                }
                else if (phone.Length != 11)
                {
                    ErrPhone.Visible = true;
                    ErrPhone.Text = "Invalid Phone Number!";
                }
                else if (confirmPass == password)
                {
                    Random Generate = new Random();
                    GeneratedOTP = Generate.Next(100000, 1000000);

                    Session["Username"] = inputUsername;
                    Session["Email"] = userEmail;
                    Session["Password"] = password;
                    Session["Address"] = address;
                    Session["Phone"] = phone;
                    Session["OTP"] = GeneratedOTP;

                    GenerateOTP(GeneratedOTP, inputUsername, userEmail);
                    con.Close();
                    Response.Redirect("OTPCheck.aspx", false);
                    return;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
        }

        private void GenerateOTP(int OTP, string username, string toEmail)
        {
            string fromMail = "registrarhirex@gmail.com";
            string fromPassword = "fvdmnukxjnnfkeim";
          

            MailMessage message = new MailMessage();

            message.From = new MailAddress(fromMail);
            message.Subject = "Your OTP";
            message.To.Add(new MailAddress(toEmail));
            message.Body = "Dear, " + username + "\nYour OTP: " + OTP.ToString();

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(message);
            return;
        }





        private void ClearFields()
        {
            Email.Text = String.Empty;
            ConfirmPassword.Text = String.Empty;
            Phone.Text = String.Empty;
            Address.Text = String.Empty;
            username.Text = String.Empty;
            Password.Text = String.Empty;
        }

        //private bool checkEmail(string inputEmail)
        //{
        //    int flags = 0;
        //    foreach (char i in inputEmail)
        //        if (i == '@')
        //            flags = 1;

        //    if (inputEmail.Contains(".com") && flags == 1)
        //        return true;
        //    return false;
        //}

        protected string distinctUsername(string inputUsername)
        {
            con = new SqlConnection(str);
            string Query = "Select Count(*) from Users where Username = @Username";

            int Count = 1;
            while (Count > 0)
            {
                Random randomGen = new Random();
                int j = randomGen.Next(100);
                cmd = new SqlCommand(Query, con);
                cmd.Parameters.AddWithValue("@Username", inputUsername);

                con.Open();
                Count = (int)cmd.ExecuteScalar();

                if (Count > 0)
                {
                    inputUsername += j;
                }
                con.Close();
            }
            return inputUsername;
        }
    }
}