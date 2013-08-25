﻿using System;
using Programe.Gui;
using Programe.Gui.Widgets;

namespace Programe
{
    // TODO: clean this awful class
    static class Interface
    {
        private static GuiSystem gui;

        private static Window messageWindow;
        private static Label messageWindowLabel;

        private static Window loginWindow;
        private static TextBox loginUsername;
        private static TextBox loginPassword;

        private static Window registerWindow;
        private static TextBox registerUsername;
        private static TextBox registerPassword1;
        private static TextBox registerPassword2;

        private static ListBox debugWindowList;

        public static void Start(GuiSystem guiSystem)
        {
            gui = guiSystem;

            var desktop = new Container(gui.SurfaceWidth, gui.SurfaceHeight - 1);
            desktop.Left = 0;
            desktop.Top = 1;
            gui.Add(desktop);

            #region Message Window
            messageWindow = new Window(0, 0, 60, 8, "");
            messageWindow.Visible = false;
            desktop.Add(messageWindow);

            messageWindowLabel = new Label(1, 1, 56, 2, "");
            messageWindow.Add(messageWindowLabel);

            var messageWindowButton = new Button(52, 4, 5, "OK");
            messageWindowButton.Clicked += () => messageWindow.Visible = false;
            messageWindow.Add(messageWindowButton);
            #endregion

            #region Debug Messages Window
            var debugWindow = new Window(10, 10, 70, 20, "Debug Messages");
            debugWindow.Visible = false;
            desktop.Add(debugWindow);

            debugWindowList = new ListBox(1, 0, 66, 18);
            debugWindowList.SelectEnabled = false;
            debugWindow.Add(debugWindowList);
            #endregion

            #region Login Window
            loginWindow = new Window(10, 10, 35, 11, "Login");
            loginWindow.Visible = false;
            desktop.Add(loginWindow);

            var loginUsernameLabel = new Label(1, 1, 31, 1, "Username:");
            loginWindow.Add(loginUsernameLabel);

            loginUsername = new TextBox(1, 2, 31);
            loginWindow.Add(loginUsername);

            var loginPasswordLabel = new Label(1, 4, 31, 1, "Password:");
            loginWindow.Add(loginPasswordLabel);

            loginPassword = new TextBox(1, 5, 31);
            loginPassword.PasswordCharacter = '*';
            loginWindow.Add(loginPassword);

            var loginButton = new Button(24, 7, 8, "Login");
            loginWindow.Add(loginButton);

            loginButton.Clicked += () =>
            {
                if (Offline())
                    return;

                Client.Login(loginUsername.Value, loginPassword.Value);
            };
            #endregion

            #region Register Window
            registerWindow = new Window(10, 10, 35, 14, "Register");
            registerWindow.Visible = false;
            desktop.Add(registerWindow);

            var registerUsernameLabel = new Label(1, 1, 31, 1, "Username:");
            registerWindow.Add(registerUsernameLabel);

            registerUsername = new TextBox(1, 2, 31);
            registerWindow.Add(registerUsername);

            var registerPassword1Label = new Label(1, 4, 31, 1, "Password:");
            registerWindow.Add(registerPassword1Label);

            registerPassword1 = new TextBox(1, 5, 31);
            registerPassword1.PasswordCharacter = '*';
            registerWindow.Add(registerPassword1);

            var registerPassword2Label = new Label(1, 7, 31, 1, "Confirm Password:");
            registerWindow.Add(registerPassword2Label);

            registerPassword2 = new TextBox(1, 8, 31);
            registerPassword2.PasswordCharacter = '*';
            registerWindow.Add(registerPassword2);

            var registerButton = new Button(21, 10, 11, "Register");
            registerWindow.Add(registerButton);

            registerButton.Clicked += () =>
            {
                if (Offline())
                    return;

                if (registerPassword1.Value != registerPassword2.Value)
                {
                    ShowMessage("Register", "The provided passwords do not match.");
                    return;
                }

                Client.Register(registerUsername.Value, registerPassword1.Value);
            };
            #endregion

            var menu = new MenuBar();
            gui.Add(menu);

            #region Account Menu
            var account = new MenuItem("Account");
            var login = new MenuItem("Login");
            var register = new MenuItem("Register");
            account.Items.Add(login);
            account.Items.Add(register);
            menu.Items.Add(account);

            login.Clicked += () =>
            {
                if (Offline())
                    return;

                ResetAccountWindows();
                loginWindow.Visible = true;
                loginWindow.Focus();
            };

            register.Clicked += () =>
            {
                if (Offline())
                    return;

                ResetAccountWindows();
                registerWindow.Visible = true;
                registerWindow.Focus();
            };
            #endregion

            #region Ship Menu
            var ship = new MenuItem("Ship");
            var log = new MenuItem("Event Log");
            var upload = new MenuItem("Upload");
            ship.Items.Add(log);
            ship.Items.Add(upload);
            menu.Items.Add(ship);
            #endregion

            #region Debug Messages Menu
#if DEBUG
            var debug = new MenuItem("Debug Messages");
            menu.Items.Add(debug);

            debug.Clicked += () =>
            {
                debugWindow.Visible = true;
                debugWindow.Focus();
            };
#endif
            #endregion
        }

        public static void Connected()
        {
            ResetAccountWindows();
        }

        public static void Disconnected()
        {
            ResetAccountWindows();
        }

        public static void ShowMessage(string caption, string message)
        {
            messageWindow.Caption = caption;
            messageWindowLabel.Caption = message;

            messageWindow.Focus();
            CenterWindow(messageWindow);
            messageWindow.Visible = true;
        }

        public static void AddStatusMessage(string message)
        {
            if (debugWindowList.Items.Count > 150)
                debugWindowList.Items.RemoveAt(debugWindowList.Items.Count - 1);
            debugWindowList.Items.Insert(0, new ListBoxItem(message));
        }

        public static void ResetAccountWindows()
        {
            loginUsername.Value = "";
            loginPassword.Value = "";
            loginWindow.Visible = false;
            CenterWindow(loginWindow);

            registerUsername.Value = "";
            registerPassword1.Value = "";
            registerPassword2.Value = "";
            registerWindow.Visible = false;
            CenterWindow(registerWindow);
        }

        private static void CenterWindow(Window window)
        {
            var halfWidth = (int)gui.SurfaceWidth / 2;
            var halfHeight = (int)gui.SurfaceHeight / 2;
            window.Left = (int)(halfWidth - (window.Width / 2));
            window.Top = (int)(halfHeight - (window.Height / 2));
        }

        private static bool Offline()
        {
            if (!Client.Connected)
            {
                ShowMessage("", "The game is not connected to a server.");
                return true;
            }

            if (Client.LoggedIn)
            {
                ShowMessage("", "You are already logged in.");
                return true;
            }

            return false;
        }
    }
}
