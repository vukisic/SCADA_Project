using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Core.Common.ServiceBus;
using GUI.ViewModels;
using NServiceBus;

namespace GUI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        public App()
        {
            var endpointConfiguration = new EndpointConfiguration(EndpointNames.GUI);

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
	}
}
