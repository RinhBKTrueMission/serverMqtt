using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Mvc;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Device
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StyleSheetCollection.Include(Environment.CurrentDirectory + "/contents/default.json");

            MyButtonBase.UrlProcess = (url) => Engine.Execute(url);

            var broswer = new Window();
            broswer.Closing += (s, ev) => AsyncEngine.Execute("Home/Exit");
            AsyncEngine.Start(this, res => {
                var view = res.View;
                broswer.Content = view.Content;         
            });
            broswer.Show();
        }
    }
}