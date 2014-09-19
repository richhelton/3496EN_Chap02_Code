using System;
using System.ComponentModel;
using System.Windows.Forms;
using AppCommon;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NServiceBus;
using NServiceBus.Installation.Environments;
using NLog;

namespace AppForApproversLevel2
{
    static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        
        static void StartupAction()
        {
            Configure.Instance.ForInstallationOn<Windows>().Install();
        }

        [STAThread]
        static void Main()
        {
            logger.Info("--------AppForApproversLevel2 Unity Config--------");
            var container = new UnityContainer();
            var locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            logger.Info("--------AppForApproversLevel2 IBus Config--------");
            Configure.With()
                .UnityBuilder(container)
                .UseTransport<Msmq>()
                .DisableTimeoutManager()
                .UnicastBus()
                .CreateBus()
                .Start(StartupAction);

            var items = new BindingList<ItemViewModel>();

            System.Console.WriteLine("AppForApproversLevel2");
            foreach (var item in items)
            {
                System.Console.WriteLine(item.ToString());
            }

            var appForm = new MainForm(items);
            var context = new Context<ItemViewModel> { Items = items, AppForm = appForm };
            container.RegisterInstance(context);

            Application.EnableVisualStyles();
            Application.Run(context.AppForm);
        }
    }
}
