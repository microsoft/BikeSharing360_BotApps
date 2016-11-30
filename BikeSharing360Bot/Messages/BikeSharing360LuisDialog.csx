#load ".\Backend\Bike.csx"
#load ".\Backend\BingMapHelper.csx"
#load ".\Backend\CustomerService.csx"
#load ".\Backend\Messages.csx"
#load ".\Backend\User.csx"

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Collections.Generic;


[Serializable]
[LuisModel("_YourModelId_", "_YourSubscriptionKey_")]
public class BikeSharing360LuisDialog : LuisDialog<object>
{
    private const string EntityDateTime = "builtin.datetime.time";

    [LuisIntent("")]
    [LuisIntent("None")]
    public async Task None(IDialogContext context, LuisResult result)
    {
        await HandleNoneAsync(context, result);
    }

    [LuisIntent("startconversation")]
    public async Task StartConversation(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
        await HandleStartConversationAsync(context, activity, result);
    }

    [LuisIntent("reportloss")]
    public async Task ReportLoss(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
        await HandleReportLossAsync(context, activity, result);
    }

    [LuisIntent("confirmlocation")]
    public async Task ConfirmLocationAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
        await HandleConfirmLocationAsync(context, activity, result);
    }

    private async Task findLostBike(IDialogContext context, LuisResult result)
    {
        //extract a dateTime entity from natural language
        EntityRecommendation datetimeEntityRecommendation;
        if (result.TryFindEntity(EntityDateTime, out datetimeEntityRecommendation))
        {
            //extract the time of day
            DateTime time;
            var succeed = DateTime.TryParse(datetimeEntityRecommendation.Resolution["time"], out time);
            if (!succeed)
            {
                await context.PostAsync(Messages.NoValidTime);
                context.Wait(this.MessageReceived);
                return;
            }

            //find the bike by ID
            string bikeid;
            bool found = context.UserData.TryGetValue("bikeid", out bikeid);
            if (!found)
            {
                await context.PostAsync(Messages.CannotFindBike);
                context.Wait(this.MessageReceived);
                return;
            }

            //send an immediate reply to the user saying we are locating their bike
            context.UserData.SetValue("state", "reporttime");
            string message = Messages.CustomerHangon;
            await context.PostAsync(message);

            //lookup the bike's locations - at the last known time and right now
            var oldLoc = await BikeData.LocateBike(bikeid, time);
            var currentLoc = await BikeData.LocateBike(bikeid);
            context.UserData.SetValue("lat", oldLoc.latitude);
            context.UserData.SetValue("lon", oldLoc.longitude);

            //tell user the bike's old and new locations
            message = string.Format(Messages.CustomerReplyLocation, result.Query.ToLower(), oldLoc.name, currentLoc.name);
            await context.PostAsync(message);

            //send a card with an image of the bike's location on a map
            var richmessage = context.MakeMessage();
            richmessage.Text = "Map";
            var mapurl = await BingMapHelper.HighlightRoute(oldLoc, currentLoc);
            attachImageToMessage(richmessage, mapurl);
            await context.PostAsync(richmessage);

            //now confirm the user's current location
            message = Messages.CustomerAskLocation;
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }
        else
        {
            context.Wait(this.MessageReceived);
        }

    }

    private void attachImageToMessage(IMessageActivity richmessage, string mapurl)
    {
        if (mapurl != "")
        {
            var heroCard = new HeroCard
            {
                Images = new List<CardImage> { new CardImage(mapurl) }
            };
            richmessage.Attachments.Add(heroCard.ToAttachment());
        }
    }

    public async Task HandleStartConversationAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
        var message = await activity;
        var name = message.From.Name;
        string reply = "Hello " + name;
        await context.PostAsync(reply);
        context.Wait(this.MessageReceived);
    }

    public async Task HandleNoneAsync(IDialogContext context, LuisResult result)
    {
        try
        {
            string state;
            bool found = context.UserData.TryGetValue("state", out state);
            if (found)
            {
                switch (state)
                {
                    case "reportloss":
                        await findLostBike(context, result);
                        break;
                }
            }
            else
            {
                string message = string.Format(Messages.CustomerDefault, result.Query);

                await context.PostAsync(message);

                context.Wait(this.MessageReceived);
            }
        }
        catch (Exception)
        {
            context.Wait(this.MessageReceived);
        }

    }

    public async Task HandleReportLossAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
        try
        {
            string name = "";
            string id = "";
            var message = await activity;

            var from = message.From.Id;
            User user = await User.LookupUser(from, ConnectorType.Skype);
            if (user == null)
            {
                await context.PostAsync(Messages.CannotFindUser);
                context.Wait(this.MessageReceived);
                return;
            }
            name = message.From.Name;
            id = user.userId;

            //look up bikes
            var bikes = await BikeData.LookupBikesWithUser(id);
            if (bikes != null && bikes.Count == 1)
                context.UserData.SetValue("bikeid", bikes[0].bikeid);
            else
            {
                await context.PostAsync(Messages.CannotFindBike);
                context.Wait(this.MessageReceived);
                return;
            }

            context.UserData.SetValue("id", id);
            context.UserData.SetValue("state", "reportloss");
            string reply = string.Format(Messages.CustomerAskTime, name);
            await context.PostAsync(reply);
            context.Wait(this.MessageReceived);
        }
        catch (Exception)
        {
            context.Wait(this.MessageReceived);
        }
    }

    public async Task HandleConfirmLocationAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
    {
        try
        {
            string state = "";
            if (context.UserData.TryGetValue("state", out state) && state == "reporttime")
            {
                var message = await activity;
                bool found = false;
                double lat, lon;
                string id = "";
                found = context.UserData.TryGetValue("lat", out lat);
                if (!found)
                {
                    context.Wait(this.MessageReceived);
                    return;
                }
                found = context.UserData.TryGetValue("lon", out lon);
                if (!found)
                {
                    context.Wait(this.MessageReceived);
                    return;
                }
                found = context.UserData.TryGetValue("id", out id);
                if (!found)
                {
                    context.Wait(this.MessageReceived);
                    return;
                }
                context.UserData.SetValue("state", "");
                string msg = Messages.CustomerReportPolice;
                await context.PostAsync(msg);

                await Task.Delay(3000);

                string ticketnumber = await CustomerService.FileCase(id, IncidentType.lost, lat, lon);
                int eta = await CustomerService.GetETA(ticketnumber);
                msg = string.Format(Messages.CustomerReplyCaseNumber, ticketnumber, eta.ToString());
                await context.PostAsync(msg);

                context.Wait(this.MessageReceived);
            }
            else
            {
                string message = string.Format(Messages.CustomerDefault, result.Query);

                await context.PostAsync(message);

                context.Wait(this.MessageReceived);
                return;
            }
        }
        catch (Exception)
        {
            context.Wait(this.MessageReceived);
        }

    }
}

