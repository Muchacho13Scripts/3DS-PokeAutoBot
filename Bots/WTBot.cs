﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord;
using PKHeX.Core;
using static _3DS_link_trade_bot.dsbotbase;
using static _3DS_link_trade_bot.dsbotbase.Buttons;
using static _3DS_link_trade_bot.MainHub;
using static _3DS_link_trade_bot.Form1;
using static _3DS_link_trade_bot.RAM;
using static _3DS_link_trade_bot.discordmain;
using static _3DS_link_trade_bot.NTRClient;
namespace _3DS_link_trade_bot
{
    public class WTBot
    {
        public static int weirds_users_suck = 30_000;
        public static async Task WTroutine()
        {
            var rng = new Random();
     
            if (!infestivalplaza)
            {
                ChangeStatus("entering festival plaza");
                await click(X, 1);
                await touch(229, 171, 10);
                await touch(296, 221, 5);
                await click(B, 10);

            }
            if (!isconnected)
            {
                ChangeStatus("connecting to the internet");
                await touch(296, 221, 5);
                await click(A, 2);
                //start and wait for connection
                await click(A, 30);
                await click(A, 5);
                //start and wait for checking status
                await click(A, 20);
                
            }
            if (userinvitedbot)
                await click(X, 1);
            ChangeStatus("starting WT distribution task");
            await touch(235, 130, 1);
            await touch(165, 165, 5);
            var thegift = wtmons[rng.Next(wtmons.Count())];
            string shiney = thegift.IsShiny ? "Shiny " : "";
            string eggy = thegift.IsEgg ? " (Egg)" : "";
            await Gen7LinkTradeBot.injection(thegift);
            SetText($"Wondertrading now:\n{shiney}{(Species)thegift.Species}{eggy}");
            await click(A, 5);
            for (int i = 0; i < 2; i++)
                await click(A, 1);
            foreach (var chan in _settings.Discordsettings.BotWTChannel)
            {
                var tosend = (ITextChannel)_client.GetChannel(chan);


                EmbedBuilder embed = new EmbedBuilder();
                try
                {
                    embed.ThumbnailUrl = thegift.IsShiny ? $"https://play.pokemonshowdown.com/sprites/ani-shiny/{((Species)thegift.Species).ToString().ToLower().Replace(" ", "")}.gif" : $"https://play.pokemonshowdown.com/sprites/ani/{((Species)thegift.Species).ToString().ToLower().Replace(" ", "")}.gif";
                }
                catch { }
                var newShowdown = new List<string>();
                thegift.ClearNickname();
                var showdown = ShowdownParsing.GetShowdownText(thegift);
                foreach (var line in showdown.Split('\n'))
                    newShowdown.Add(line);

                if (thegift.IsEgg)
                    newShowdown.Add("\nPokémon is an egg");
                if (thegift.Ball > (int)Ball.None)
                    newShowdown.Insert(newShowdown.FindIndex(z => z.Contains("Nature")), $"Ball: {(Ball)thegift.Ball} Ball");
                if (thegift.IsShiny)
                {
                    var index = newShowdown.FindIndex(x => x.Contains("Shiny: Yes"));
                    if (thegift.ShinyXor == 0 || thegift.FatefulEncounter)
                        newShowdown[index] = "Shiny: Square\r";
                    else newShowdown[index] = "Shiny: Star\r";
                }



                embed.AddField("Wonder trading in 15 seconds", Format.Code(string.Join("\n", newShowdown).TrimEnd()));
                try
                {
                    await tosend.SendMessageAsync(embed: embed.Build());
                }
                catch (Exception ex) { await Log(ex.ToString()); }
            }
                await Task.Delay(10_000);
         

                for (int i = 5; i > 0; i--)
                {
                foreach (var chan in _settings.Discordsettings.BotWTChannel)
                {

                    var tosend = (ITextChannel)_client.GetChannel(chan);
                    await tosend.SendMessageAsync(Format.Code($"{i}"));
                }
                await Task.Delay(1000);
                }



            foreach (var chan in _settings.Discordsettings.BotWTChannel)
            {
                var tosend = (ITextChannel)_client.GetChannel(chan);
                await tosend.SendMessageAsync(Format.Code("Starting Wondertrade now!"));
            }
                ChangeStatus($"Wonder Trading: {(Species)thegift.Species}");
            
                var stop = new Stopwatch();
                await click(A, 3);
                stop.Restart();
                if (BitConverter.ToInt16(ntr.ReadBytes(screenoff, 2)) != 0x41A8)
                {
                    while (BitConverter.ToInt16(ntr.ReadBytes(screenoff, 2)) != start_seekscreen && stop.ElapsedMilliseconds < weirds_users_suck)
                        await click(B, 1);
                    await click(B, 5);
                    return;
                }
            var receivingpkm = EntityFormat.GetFromBytes(ntr.ReadBytes(WTReceivingPokemon, 232));
          
            while (stop.ElapsedMilliseconds < 90_000)
            {
                if(receivingpkm != null)
                {
                    if (receivingpkm.ChecksumValid)
                        break;
                }
                receivingpkm = EntityFormat.GetFromBytes(ntr.ReadBytes(WTReceivingPokemon, 232));
             
                await Task.Delay(1000);
            }
            var matchedtrainerbytes = ntr.ReadBytes(WTTrainerMatch, 24);
            var matchedtrainer = Encoding.Unicode.GetString(matchedtrainerbytes).Trim('\0');
            while(matchedtrainer == "１２３４５６")
            {
                matchedtrainerbytes = ntr.ReadBytes(WTTrainerMatch, 24);
                matchedtrainer = Encoding.Unicode.GetString(matchedtrainerbytes).Trim('\0');
            }
            matchedtrainer = Encoding.Unicode.GetString(matchedtrainerbytes).Trim('\0').Trim('６').Trim('５').Trim('４');
            ChangeStatus($"WT Match Found: Trainer: {matchedtrainer} Incoming: {(Species)receivingpkm.Species}");
            SetText($"Trainer Found: {matchedtrainer}!\nTrading Now...");
            foreach (var chan in _settings.Discordsettings.BotWTChannel)
            {
                var tosend = (ITextChannel)_client.GetChannel(chan);

                EmbedBuilder embed = new EmbedBuilder();
                try
                {
                    embed.ThumbnailUrl = receivingpkm.IsShiny ? $"https://play.pokemonshowdown.com/sprites/ani-shiny/{((Species)receivingpkm.Species).ToString().ToLower().Replace(" ", "")}.gif" : $"https://play.pokemonshowdown.com/sprites/ani/{((Species)receivingpkm.Species).ToString().ToLower().Replace(" ", "")}.gif";
                }
                catch { }
                var newShowdown = new List<string>();
               
                var showdown = ShowdownParsing.GetShowdownText(receivingpkm);
                foreach (var line in showdown.Split('\n'))
                    newShowdown.Add(line);

                if (receivingpkm.IsEgg)
                    newShowdown.Add("\nPokémon is an egg");
                if (receivingpkm.Ball > (int)Ball.None)
                    newShowdown.Insert(newShowdown.FindIndex(z => z.Contains("Nature")), $"Ball: {(Ball)receivingpkm.Ball} Ball");
                if (receivingpkm.IsShiny)
                {
                    var index = newShowdown.FindIndex(x => x.Contains("Shiny: Yes"));
                    if (receivingpkm.ShinyXor == 0 || receivingpkm.FatefulEncounter)
                        newShowdown[index] = "Shiny: Square\r";
                    else newShowdown[index] = "Shiny: Star\r";
                }
                embed.AddField($"WT Match Found!\nTrainer: {matchedtrainer}", Format.Code(string.Join("\n", newShowdown).TrimEnd()));
                try
                {
                    await tosend.SendMessageAsync(embed: embed.Build());
                }
                catch (Exception ex) { await Log(ex.ToString()); }
            }
            stop.Restart();
                while(BitConverter.ToInt16(ntr.ReadBytes(WTReceivingPokemon, 2)) != 0&&stop.ElapsedMilliseconds < 60_000)
                {

                    await touch(155, 151, 1);
                }
                ChangeStatus("Wonder Trade Complete");
            SetText($"Wonder Trade Complete!\nNext Trade Incoming!");
            stop.Restart();
                while (!infestivalplaza&&stop.ElapsedMilliseconds < 60_000)
                    await click(B, 2);
                await Task.Delay(5_000);

            


        }
        private static void SetText(string text)
        {
            System.IO.File.WriteAllText($"BotStatus.txt", text);
        }
    }

}
