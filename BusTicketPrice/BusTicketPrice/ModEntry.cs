using System;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using GenericModConfigMenu;

namespace BusTicketPrice
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config = null!;

        public override void Entry(IModHelper helper)
        {
            // Load config file
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // Validate config
            if (this.Config.DesertBusTicketPrice < 0)
            {
                this.Monitor.Log("Price cannot be negative. Setting to default value of 100.", LogLevel.Warn);
                this.Config.DesertBusTicketPrice = 100;
                this.Helper.WriteConfig(this.Config);
            }

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add some config options
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Desert Bus Ticket Price",
                tooltip: () => "0 - 9999",
                getValue: () => this.Config.DesertBusTicketPrice,
                setValue: value => this.Config.DesertBusTicketPrice = value,
                min: 0,
                max: 9999
            );
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // Set price immediately when save is loaded
            SetBusPrice();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            // Set price at start of each day
            SetBusPrice();
        }

        private void SetBusPrice()
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.getLocationFromName("BusStop") is BusStop busStop)
            {
                busStop.TicketPrice = this.Config.DesertBusTicketPrice;
                this.Monitor.Log($"Bus ticket price set to {busStop.TicketPrice}g", LogLevel.Debug);
            }
            else
            {
                this.Monitor.Log("Could not find BusStop location", LogLevel.Warn);
            }
        }
    }
}