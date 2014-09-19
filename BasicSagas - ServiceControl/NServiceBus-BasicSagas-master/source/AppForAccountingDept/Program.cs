using System;
using System.ComponentModel;
using System.Windows.Forms;
using AppCommon;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NServiceBus;
using NServiceBus.Installation.Environments;
using NLog;

namespace AppForAccountingDept
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

            // Log the Bus
            SetLoggingLibrary.NLog();
            
            var container = new UnityContainer();
            var locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            logger.Info("--------AppForAccountingDept IBus Config Start--------");
  

            Configure.With()
                .UnityBuilder(container)
                .UseTransport<Msmq>()
                .DisableTimeoutManager()
                .UnicastBus()
                .CreateBus()
                .Start(StartupAction);

            logger.Info("--------AppForAccountingDept IBus Config End--------");

            var items = new BindingList<ItemViewModel>();

            var appForm = new MainForm(items);
            var context = new Context<ItemViewModel> { Items = items, AppForm = appForm };
            container.RegisterInstance(context);

            Application.EnableVisualStyles();
            Application.Run(context.AppForm);
        }
    }
}
