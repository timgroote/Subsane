using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubSane.ConsoleForms;

namespace SubSane.SubSaneConsoleForms
{
    public class LoginForm : ConsoleForm
    {
        public LoginForm(int width, int height, string name) : base(width, height)
        {
            Name = name;

            Label lblTitle = new Label("lblTitle",
                new Point(1, 2),
                10,
                "Login",
                ConsoleColor.Green,
                ConsoleColor.Black);

            Label lblserver = new Label("lblserver",
                new Point(1, 10),
                28,
                "server",
                ConsoleColor.Green,
                ConsoleColor.Black);

            Label lblUsername = new Label("lbluname",
                new Point(1, 12),
                10,
                "Username",
                ConsoleColor.Green,
                ConsoleColor.Black);

            Label lblPass = new Label("lblpass",
                new Point(1, 14),
                10,
                "Password",
                ConsoleColor.Green,
                ConsoleColor.Black);


            Textbox txtServer = new Textbox("txtServer",
                new Point(12, 10),
                30,
                string.Empty,
                ConsoleColor.White,
                ConsoleColor.DarkGray);

            Textbox txtUser = new Textbox("txtUser",
                new Point(12, 12),
                30,
                string.Empty,
                ConsoleColor.White,
                ConsoleColor.DarkGray);

            Textbox txtPassword = new Textbox("txtPass",
                new Point(12, 14),
                30,
                string.Empty,
                ConsoleColor.White,
                ConsoleColor.DarkGray)
            {
                PasswordChar = '*'
            };
            
            Add(lblTitle);

            Add(lblTitle);
            Add(lblserver);
            Add(lblUsername);
            Add(lblPass);

            Add(txtServer);
            Add(txtUser);
            Add(txtPassword);
        }
    }
}
