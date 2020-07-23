using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Plugins;
using OpenMod.API.Plugins;

[assembly: PluginMetadata("Pustalorc.GlobalBan", Author = "Pustalorc", DisplayName = "Global Ban", Website = "https://github.com/Pustalorc/GlobalBan/")]
namespace MyOpenModPlugin
{
    public class MyOpenModPlugin : OpenModUniversalPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<MyOpenModPlugin> m_Logger;

        public MyOpenModPlugin(
            IConfiguration configuration, 
            IStringLocalizer stringLocalizer,
            ILogger<MyOpenModPlugin> logger, 
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
        }

        protected override Task OnLoadAsync()
        {
            m_Logger.LogInformation(m_StringLocalizer["plugin_events:plugin_start"]);
            return Task.CompletedTask;
        }

        protected override Task OnUnloadAsync()
        {
            m_Logger.LogInformation(m_StringLocalizer["plugin_events:plugin_stop"]);
            return Task.CompletedTask;
        }
    }
}
