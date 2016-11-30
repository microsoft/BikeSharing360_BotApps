using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


public static string GetEnv(string name)
{
    return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}


private static string GetStateApi() 
{
  var result = GetEnv("BotStateEndpoint");
  if (String.IsNullOrEmpty(result)) {
    result = "https://state.botframework.com/";
  }
  return result;
}
 
private static Lazy<string> stateApi = new Lazy<string>(GetStateApi);

public static async Task PostInScratchAsync(this Activity activity, Func<IDialog<object>> makeDialog)
{
    var builder = new ContainerBuilder();
    builder.RegisterModule(new DialogModule_MakeRoot());

    builder.Register(c =>
    {
        if (activity.ChannelId == "emulator")
        {
            // for emulator we should use serviceUri of the emulator for storage
            MicrosoftAppCredentials.TrustServiceUrl(activity.ServiceUrl);
            return new StateClient(new Uri(activity.ServiceUrl));
        }
        else
        {
            // TODO: remove this when going to against production
            MicrosoftAppCredentials.TrustServiceUrl(stateApi.Value);
            MicrosoftAppCredentials.TrustServiceUrl(activity.ServiceUrl);
            return new StateClient(new Uri(stateApi.Value));
        }

    })
    .As<IStateClient>()
    .InstancePerLifetimeScope();



    
    using (new ResolveFunctionAssembly())
    using (var container = builder.Build())
    {
        var myBuilder = new ContainerBuilder();
    
            myBuilder.Register(c => new CachingBotDataStore(c.Resolve<ConnectorStore>(),
                                                        CachingBotDataStoreConsistencyPolicy.LastWriteWins))
            .As<IBotDataStore<BotData>>()
            .AsSelf()
            .InstancePerLifetimeScope();

            myBuilder.Update(container);



        using (var scope = DialogModule.BeginLifetimeScope(container, activity))
        {
            DialogModule_MakeRoot.Register(scope, makeDialog);
            var task = scope.Resolve<IPostToBot>();
            await task.PostAsync(activity, CancellationToken.None);
        }
    }
}

public sealed class ResolveFunctionAssembly : IDisposable
{
    private readonly Assembly assembly;

    public ResolveFunctionAssembly()
    {
        this.assembly = Assembly.GetExecutingAssembly();
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    void IDisposable.Dispose()
    {
        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
    }
    
    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs arguments)
    {
        if (arguments.Name == this.assembly.FullName)
        {
            return this.assembly;
        }

        return null;
    }
}