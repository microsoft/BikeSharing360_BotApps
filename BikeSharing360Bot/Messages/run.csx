#r "Newtonsoft.Json"
#load "PostDialogExtensions.csx"
#load "BikeSharing360LuisDialog.csx"

using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!"); 

    string jsonContent = await req.Content.ReadAsStringAsync();

    var activity = JsonConvert.DeserializeObject<Activity>(jsonContent);
    
    if (activity != null)
    {
        // one of these will have an interface and process it
        switch (activity.GetActivityType())
        {
            case ActivityTypes.Message:
                await activity.PostInScratchAsync(() => new BikeSharing360LuisDialog());
                break;
            case ActivityTypes.ConversationUpdate:
                IConversationUpdateActivity update = activity;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    if (update.MembersAdded.Any())
                    {
                        var reply = activity.CreateReply();
                        var newMembers = update.MembersAdded?.Where(t => t.Id != activity.Recipient.Id);
                        foreach (var newMember in newMembers)
                        {
                            reply.Text = "Hi";
                            if (!string.IsNullOrEmpty(newMember.Name))
                            {
                                reply.Text += $" {newMember.Name}";
                            }
                            reply.Text += ", what can I help you with?";
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
                break;
            case ActivityTypes.ContactRelationUpdate:
            case ActivityTypes.Typing:
            case ActivityTypes.DeleteUserData:
            case ActivityTypes.Ping:
            default:
                log.Error($"Unknown activity type ignored: {activity.GetActivityType()}");
                break;
        }
    }
    return req.CreateResponse(HttpStatusCode.Accepted);    
}
